using System.ComponentModel.DataAnnotations;

namespace DatLichKhamBenh.Models.ViewModels;

// Form dang nhap
public class DangNhapViewModel
{
    [Required(ErrorMessage = "Vui long nhap ten dang nhap")]
    [Display(Name = "Ten dang nhap")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui long nhap mat khau")]
    [DataType(DataType.Password)]
    [Display(Name = "Mat khau")]
    public string MatKhau { get; set; } = string.Empty;

    [Display(Name = "Ghi nho dang nhap")]
    public bool GhiNho { get; set; }

    // URL chuyen huong sau khi dang nhap thanh cong (do cookie auth dien vao)
    public string? ReturnUrl { get; set; }
}
