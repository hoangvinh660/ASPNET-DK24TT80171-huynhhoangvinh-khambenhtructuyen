using System.ComponentModel.DataAnnotations;

namespace DatLichKhamBenh.Models;

// Chuyen khoa kham benh (Noi, Ngoai, San, Nhi, Mat...)
public class ChuyenKhoa
{
    [Key]
    public int MaChuyenKhoa { get; set; }

    [Required(ErrorMessage = "Vui long nhap ten chuyen khoa")]
    [StringLength(100)]
    public string TenChuyenKhoa { get; set; } = string.Empty;

    [StringLength(500)]
    public string? MoTa { get; set; }

    // Navigation: 1 chuyen khoa co nhieu bac si
    public virtual ICollection<BacSi> DanhSachBacSi { get; set; } = new List<BacSi>();
}
