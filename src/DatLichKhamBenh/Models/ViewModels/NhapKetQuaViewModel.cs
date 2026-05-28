using System.ComponentModel.DataAnnotations;
using DatLichKhamBenh.Models;

namespace DatLichKhamBenh.Models.ViewModels;

// View model cho form bac si nhap ket qua kham
public class NhapKetQuaViewModel
{
    [Required]
    public int MaLichHen { get; set; }

    [Required(ErrorMessage = "Vui long nhap chan doan")]
    [StringLength(1000)]
    [Display(Name = "Chan doan")]
    public string ChanDoan { get; set; } = string.Empty;

    [StringLength(2000)]
    [Display(Name = "Don thuoc")]
    public string? DonThuoc { get; set; }

    [StringLength(2000)]
    [Display(Name = "Loi khuyen")]
    public string? LoiKhuyen { get; set; }

    // Thong tin lich hen de hien thi (read-only) tren form
    public LichHen? LichHen { get; set; }
}
