using DatLichKhamBenh.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers;

// Controller liet ke + xem chi tiet chuyen khoa (public)
public class ChuyenKhoaController : Controller
{
    private readonly AppDbContext _db;

    public ChuyenKhoaController(AppDbContext db)
    {
        _db = db;
    }

    // GET /ChuyenKhoa
    public async Task<IActionResult> Index()
    {
        var ds = await _db.ChuyenKhoas
            .Include(ck => ck.DanhSachBacSi)
            .OrderBy(ck => ck.TenChuyenKhoa)
            .ToListAsync();
        return View(ds);
    }

    // GET /ChuyenKhoa/ChiTiet/5
    public async Task<IActionResult> ChiTiet(int id)
    {
        var ck = await _db.ChuyenKhoas
            .Include(c => c.DanhSachBacSi).ThenInclude(b => b.NguoiDung)
            .FirstOrDefaultAsync(c => c.MaChuyenKhoa == id);

        if (ck is null)
        {
            return NotFound();
        }

        return View(ck);
    }
}
