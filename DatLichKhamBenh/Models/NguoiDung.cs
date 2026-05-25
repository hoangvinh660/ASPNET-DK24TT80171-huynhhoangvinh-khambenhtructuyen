using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatLichKhamBenh.Models;

// Tai khoan he thong, dung chung cho Admin/BacSi/BenhNhan
public class NguoiDung
{
    [Key]
    public int MaNguoiDung { get; set; }

    [Required(ErrorMessage = "Vui long nhap ten dang nhap")]
    [StringLength(50)]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau")]
    [StringLength(100)]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap ho ten")]
    [StringLength(100)]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap email")]
    [EmailAddress(ErrorMessage = "Email khong hop le")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "So dien thoai khong hop le")]
    [StringLength(20)]
    public string? SoDienThoai { get; set; }

    // Admin / BacSi / BenhNhan
    [Required]
    [StringLength(20)]
    public string VaiTro { get; set; } = "BenhNhan";

    // Cho phep Admin khoa tai khoan
    public bool DaKhoa { get; set; } = false;

    public DateTime NgayTao { get; set; } = DateTime.Now;

    // Navigation: 1 NguoiDung co the la 1 BacSi hoac 1 BenhNhan
    public virtual BacSi? BacSi { get; set; }
    public virtual BenhNhan? BenhNhan { get; set; }
}
