using System.ComponentModel.DataAnnotations;

namespace DatLichKhamBenh.Models.ViewModels;

// Form dang ky tai khoan benh nhan
public class DangKyViewModel
{
    [Required(ErrorMessage = "Vui long nhap ten dang nhap")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "Ten dang nhap tu 3-50 ky tu")]
    [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Ten dang nhap chi gom chu, so, dau cham, gach duoi, gach ngang")]
    [Display(Name = "Ten dang nhap")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mat khau tu 6-100 ky tu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau")]
    public string MatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long xac nhan mat khau")]
    [Compare(nameof(MatKhau), ErrorMessage = "Mat khau xac nhan khong khop")]
    [DataType(DataType.Password)]
    [Display(Name = "Xac nhan mat khau")]
    public string XacNhanMatKhau { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap ho ten")]
    [StringLength(100)]
    [Display(Name = "Ho va ten")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap email")]
    [EmailAddress(ErrorMessage = "Email khong hop le")]
    [StringLength(100)]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "So dien thoai khong hop le")]
    [StringLength(20)]
    [Display(Name = "So dien thoai")]
    public string? SoDienThoai { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Ngay sinh")]
    public DateTime? NgaySinh { get; set; }

    [StringLength(10)]
    [Display(Name = "Gioi tinh")]
    public string? GioiTinh { get; set; }

    [StringLength(255)]
    [Display(Name = "Dia chi")]
    public string? DiaChi { get; set; }
}
