using DatLichKhamBenh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/ChuyenKhoa/{action=Index}/{id?}")]
public class AdminChuyenKhoaController : Controller
{
    private readonly AppDbContext _db;

    public AdminChuyenKhoaController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var ds = await _db.ChuyenKhoas
            .Select(c => new
            {
                c.MaChuyenKhoa,
                c.TenChuyenKhoa,
                c.MoTa,
                SoBacSi = c.DanhSachBacSi.Count
            })
            .OrderBy(c => c.TenChuyenKhoa)
            .ToListAsync();

        ViewBag.DanhSach = ds;
        return View();
    }

    [HttpGet]
    public IActionResult Tao() => View(new ChuyenKhoa());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tao(ChuyenKhoa model)
    {
        if (!ModelState.IsValid) return View(model);
        _db.ChuyenKhoas.Add(model);
        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Da them chuyen khoa '{model.TenChuyenKhoa}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var ck = await _db.ChuyenKhoas.FindAsync(id);
        if (ck is null) return NotFound();
        return View(ck);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(int id, ChuyenKhoa model)
    {
        if (id != model.MaChuyenKhoa) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        var ck = await _db.ChuyenKhoas.FindAsync(id);
        if (ck is null) return NotFound();

        ck.TenChuyenKhoa = model.TenChuyenKhoa;
        ck.MoTa = model.MoTa;
        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Da cap nhat chuyen khoa #{id}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        var ck = await _db.ChuyenKhoas
            .Include(c => c.DanhSachBacSi)
            .FirstOrDefaultAsync(c => c.MaChuyenKhoa == id);
        if (ck is null) return NotFound();

        if (ck.DanhSachBacSi.Any())
        {
            TempData["LoiThongBao"] = $"Khong the xoa chuyen khoa '{ck.TenChuyenKhoa}' vi dang co {ck.DanhSachBacSi.Count} bac si.";
            return RedirectToAction(nameof(Index));
        }

        _db.ChuyenKhoas.Remove(ck);
        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Da xoa chuyen khoa '{ck.TenChuyenKhoa}'.";
        return RedirectToAction(nameof(Index));
    }
}
