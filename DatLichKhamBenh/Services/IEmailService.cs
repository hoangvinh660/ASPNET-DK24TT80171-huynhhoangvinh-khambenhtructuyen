using DatLichKhamBenh.Models;

namespace DatLichKhamBenh.Services;

// Interface gui mail cho 3 su kien chinh + 1 phuong thuc thap chung.
public interface IEmailService
{
    // Khi benh nhan vua dat lich -> gui cho benh nhan email xac nhan da nhan lich.
    Task GuiMailDatLichAsync(LichHen lichHen);

    // Khi bac si xac nhan lich -> gui cho benh nhan biet lich da duoc xac nhan.
    Task GuiMailXacNhanLichAsync(LichHen lichHen);

    // Khi lich bi huy (do benh nhan / bac si / admin) -> gui cho benh nhan biet.
    Task GuiMailHuyLichAsync(LichHen lichHen, string? lyDo = null);

    // Method co ban: gui 1 email HTML toi 1 dia chi.
    Task SendAsync(string toEmail, string toName, string subject, string htmlBody);
}
