using DatLichKhamBenh.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace DatLichKhamBenh.Services;

// Trien khai gui mail bang MailKit.
// Khi EmailSettings chua duoc cau hinh (van la placeholder) thi log noi dung ra
// console thay vi thuc su mo ket noi SMTP -> tien khi demo offline.
public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        if (!_settings.IsConfigured)
        {
            _logger.LogWarning(
                "[EMAIL-DEV] EmailSettings chua duoc cau hinh -> KHONG gui mail." +
                " To: {ToEmail} ({ToName}) | Subject: {Subject}\nBody:\n{Body}",
                toEmail, toName, subject, htmlBody);
            await Task.CompletedTask;
            return;
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
        message.Body = bodyBuilder.ToMessageBody();

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_settings.SmtpServer, _settings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_settings.SenderEmail, _settings.SenderPassword);
            await smtp.SendAsync(message);
            _logger.LogInformation("[EMAIL] Da gui mail toi {ToEmail} - {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[EMAIL] Loi khi gui mail toi {ToEmail}", toEmail);
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

        var subject = $"[DatLichKhamBenh] Da nhan lich hen #{lichHen.MaLichHen} - cho xac nhan";
        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#2563eb'>Xac nhan da nhan lich hen</h2>
                <p>Xin chao <strong>{bn.HoTen}</strong>,</p>
                <p>Chung toi da ghi nhan yeu cau dat lich kham cua ban voi thong tin sau:</p>
                <table style='border-collapse:collapse;margin:12px 0'>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ma lich hen</td><td style='padding:6px 12px'><strong>#{lichHen.MaLichHen}</strong></td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Bac si</td><td style='padding:6px 12px'>{bs?.HoTen} ({ck})</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ngay kham</td><td style='padding:6px 12px'>{lichHen.NgayKham:dd/MM/yyyy}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Gio kham</td><td style='padding:6px 12px'>{lichHen.GioKham:hh\:mm}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ly do</td><td style='padding:6px 12px'>{lichHen.LyDoKham ?? "(khong co)"}</td></tr>
                </table>
                <p>Lich hen dang o trang thai <strong style='color:#f59e0b'>CHO XAC NHAN</strong>.
                Chung toi se gui email khi bac si xac nhan lich hen cua ban.</p>
                <p style='color:#666;font-size:12px'>Email tu dong tu he thong DatLichKhamBenh - vui long khong tra loi.</p>
            </div>";

        return SendAsync(bn.Email, bn.HoTen, subject, html);
    }

    public Task GuiMailXacNhanLichAsync(LichHen lichHen)
    {
        var bn = lichHen.BenhNhan?.NguoiDung;
        var bs = lichHen.BacSi?.NguoiDung;
        var ck = lichHen.BacSi?.ChuyenKhoa?.TenChuyenKhoa ?? "";

        if (bn is null) return Task.CompletedTask;

        var subject = $"[DatLichKhamBenh] Lich hen #{lichHen.MaLichHen} da duoc xac nhan";
        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#10b981'>Lich hen da duoc xac nhan</h2>
                <p>Xin chao <strong>{bn.HoTen}</strong>,</p>
                <p>Bac si <strong>{bs?.HoTen}</strong> ({ck}) da <strong style='color:#10b981'>XAC NHAN</strong> lich kham cua ban.</p>
                <table style='border-collapse:collapse;margin:12px 0'>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ma lich hen</td><td style='padding:6px 12px'><strong>#{lichHen.MaLichHen}</strong></td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ngay kham</td><td style='padding:6px 12px'>{lichHen.NgayKham:dd/MM/yyyy}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Gio kham</td><td style='padding:6px 12px'>{lichHen.GioKham:hh\:mm}</td></tr>
                </table>
                <p>Vui long den dung gio. Neu khong the den, hay huy lich som tren he thong.</p>
                <p style='color:#666;font-size:12px'>Email tu dong tu he thong DatLichKhamBenh - vui long khong tra loi.</p>
            </div>";

        return SendAsync(bn.Email, bn.HoTen, subject, html);
    }

    public Task GuiMailHuyLichAsync(LichHen lichHen, string? lyDo = null)
    {
        var bn = lichHen.BenhNhan?.NguoiDung;
        var bs = lichHen.BacSi?.NguoiDung;

        if (bn is null) return Task.CompletedTask;

        var subject = $"[DatLichKhamBenh] Lich hen #{lichHen.MaLichHen} da bi huy";
        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#ef4444'>Lich hen da bi huy</h2>
                <p>Xin chao <strong>{bn.HoTen}</strong>,</p>
                <p>Lich hen sau day da bi huy:</p>
                <table style='border-collapse:collapse;margin:12px 0'>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ma lich hen</td><td style='padding:6px 12px'><strong>#{lichHen.MaLichHen}</strong></td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Bac si</td><td style='padding:6px 12px'>{bs?.HoTen}</td></tr>
                    <tr><td style='padding:6px 12px;background:#f3f4f6'>Ngay kham</td><td style='padding:6px 12px'>{lichHen.NgayKham:dd/MM/yyyy} {lichHen.GioKham:hh\:mm}</td></tr>
                    {(string.IsNullOrWhiteSpace(lyDo) ? "" : $"<tr><td style='padding:6px 12px;background:#f3f4f6'>Ly do</td><td style='padding:6px 12px'>{lyDo}</td></tr>")}
                </table>
                <p>Neu can dat lai, vui long truy cap he thong de dat lich moi.</p>
                <p style='color:#666;font-size:12px'>Email tu dong tu he thong DatLichKhamBenh - vui long khong tra loi.</p>
            </div>";

        return SendAsync(bn.Email, bn.HoTen, subject, html);
    }
}
