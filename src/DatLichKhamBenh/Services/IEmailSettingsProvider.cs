using DatLichKhamBenh.Models;

namespace DatLichKhamBenh.Services;

// Doc cau hinh email tu DB (bang CauHinhEmail).
// Neu DB chua co record nao -> fallback appsettings.json (IOptions<EmailSettings>).
public interface IEmailSettingsProvider
{
    Task<EmailSettings> GetAsync();
    Task<bool> IsEnabledAsync();
    Task<CauHinhEmail> GetEntityAsync();
}
