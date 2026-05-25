using System.Diagnostics;
using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers;

// Controller cho trang chu va cac trang chung (Privacy, Error)
public class TrangChuController : Controller
{
    private readonly AppDbContext _db;

    public TrangChuController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var vm = new TrangChuViewModel
        {
            TongSoBacSi = await _db.BacSis.CountAsync(),
            TongSoChuyenKhoa = await _db.ChuyenKhoas.CountAsync(),
            TongSoBenhNhan = await _db.BenhNhans.CountAsync(),
            ChuyenKhoaNoiBat = await _db.ChuyenKhoas
                .Include(ck => ck.DanhSachBacSi)
                .OrderBy(ck => ck.TenChuyenKhoa)
                .Take(6)
                .ToListAsync(),
            BacSiNoiBat = await _db.BacSis
                .Include(b => b.NguoiDung)
                .Include(b => b.ChuyenKhoa)
                .OrderBy(b => b.GiaKham)
                .Take(6)
                .ToListAsync()
        };
        return View(vm);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
