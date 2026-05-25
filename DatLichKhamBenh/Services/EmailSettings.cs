namespace DatLichKhamBenh.Services;

// Cau hinh SMTP, load tu appsettings.json -> "EmailSettings"
public class EmailSettings
{
    public string SmtpServer { get; set; } = "smtp.gmail.com";
    public int Port { get; set; } = 587;
    public string SenderName { get; set; } = "Phong kham DatLichKhamBenh";
    public string SenderEmail { get; set; } = string.Empty;
    public string SenderPassword { get; set; } = string.Empty;

    // True khi config van con la placeholder -> EmailService se chay che do "dev log"
    // thay vi thuc su gui mail qua SMTP.
    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(SenderEmail)
        && !SenderEmail.Contains("your-email", StringComparison.OrdinalIgnoreCase)
        && !string.IsNullOrWhiteSpace(SenderPassword)
        && !SenderPassword.Contains("app-password", StringComparison.OrdinalIgnoreCase);
}
