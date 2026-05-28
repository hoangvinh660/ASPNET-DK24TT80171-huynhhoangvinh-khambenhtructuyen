using DatLichKhamBenh.Models;

namespace DatLichKhamBenh.Models.ViewModels;

// View model cho trang chu: thong ke nho + danh sach chuyen khoa + bac si noi bat
public class TrangChuViewModel
{
    public int TongSoBacSi { get; set; }
    public int TongSoChuyenKhoa { get; set; }
    public int TongSoBenhNhan { get; set; }

    public List<ChuyenKhoa> ChuyenKhoaNoiBat { get; set; } = new();
    public List<BacSi> BacSiNoiBat { get; set; } = new();
}
