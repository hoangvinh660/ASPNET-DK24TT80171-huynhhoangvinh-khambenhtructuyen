using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using DatLichKhamBenh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/BacSi/{action=Index}/{id?}")]
public class AdminBacSiController : Controller
{
    private readonly AppDbContext _db;
    private readonly IBacSiImageService _anhService;
    private readonly BacSiUploadSettings _uploadOpt;

    public AdminBacSiController(
        AppDbContext db,
        IBacSiImageService anhService,
        IOptions<BacSiUploadSettings> uploadOpt)
    {
        _db = db;
        _anhService = anhService;
        _uploadOpt = uploadOpt.Value;
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
        return View(new BacSiAdminViewModel { HinhAnh = _uploadOpt.AnhMacDinh });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Tao(BacSiAdminViewModel model)
    {
        await NapDsChuyenKhoaAsync(model.MaChuyenKhoa);
        ModelState.Remove(nameof(model.AnhDaiDien));

        if (string.IsNullOrWhiteSpace(model.MatKhau))
        {
            ModelState.AddModelError(nameof(model.MatKhau), "Vui lòng nhập mật khẩu cho tài khoản bác sĩ.");
        }

        if (await _db.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
        }
        if (await _db.NguoiDungs.AnyAsync(u => u.Email == model.Email))
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại.");
        }

        var hinhAnh = await XuLyAnhAsync(model, null);
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
            HinhAnh = hinhAnh ?? _uploadOpt.AnhMacDinh,
            GiaKham = model.GiaKham
        };
        _db.BacSis.Add(bacSi);
        await _db.SaveChangesAsync();

        TempData["ThongBao"] = $"Đã thêm bác sĩ '{model.HoTen}'.";
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
            HinhAnh = string.IsNullOrWhiteSpace(bs.HinhAnh) ? _uploadOpt.AnhMacDinh : bs.HinhAnh,
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
        ModelState.Remove(nameof(model.AnhDaiDien));

        var bs = await _db.BacSis
            .Include(b => b.NguoiDung)
            .FirstOrDefaultAsync(b => b.MaBacSi == id);
        if (bs is null) return NotFound();

        if (await _db.NguoiDungs.AnyAsync(u => u.TenDangNhap == model.TenDangNhap && u.MaNguoiDung != bs.MaNguoiDung))
        {
            ModelState.AddModelError(nameof(model.TenDangNhap), "Tên đăng nhập đã tồn tại.");
        }
        if (await _db.NguoiDungs.AnyAsync(u => u.Email == model.Email && u.MaNguoiDung != bs.MaNguoiDung))
        {
            ModelState.AddModelError(nameof(model.Email), "Email đã tồn tại.");
        }

        var hinhAnh = await XuLyAnhAsync(model, bs.HinhAnh);
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
        if (hinhAnh is not null)
        {
            bs.HinhAnh = hinhAnh;
        }
        bs.GiaKham = model.GiaKham;

        await _db.SaveChangesAsync();
        TempData["ThongBao"] = $"Đã cập nhật bác sĩ '{model.HoTen}'.";
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
            TempData["LoiThongBao"] = $"Không thể xóa bác sĩ '{bs.NguoiDung!.HoTen}' vì đang có {bs.DanhSachLichHen.Count} lịch hẹn.";
            return RedirectToAction(nameof(Index));
        }

        _anhService.XoaAnh(bs.HinhAnh);

        _db.BacSis.Remove(bs);
        if (bs.NguoiDung is not null)
        {
            _db.NguoiDungs.Remove(bs.NguoiDung);
        }
        await _db.SaveChangesAsync();

        TempData["ThongBao"] = "Đã xóa bác sĩ và tài khoản liên quan.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Xu ly upload anh; tra ve null neu giu nguyen anh cu</summary>
    private async Task<string?> XuLyAnhAsync(BacSiAdminViewModel model, string? anhCu)
    {
        if (model.AnhDaiDien is { Length: > 0 })
        {
            var (duongDan, loi) = await _anhService.LuuAnhAsync(model.AnhDaiDien, anhCu);
            if (loi is not null)
            {
                ModelState.AddModelError(nameof(model.AnhDaiDien), loi);
                return anhCu;
            }
            return duongDan;
        }

        return anhCu;
    }

    private async Task NapDsChuyenKhoaAsync(int? selected = null)
    {
        var ds = await _db.ChuyenKhoas.OrderBy(c => c.TenChuyenKhoa).ToListAsync();
        ViewBag.DsChuyenKhoa = new SelectList(ds, "MaChuyenKhoa", "TenChuyenKhoa", selected);
        ViewBag.UploadMb = _uploadOpt.KichThuocToiDaMb;
        ViewBag.DinhDangAnh = string.Join(", ", _uploadOpt.DinhDangChoPhep);
    }
}
