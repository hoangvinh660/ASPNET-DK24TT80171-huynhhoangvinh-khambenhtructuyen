using System.Security.Claims;
using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers;

// Controller xu ly dang ky / dang nhap / dang xuat
public class TaiKhoanController : Controller
{
    private readonly AppDbContext _db;

    public TaiKhoanController(AppDbContext db)
    {
        _db = db;
    }

    // ---------- DANG NHAP ----------

    [HttpGet]
    public IActionResult DangNhap(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new DangNhapViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangNhap(DangNhapViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _db.NguoiDungs
            .FirstOrDefaultAsync(u => u.TenDangNhap == model.TenDangNhap);

        if (user is null || user.MatKhau != model.MatKhau)
        {
            ModelState.AddModelError(string.Empty, "Ten dang nhap hoac mat khau khong dung");
            return View(model);
        }

        if (user.DaKhoa)
        {
            ModelState.AddModelError(string.Empty, "Tai khoan da bi khoa, vui long lien he quan tri vien");
            return View(model);
        }

        await SignInUserAsync(user, model.GhiNho);

        // Chuyen huong: ReturnUrl (neu cookie auth da gan) hoac trang chu / khu vuc tuong ung
        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        // Sau khi dang nhap thanh cong: dieu huong ve Home.
        // Buoc 7: doi case Admin -> RedirectToAction("Index", "Dashboard") khi co Admin controller.
        return RedirectToAction("Index", "Home");
    }

    // ---------- DANG KY (Benh nhan tu dang ky) ----------

    [HttpGet]
    public IActionResult DangKy()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new DangKyViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DangKy(DangKyViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (await _db.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Ten dang nhap da ton tai");
            return View(model);
        }

        if (await _db.NguoiDungs.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email da duoc su dung");
            return View(model);
        }

        var nguoiDung = new NguoiDung
        {
            TenDangNhap = model.TenDangNhap,
            MatKhau = model.MatKhau,
            HoTen = model.HoTen,
            Email = model.Email,
            SoDienThoai = model.SoDienThoai,
            VaiTro = "BenhNhan",
            NgayTao = DateTime.Now
        };

        var benhNhan = new BenhNhan
        {
            NguoiDung = nguoiDung,
            NgaySinh = model.NgaySinh,
            GioiTinh = model.GioiTinh,
            DiaChi = model.DiaChi
        };

        _db.BenhNhans.Add(benhNhan);
        await _db.SaveChangesAsync();

        // Tu dong dang nhap sau khi dang ky thanh cong
        await SignInUserAsync(nguoiDung, isPersistent: false);

        TempData["ThongBao"] = "Dang ky thanh cong! Chao mung ban den voi he thong.";
        return RedirectToAction("Index", "Home");
    }

    // ---------- DANG XUAT ----------

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> DangXuat()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }

    // ---------- TU CHOI (403) ----------

    [HttpGet]
    public IActionResult TuChoi() => View();

    // ---------- HELPER ----------

    private async Task SignInUserAsync(NguoiDung user, bool isPersistent)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.MaNguoiDung.ToString()),
            new(ClaimTypes.Name, user.TenDangNhap),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.VaiTro),
            new("HoTen", user.HoTen)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = isPersistent,
            ExpiresUtc = isPersistent
                ? DateTimeOffset.UtcNow.AddDays(7)
                : DateTimeOffset.UtcNow.AddHours(2)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            authProps);

        // Luu them vao Session de View truy cap nhanh
        HttpContext.Session.SetInt32("MaNguoiDung", user.MaNguoiDung);
        HttpContext.Session.SetString("HoTen", user.HoTen);
        HttpContext.Session.SetString("VaiTro", user.VaiTro);
    }
}
