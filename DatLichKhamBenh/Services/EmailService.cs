using DatLichKhamBenh.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace DatLichKhamBenh.Services;

// Triển khai gửi mail bằng MailKit.
// Đọc cấu hình từ IEmailSettingsProvider (DB - bảng CauHinhEmail).
// 3 nhánh hành vi:
//   1) BatEmail = false (admin tắt)          -> chỉ log [EMAIL-OFF], không gửi.
//   2) BatEmail = true nhưng chưa cấu hình   -> log [EMAIL-DEV], không gửi (tiện demo).
//   3) BatEmail = true + cấu hình hợp lệ     -> gửi thật qua SMTP.
public class EmailService : IEmailService
{
    private readonly IEmailSettingsProvider _provider;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IEmailSettingsProvider provider, ILogger<EmailService> logger)
    {
        _provider = provider;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        var enabled = await _provider.IsEnabledAsync();
        if (!enabled)
        {
            _logger.LogInformation(
                "[EMAIL-OFF] Admin đã tắt email -> KHÔNG gửi. To: {ToEmail} | Subject: {Subject}",
                toEmail, subject);
            return;
        }

        var settings = await _provider.GetAsync();
        if (!settings.IsConfigured)
        {
            _logger.LogWarning(
                "[EMAIL-DEV] EmailSettings chưa được cấu hình -> KHÔNG gửi mail." +
                " To: {ToEmail} ({ToName}) | Subject: {Subject}\nBody:\n{Body}",
                toEmail, toName, subject, htmlBody);
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(settings.SenderName, settings.SenderEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(settings.SmtpServer, settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(settings.SenderEmail, settings.SenderPassword);
            await smtp.SendAsync(message);
            _logger.LogInformation("[EMAIL] Đã gửi mail tới {ToEmail} - {Subject}", toEmail, subject);
        }
        catch (AuthenticationException ex)
        {
            _logger.LogError(ex,
                "[EMAIL] Sai thông tin đăng nhập SMTP ({SenderEmail}). " +
                "Với Gmail: bật xác minh 2 bước và tạo App Password tại https://myaccount.google.com/apppasswords. " +
                "Cấu hình tại /Admin/Email hoặc TẮT gửi mail nếu đang demo.",
                settings.SenderEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL] Lỗi khi gửi mail tới {ToEmail}", toEmail);
        }
        finally
        {
            if (smtp.IsConnected) await smtp.DisconnectAsync(true);
        }
    }

    public Task GuiMailDatLichAsync(LichHen lichHen)
    {
        var bn = lichHen.BenhNhan?.NguoiDung;
        var bs = lichHen.BacSi?.NguoiDung;
        var ck = lichHen.BacSi?.ChuyenKhoa?.TenChuyenKhoa ?? "";

        if (bn is null) return Task.CompletedTask;

        var subject = $"[DatLichKhamBenh] Đã nhận lịch hẹn #{lichHen.MaLichHen} - chờ xác nhận";
        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#2563eb'>Xác nhận đã nhận lịch hẹn</h2>
                <p>Xin chào <strong>{bn.HoTen}</strong>,</p>
                <p>Chúng tôi đã ghi nhận yêu cầu đặt lịch khám của bạn với thông tin sau:</p>
                <table style='border-collapse:collapse;margin:12px 0'>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Mã lịch hẹn</td><td style='padding:6px 12px'><strong>#{lichHen.MaLichHen}</strong></td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Bác sĩ</td><td style='padding:6px 12px'>{bs?.HoTen} ({ck})</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ngày khám</td><td style='padding:6px 12px'>{lichHen.NgayKham:dd/MM/yyyy}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Giờ khám</td><td style='padding:6px 12px'>{lichHen.GioKham:hh\:mm}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Lý do</td><td style='padding:6px 12px'>{lichHen.LyDoKham ?? "(không có)"}</td></tr>
                </table>
                <p>Lịch hẹn đang ở trạng thái <strong style='color:#f59e0b'>CHỜ XÁC NHẬN</strong>.
                Chúng tôi sẽ gửi email khi bác sĩ xác nhận lịch hẹn của bạn.</p>
                <p style='color:#666;font-size:12px'>Email tự động từ hệ thống DatLichKhamBenh - vui lòng không trả lời.</p>
            </div>";

        return SendAsync(bn.Email, bn.HoTen, subject, html);
    }

    public Task GuiMailXacNhanLichAsync(LichHen lichHen)
    {
        var bn = lichHen.BenhNhan?.NguoiDung;
        var bs = lichHen.BacSi?.NguoiDung;
        var ck = lichHen.BacSi?.ChuyenKhoa?.TenChuyenKhoa ?? "";

        if (bn is null) return Task.CompletedTask;

        var subject = $"[DatLichKhamBenh] Lịch hẹn #{lichHen.MaLichHen} đã được xác nhận";
        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#10b981'>Lịch hẹn đã được xác nhận</h2>
                <p>Xin chào <strong>{bn.HoTen}</strong>,</p>
                <p>Bác sĩ <strong>{bs?.HoTen}</strong> ({ck}) đã <strong style='color:#10b981'>XÁC NHẬN</strong> lịch khám của bạn.</p>
                <table style='border-collapse:collapse;margin:12px 0'>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Mã lịch hẹn</td><td style='padding:6px 12px'><strong>#{lichHen.MaLichHen}</strong></td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ngày khám</td><td style='padding:6px 12px'>{lichHen.NgayKham:dd/MM/yyyy}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Giờ khám</td><td style='padding:6px 12px'>{lichHen.GioKham:hh\:mm}</td></tr>
                </table>
                <p>Vui lòng đến đúng giờ. Nếu không thể đến, hãy hủy lịch sớm trên hệ thống.</p>
                <p style='color:#666;font-size:12px'>Email tự động từ hệ thống DatLichKhamBenh - vui lòng không trả lời.</p>
            </div>";

        return SendAsync(bn.Email, bn.HoTen, subject, html);
    }

    public Task GuiMailHuyLichAsync(LichHen lichHen, string? lyDo = null)
    {
        var bn = lichHen.BenhNhan?.NguoiDung;
        var bs = lichHen.BacSi?.NguoiDung;

        if (bn is null) return Task.CompletedTask;

        var subject = $"[DatLichKhamBenh] Lịch hẹn #{lichHen.MaLichHen} đã bị hủy";
        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#ef4444'>Lịch hẹn đã bị hủy</h2>
                <p>Xin chào <strong>{bn.HoTen}</strong>,</p>
                <p>Lịch hẹn sau đây đã bị hủy:</p>
                <table style='border-collapse:collapse;margin:12px 0'>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Mã lịch hẹn</td><td style='padding:6px 12px'><strong>#{lichHen.MaLichHen}</strong></td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Bác sĩ</td><td style='padding:6px 12px'>{bs?.HoTen}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ngày khám</td><td style='padding:6px 12px'>{lichHen.NgayKham:dd/MM/yyyy} {lichHen.GioKham:hh\:mm}</td></tr>
                    {(string.IsNullOrWhiteSpace(lyDo) ? "" : $"<tr><td style='padding:6px 12px;background:#f3f4f6'>Lý do</td><td style='padding:6px 12px'>{lyDo}</td></tr>")}
                </table>
                <p>Nếu cần đặt lại, vui lòng truy cập hệ thống để đặt lịch mới.</p>
                <p style='color:#666;font-size:12px'>Email tự động từ hệ thống DatLichKhamBenh - vui lòng không trả lời.</p>
            </div>";

        return SendAsync(bn.Email, bn.HoTen, subject, html);
    }
}
