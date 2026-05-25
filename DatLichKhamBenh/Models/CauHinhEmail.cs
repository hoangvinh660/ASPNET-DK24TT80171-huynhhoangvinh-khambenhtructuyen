using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatLichKhamBenh.Models;

// Cau hinh email SMTP do admin chinh sua truc tiep trong UI.
// Bang nay chi luu 1 record duy nhat (MaCauHinh = 1) - quan ly nhu singleton.
[Table("CauHinhEmail")]
public class CauHinhEmail
{
    [Key]
    public int MaCauHinh { get; set; }

    // Cong tac master: false -> EmailService bo qua moi yeu cau gui mail.
    public bool BatEmail { get; set; } = true;

    [Required]
    [StringLength(255)]
    public string SmtpServer { get; set; } = "smtp.gmail.com";

    [Range(1, 65535)]
    public int Port { get; set; } = 587;

    [Required]
    [StringLength(255)]
    public string SenderName { get; set; } = "Phong kham DatLichKhamBenh";

    [Required]
    [StringLength(255)]
    [EmailAddress]
    public string SenderEmail { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string SenderPassword { get; set; } = string.Empty;

    public DateTime NgayCapNhat { get; set; } = DateTime.Now;
}
