using DatLichKhamBenh.Models;
using DatLichKhamBenh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/LichHen/{action=Index}/{id?}")]
public class AdminLichHenController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public AdminLichHenController(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task<IActionResult> Index(string? trangThai, int? maBacSi, DateTime? tuNgay, DateTime? denNgay)
    {
        var q = _db.LichHens
            .Include(l => l.BacSi).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.BacSi).ThenInclude(b => b!.ChuyenKhoa)
            .Include(l => l.BenhNhan).ThenInclude(b => b!.NguoiDung)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(trangThai))
            q = q.Where(l => l.TrangThai == trangThai);
        if (maBacSi.HasValue)
            q = q.Where(l => l.MaBacSi == maBacSi.Value);
        if (tuNgay.HasValue)
            q = q.Where(l => l.NgayKham >= tuNgay.Value);
        if (denNgay.HasValue)
            q = q.Where(l => l.NgayKham <= denNgay.Value);

        var ds = await q
            .OrderByDescending(l => l.NgayKham)
            .ThenByDescending(l => l.GioKham)
            .ToListAsync();

        ViewBag.TrangThai = trangThai;
        ViewBag.MaBacSi = maBacSi;
        ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
        ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");
        ViewBag.DsBacSi = await _db.BacSis
            .Include(b => b.NguoiDung)
            .OrderBy(b => b.NguoiDung!.HoTen)
            .Select(b => new { b.MaBacSi, HoTen = b.NguoiDung!.HoTen })
            .ToListAsync();

        return View(ds);
    }

    // Admin co the huy bat ky lich hen nao (de xu ly truong hop dac biet)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        var l = await _db.LichHens.FindAsync(id);
        if (l is null) return NotFound();
        if (l.TrangThai == TrangThaiLichHen.DaKham)
        {
            TempData["LoiThongBao"] = "Khong the huy lich da kham.";
            return RedirectToAction(nameof(Index));
        }
        l.TrangThai = TrangThaiLichHen.DaHuy;
        await _db.SaveChangesAsync();

        var lichDayDu = await _db.LichHens
            .Include(x => x.BenhNhan).ThenInclude(b => b!.NguoiDung)
            .Include(x => x.BacSi).ThenInclude(b => b!.NguoiDung)
            .FirstAsync(x => x.MaLichHen == id);
        await _email.GuiMailHuyLichAsync(lichDayDu, "Quan tri vien huy lich");

        TempData["ThongBao"] = $"Da huy lich hen #{id}.";
        return RedirectToAction(nameof(Index));
    }
}
