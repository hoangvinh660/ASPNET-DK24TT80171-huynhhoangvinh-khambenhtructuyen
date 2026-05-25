using DatLichKhamBenh.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatLichKhamBenh.Services;

public class EmailSettingsProvider : IEmailSettingsProvider
{
    private readonly AppDbContext _db;
    private readonly EmailSettings _fallback;

    public EmailSettingsProvider(AppDbContext db, IOptions<EmailSettings> fallback)
    {
        _db = db;
        _fallback = fallback.Value;
    }

    public async Task<CauHinhEmail> GetEntityAsync()
    {
        var ch = await _db.CauHinhEmails.AsNoTracking().FirstOrDefaultAsync();
        if (ch is not null) return ch;

        // DB chua co record -> tra ve doi tuong tam dua tren appsettings (chua luu DB)
        return new CauHinhEmail
        {
            BatEmail = true,
            SmtpServer = _fallback.SmtpServer,
            Port = _fallback.Port,
            SenderName = _fallback.SenderName,
            SenderEmail = _fallback.SenderEmail,
            SenderPassword = _fallback.SenderPassword
        };
    }

    public async Task<EmailSettings> GetAsync()
    {
        var ch = await GetEntityAsync();
        return new EmailSettings
        {
            SmtpServer = ch.SmtpServer,
            Port = ch.Port,
            SenderName = ch.SenderName,
            SenderEmail = ch.SenderEmail,
            SenderPassword = ch.SenderPassword
        };
    }

    public async Task<bool> IsEnabledAsync()
    {
        var ch = await GetEntityAsync();
        return ch.BatEmail;
    }
}
