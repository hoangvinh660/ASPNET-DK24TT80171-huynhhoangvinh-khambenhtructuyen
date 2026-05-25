namespace DatLichKhamBenh.Models.ViewModels;

// View model cho trang Admin Dashboard
public class DashboardViewModel
{
    public int TongNguoiDung { get; set; }
    public int TongBacSi { get; set; }
    public int TongBenhNhan { get; set; }
    public int TongChuyenKhoa { get; set; }
    public int TongLichHen { get; set; }

    public int LichChoXacNhan { get; set; }
    public int LichDaXacNhan { get; set; }
    public int LichDaKham { get; set; }
    public int LichDaHuy { get; set; }

    public List<TopBacSiItem> TopBacSi { get; set; } = new();

    // Du lieu cho bieu do "Lich hen 7 ngay gan day"
    public List<string> NgayLabels { get; set; } = new();
    public List<int> LichTheoNgay { get; set; } = new();

    // Lich hen moi nhat (5 record)
    public List<LichHen> LichHenMoiNhat { get; set; } = new();
}

public class TopBacSiItem
{
    public string HoTen { get; set; } = string.Empty;
    public string ChuyenKhoa { get; set; } = string.Empty;
    public int SoLichHen { get; set; }
}
