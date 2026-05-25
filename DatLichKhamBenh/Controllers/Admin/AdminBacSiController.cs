using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/BacSi/{action=Index}/{id?}")]
public class AdminBacSiController : Controller
{
    private readonly AppDbContext _db;

    public AdminBacSiController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var ds = await _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.ChuyenKhoa)
            .OrderBy(b => b.NguoiDung!.HoTen)
            .ToListAsync();
        return View(ds);
    }

    [HttpGet]
    public async Task<IActionResult> Tao()
    {
        await NapDsChuyenKhoaAsync();
        return View(new BacSiAdminViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tao(BacSiAdminViewModel model)
    {
        await NapDsChuyenKhoaAsync(model.MaChuyenKhoa);

        if (string.IsNullOrWhiteSpace(model.MatKhau))
        {
            ModelState.AddModelError(nameof(model.MatKhau), "Vui long nhap mat khau cho tai khoan bac si.");
        }

        // Check trung ten dang nhap / email
        if (await _db.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Ten dang nhap da ton tai.");
        }
        if (await _db.NguoiDungs.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email da ton tai.");
        }

        if (!ModelState.IsValid) return View(model);

        var nguoiDung = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap.Trim(),
            MatKhau = model.MatKhau!,
            HoTen = model.HoTen.Trim(),
            Email = model.Email.Trim(),
            SoDienThoai = model.SoDienThoai,
            VaiTro = "BacSi",
            NgayTao = DateTime.Now
        };
        _db.NguoiDungs.Add(nguoiDung);
        await _db.SaveChangesAsync();

        var bacSi = new BacSi
        {
            MaNguoiDung = nguoiDung.MaNguoiDung,
            MaChuyenKhoa = model.MaChuyenKhoa,
            HocVi = model.HocVi,
            KinhNghiem = model.KinhNghiem,
            MoTa = model.MoTa,
            HinhAnh = model.HinhAnh,
            GiaKham = model.GiaKham
        };
        _db.BacSis.Add(bacSi);
        await _db.SaveChangesAsync();

        TempData["ThongBao"] = $"Da them bac si '{model.HoTen}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Sua(int id)
    {
        var bs = await _db.BacSis
            .Include(b => b.NguoiDung)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);
        if (bs is null) return NotFound();

        await NapDsChuyenKhoaAsync(bs.MaChuyenKhoa);

        var vm = new BacSiAdminViewModel
        {
            MaBacSi = bs.MaBacSi,
            MaNguoiDung = bs.MaNguoiDung,
            TenDangNhap = bs.NguoiDung!.TenDangNhap,
            HoTen = bs.NguoiDung.HoTen,
            Email = bs.NguoiDung.Email,
            SoDienThoai = bs.NguoiDung.SoDienThoai,
            MaChuyenKhoa = bs.MaChuyenKhoa,
            HocVi = bs.HocVi,
            KinhNghiem = bs.KinhNghiem,
            MoTa = bs.MoTa,
            HinhAnh = bs.HinhAnh,
            GiaKham = bs.GiaKham
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Sua(int id, BacSiAdminViewModel model)
    {
        if (id != model.MaBacSi) return BadRequest();
        await NapDsChuyenKhoaAsync(model.MaChuyenKhoa);

        var bs = await _db.BacSis
            .Include(b => b.NguoiDung)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);
        if (bs is null) return NotFound();

        // Khi sua: cho doi email/ten dang nhap nhung phai unique voi nguoi khac
        if (await _db.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap && u.MaNguoiDung != bs.MaNguoiDung))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Ten dang nhap da ton tai.");
        }
        if (await _db.NguoiDungs.AnyAsync(u => u.Email == model.Email && u.MaNguoiDung != bs.MaNguoiDung))
        {
            ModelState.AddModelError(nameof(model.Email), "Email da ton tai.");
        }

        if (!ModelState.IsValid) return View(model);

        bs.NguoiDung!.TenDangNhap = model.TenDangNhap.Trim();
        bs.NguoiDung.HoTen = model.HoTen.Trim();
        bs.NguoiDung.Email = model.Email.Trim();
        bs.NguoiDung.SoDienThoai = model.SoDienThoai;
        if (!string.IsNullOrWhiteSpace(model.MatKhau))
        {
            bs.NguoiDung.MatKhau = model.MatKhau;
        }

        bs.MaChuyenKhoa = model.MaChuyenKhoa;
        bs.HocVi = model.HocVi;
        bs.KinhNghiem = model.KinhNghiem;
        bs.MoTa = model.MoTa;
        bs.HinhAnh = model.HinhAnh;
        bs.GiaKham = model.GiaKham;

        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Da cap nhat bac si '{model.HoTen}'.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Xoa(int id)
    {
        var bs = await _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.DanhSachLichHen)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);
        if (bs is null) return NotFound();

        if (bs.DanhSachLichHen.Any())
        {
            TempData["LoiThongBao"] = $"Khong the xoa bac si '{bs.NguoiDung!.HoTen}' vi dang co {bs.DanhSachLichHen.Count} lich hen.";
            return RedirectToAction(nameof(Index));
        }

        // Xoa BacSi truoc, sau do xoa NguoiDung lien quan
        _db.BacSis.Remove(bs);
        if (bs.NguoiDung is not null)
        {
            _db.NguoiDungs.Remove(bs.NguoiDung);
        }
        await _db.SaveChangesAsync();

        TempData["ThongBao"] = "Da xoa bac si va tai khoan lien quan.";
        return RedirectToAction(nameof(Index));
    }

    private async Task NapDsChuyenKhoaAsync(int? selected = null)
    {
        var ds = await _db.ChuyenKhoas.OrderBy(c => c.TenChuyenKhoa).ToListAsync();
        ViewBag.DsChuyenKhoa = new SelectList(ds, "MaChuyenKhoa", "TenChuyenKhoa", selected);
    }
}
