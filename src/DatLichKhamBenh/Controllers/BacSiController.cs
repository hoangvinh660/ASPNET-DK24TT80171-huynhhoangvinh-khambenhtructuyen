using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers;

// Controller liet ke + tim kiem + xem chi tiet bac si (public)
public class BacSiController : Controller
{
    private readonly AppDbContext _db;

    public BacSiController(AppDbContext db)
    {
        _db = db;
    }

    // GET /BacSi?tuKhoa=...&maChuyenKhoa=...
    public async Task<IActionResult> Index(string? tuKhoa, int? maChuyenKhoa)
    {
        var query = _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.ChuyenKhoa)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            tuKhoa = tuKhoa.Trim();
            query = query.Where(b =>
                b.NguoiDung!.HoTen.Contains(tuKhoa) ||
                b.HocVi.Contains(tuKhoa) ||
                (b.MoTa != null && b.MoTa.Contains(tuKhoa)));
        }

        if (maChuyenKhoa.HasValue && maChuyenKhoa.Value > 0)
        {
            query = query.Where(b => b.MaChuyenKhoa == maChuyenKhoa.Value);
        }

        var vm = new TimKiemBacSiViewModel
        {
            TuKhoa = tuKhoa,
            MaChuyenKhoa = maChuyenKhoa,
            DanhSachChuyenKhoa = await _db.ChuyenKhoas.OrderBy(c => c.TenChuyenKhoa).ToListAsync(),
            DanhSachBacSi = await query
                .OrderBy(b => b.NguoiDung!.HoTen)
                .ToListAsync()
        };

        return View(vm);
    }

    // GET /BacSi/ChiTiet/5
    public async Task<IActionResult> ChiTiet(int id)
    {
        var bs = await _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.ChuyenKhoa)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);

        if (bs is null)
        {
            return NotFound();
        }

        return View(bs);
    }
}
