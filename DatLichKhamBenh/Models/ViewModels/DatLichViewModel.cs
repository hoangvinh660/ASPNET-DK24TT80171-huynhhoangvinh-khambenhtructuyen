using System.ComponentModel.DataAnnotations;
using DatLichKhamBenh.Models;

namespace DatLichKhamBenh.Models.ViewModels;

// View model cho form dat lich kham
public class DatLichViewModel
{
    [Required(ErrorMessage = "Vui long chon bac si")]
    [Display(Name = "Bac si")]
    public int MaBacSi { get; set; }

    [Required(ErrorMessage = "Vui long chon ngay kham")]
    [DataType(DataType.Date)]
    [Display(Name = "Ngay kham")]
    public DateTime NgayKham { get; set; } = DateTime.Today.AddDays(1);

    [Required(ErrorMessage = "Vui long chon gio kham")]
    [Display(Name = "Gio kham")]
    public string GioKhamStr { get; set; } = string.Empty; // dang "HH:mm"

    [StringLength(500)]
    [Display(Name = "Ly do kham / Trieu chung")]
    public string? LyDoKham { get; set; }

    // Du lieu phu de render
    public List<BacSi> DanhSachBacSi { get; set; } = new();
    public BacSi? BacSiHienTai { get; set; }

    // Cac khung gio kham co dinh trong ngay (sang + chieu)
    public static readonly string[] KhungGioMau = new[]
    {
        "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30",
        "14:00", "14:30", "15:00", "15:30", "16:00", "16:30"
    };
}
