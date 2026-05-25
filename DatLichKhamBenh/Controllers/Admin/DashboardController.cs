using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Dashboard/{action=Index}/{id?}")]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var hom_nay = DateTime.Today;
        var ngay_bat_dau = hom_nay.AddDays(-6); // 7 ngay gan day (gom hom nay)

        var vm = new DashboardViewModel
        {
            TongNguoiDung = await _db.NguoiDungs.CountAsync(),
            TongBacSi = await _db.BacSis.CountAsync(),
            TongBenhNhan = await _db.BenhNhans.CountAsync(),
            TongChuyenKhoa = await _db.ChuyenKhoas.CountAsync(),
            TongLichHen = await _db.LichHens.CountAsync(),

            LichChoXacNhan = await _db.LichHens.CountAsync(l => l.TrangThai == TrangThaiLichHen.ChoXacNhan),
            LichDaXacNhan = await _db.LichHens.CountAsync(l => l.TrangThai == TrangThaiLichHen.DaXacNhan),
            LichDaKham = await _db.LichHens.CountAsync(l => l.TrangThai == TrangThaiLichHen.DaKham),
            LichDaHuy = await _db.LichHens.CountAsync(l => l.TrangThai == TrangThaiLichHen.DaHuy),
        };

        // Top 5 bac si co nhieu lich nhat
        vm.TopBacSi = await _db.BacSis
            .Select(b => new TopBacSiItem
            {
                HoTen = b.NguoiDung!.HoTen,
                ChuyenKhoa = b.ChuyenKhoa!.TenChuyenKhoa,
                SoLichHen = b.DanhSachLichHen.Count
            })
            .OrderByDescending(x => x.SoLichHen)
            .Take(5)
            .ToListAsync();

        // Lich theo ngay (7 ngay gan day, tinh theo NgayDat)
        var thongKeNgay = await _db.LichHens
            .Where(l => l.NgayDat >= ngay_bat_dau)
            .GroupBy(l => l.NgayDat.Date)
            .Select(g => new { Ngay = g.Key, So = g.Count() })
            .ToListAsync();

        for (var d = 0; d < 7; d++)
        {
            var ng = ngay_bat_dau.AddDays(d);
            vm.NgayLabels.Add(ng.ToString("dd/MM"));
            vm.LichTheoNgay.Add(thongKeNgay.FirstOrDefault(x => x.Ngay == ng)?.So ?? 0);
        }

        // 5 lich hen moi nhat
        vm.LichHenMoiNhat = await _db.LichHens
            .Include(l => l.BenhNhan).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.BacSi).ThenInclude(b => b!.NguoiDung)
            .OrderByDescending(l => l.NgayDat)
            .Take(5)
            .ToListAsync();

        return View(vm);
    }
}
