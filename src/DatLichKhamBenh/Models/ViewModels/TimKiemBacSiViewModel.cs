using System.ComponentModel.DataAnnotations;
using DatLichKhamBenh.Models;

namespace DatLichKhamBenh.Models.ViewModels;

// View model trang danh sach/tim kiem bac si
public class TimKiemBacSiViewModel
{
    [Display(Name = "Tu khoa")]
    public string? TuKhoa { get; set; }

    [Display(Name = "Chuyen khoa")]
    public int? MaChuyenKhoa { get; set; }

    public List<ChuyenKhoa> DanhSachChuyenKhoa { get; set; } = new();
    public List<BacSi> DanhSachBacSi { get; set; } = new();
}
