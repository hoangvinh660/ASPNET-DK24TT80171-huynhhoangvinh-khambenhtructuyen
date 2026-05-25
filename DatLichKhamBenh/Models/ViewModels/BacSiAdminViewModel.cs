using System.ComponentModel.DataAnnotations;

namespace DatLichKhamBenh.Models.ViewModels;

// View model cho Admin tao/sua bac si (gom thong tin NguoiDung + BacSi).
public class BacSiAdminViewModel
{
    // ==== Khoa chinh BacSi (chi co khi sua) ====
    public int? MaBacSi { get; set; }
    public int? MaNguoiDung { get; set; }

    // ==== Thong tin tai khoan NguoiDung ====
    [Required(ErrorMessage = "Vui long nhap ten dang nhap")]
    [StringLength(50)]
    [Display(Name = "Ten dang nhap")]
    public string TenDangNhap { get; set; } = string.Empty;

    // Bat buoc khi tao moi; khi sua de trong = khong doi mat khau
    [StringLength(100, MinimumLength = 4, ErrorMessage = "Mat khau toi thieu 4 ky tu")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau")]
    public string? MatKhau { get; set; }

    [Required(ErrorMessage = "Vui long nhap ho ten")]
    [StringLength(100)]
    [Display(Name = "Ho va ten")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap email")]
    [EmailAddress(ErrorMessage = "Email khong hop le")]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "So dien thoai khong hop le")]
    [StringLength(20)]
    [Display(Name = "So dien thoai")]
    public string? SoDienThoai { get; set; }

    // ==== Thong tin BacSi ====
    [Required(ErrorMessage = "Vui long chon chuyen khoa")]
    [Display(Name = "Chuyen khoa")]
    public int MaChuyenKhoa { get; set; }

    [Required(ErrorMessage = "Vui long nhap hoc vi")]
    [StringLength(100)]
    [Display(Name = "Hoc vi")]
    public string HocVi { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Kinh nghiem")]
    public string? KinhNghiem { get; set; }

    [StringLength(2000)]
    [Display(Name = "Mo ta")]
    public string? MoTa { get; set; }

    [StringLength(255)]
    [Display(Name = "Duong dan anh")]
    public string? HinhAnh { get; set; }

    [Required(ErrorMessage = "Vui long nhap gia kham")]
    [Range(0, 100_000_000, ErrorMessage = "Gia kham phai >= 0")]
    [Display(Name = "Gia kham (VND)")]
    public decimal GiaKham { get; set; }
}
