using System.Security.Claims;
using DatLichKhamBenh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/NguoiDung/{action=Index}/{id?}")]
public class AdminNguoiDungController : Controller
{
    private readonly AppDbContext _db;

    public AdminNguoiDungController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(string? vaiTro, string? tuKhoa)
    {
        var q = _db.NguoiDungs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(vaiTro) && new[] { "Admin", "BacSi", "BenhNhan" }.Contains(vaiTro))
        {
            q = q.Where(u => u.VaiTro == vaiTro);
        }

        if (!string.IsNullOrWhiteSpace(tuKhoa))
        {
            var tk = tuKhoa.Trim();
            q = q.Where(u =>
                u.TenDangNhap.Contains(tk) ||
                u.HoTen.Contains(tk) ||
                u.Email.Contains(tk));
        }

        var ds = await q.OrderByDescending(u => u.NgayTao).ToListAsync();

        ViewBag.VaiTro = vaiTro;
        ViewBag.TuKhoa = tuKhoa;
        return View(ds);
    }

    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var u = await _db.NguoiDungs.FindAsync(id);
        if (u is null) return NotFound();
        return View(u);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(int id, NguoiDung model, string? matKhauMoi)
    {
        if (id != model.MaNguoiDung) return BadRequest();

        var u = await _db.NguoiDungs.FindAsync(id);
        if (u is null) return NotFound();

        // Check trung email/ten dang nhap voi nguoi khac
        if (await _db.NguoiDungs.AnyAsync(x => x.TenDangNhap == model.TenDangNhap && x.MaNguoiDung != id))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Ten dang nhap da ton tai.");
        }
        if (await _db.NguoiDungs.AnyAsync(x => x.Email == model.Email && x.MaNguoiDung != id))
        {
            ModelState.AddModelError(nameof(model.Email), "Email da ton tai.");
        }

        // Khong cho doi vai tro cua chinh minh hoac admin cuoi cung
        if (u.VaiTro == "Admin" && model.VaiTro != "Admin")
        {
            var conAdminKhac = await _db.NguoiDungs.AnyAsync(x => x.VaiTro == "Admin" && x.MaNguoiDung != id);
            if (!conAdminKhac)
            {
                ModelState.AddModelError(nameof(model.VaiTro), "Khong the doi vai tro cua admin cuoi cung.");
            }
        }

        // Khong cho doi vai tro cua BacSi/BenhNhan da co ho so (de tranh data inconsistent)
        if (model.VaiTro != u.VaiTro)
        {
            if (u.VaiTro == "BacSi" && await _db.BacSis.AnyAsync(b => b.MaNguoiDung == id))
            {
                ModelState.AddModelError(nameof(model.VaiTro), "Tai khoan da gan ho so bac si, khong the doi vai tro.");
            }
            else if (u.VaiTro == "BenhNhan" && await _db.BenhNhans.AnyAsync(b => b.MaNguoiDung == id))
            {
                ModelState.AddModelError(nameof(model.VaiTro), "Tai khoan da gan ho so benh nhan, khong the doi vai tro.");
            }
        }

        // Loai bot validation cho MatKhau (vi form khong gui mat khau cu)
        ModelState.Remove(nameof(model.MatKhau));

        if (!ModelState.IsValid) return View(u);

        u.TenDangNhap = model.TenDangNhap.Trim();
        u.HoTen = model.HoTen.Trim();
        u.Email = model.Email.Trim();
        u.SoDienThoai = model.SoDienThoai;
        u.VaiTro = model.VaiTro;
        u.DaKhoa = model.DaKhoa;
        if (!string.IsNullOrWhiteSpace(matKhauMoi))
        {
            u.MatKhau = matKhauMoi.Trim();
        }

        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Da cap nhat tai khoan '{u.TenDangNhap}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DoiTrangThaiKhoa(int id)
    {
        var u = await _db.NguoiDungs.FindAsync(id);
        if (u is null) return NotFound();

        // Khong cho khoa chinh minh
        var maHienTai = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        if (u.MaNguoiDung == maHienTai)
        {
            TempData["LoiThongBao"] = "Khong the khoa tai khoan dang dang nhap.";
            return RedirectToAction(nameof(Index));
        }

        u.DaKhoa = !u.DaKhoa;
        await _db.SaveChangesAsync();
        TempData["ThongBao"] = u.DaKhoa
            ? $"Da khoa tai khoan '{u.TenDangNhap}'."
            : $"Da mo khoa tai khoan '{u.TenDangNhap}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        var u = await _db.NguoiDungs
            .Include(x => x.BacSi).ThenInclude(b => b!.DanhSachLichHen)
            .Include(x => x.BenhNhan).ThenInclude(b => b!.DanhSachLichHen)
            .FirstOrDefaultAsync(x => x.MaNguoiDung == id);
        if (u is null) return NotFound();

        var maHienTai = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        if (u.MaNguoiDung == maHienTai)
        {
            TempData["LoiThongBao"] = "Khong the tu xoa tai khoan dang dang nhap.";
            return RedirectToAction(nameof(Index));
        }

        if (u.VaiTro == "Admin")
        {
            var conAdminKhac = await _db.NguoiDungs.AnyAsync(x => x.VaiTro == "Admin" && x.MaNguoiDung != id);
            if (!conAdminKhac)
            {
                TempData["LoiThongBao"] = "Khong the xoa admin cuoi cung.";
                return RedirectToAction(nameof(Index));
            }
        }

        // Khong cho xoa neu da co lich hen
        if (u.BacSi?.DanhSachLichHen.Any() == true)
        {
            TempData["LoiThongBao"] = $"Khong the xoa: bac si nay co {u.BacSi.DanhSachLichHen.Count} lich hen.";
            return RedirectToAction(nameof(Index));
        }
        if (u.BenhNhan?.DanhSachLichHen.Any() == true)
        {
            TempData["LoiThongBao"] = $"Khong the xoa: benh nhan nay co {u.BenhNhan.DanhSachLichHen.Count} lich hen.";
            return RedirectToAction(nameof(Index));
        }

        // Cascade tu NguoiDung -> BacSi/BenhNhan (theo cau hinh trong DbContext)
        _db.NguoiDungs.Remove(u);
        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Da xoa tai khoan '{u.TenDangNhap}'.";
        return RedirectToAction(nameof(Index));
    }
}
