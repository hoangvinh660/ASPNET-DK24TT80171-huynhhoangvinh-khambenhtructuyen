using System.Globalization;
using System.Security.Claims;
using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using DatLichKhamBenh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers;

// Controller cho phep benh nhan dat lich, xem lich, huy lich.
// Bac si xu ly xac nhan/kham se o BacSiLichHenController (Buoc 6).
[Authorize(Roles = "BenhNhan")]
public class LichHenController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public LichHenController(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    // ---------- DAT LICH ----------

    // GET /LichHen/DatLich?maBacSi=5
    [HttpGet]
    public async Task<IActionResult> DatLich(int? maBacSi)
    {
        var vm = new DatLichViewModel
        {
            DanhSachBacSi = await _db.BacSis
                .Include(b => b.NguoiDung)
                .Include(b => b.ChuyenKhoa)
                .OrderBy(b => b.NguoiDung!.HoTen)
                .ToListAsync(),
            NgayKham = DateTime.Today.AddDays(1)
        };

        if (maBacSi.HasValue && maBacSi.Value > 0)
        {
            vm.MaBacSi = maBacSi.Value;
            vm.BacSiHienTai = vm.DanhSachBacSi.FirstOrDefault(b => b.MaBacSi == maBacSi.Value);
        }

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DatLich(DatLichViewModel model)
    {
        // Validate khung gio (phai nam trong DatLichViewModel.KhungGioMau)
        if (!DatLichViewModel.KhungGioMau.Contains(model.GioKhamStr))
        {
            ModelState.AddModelError(nameof(model.GioKhamStr), "Khung gio khong hop le");
        }

        // Validate ngay >= ngay mai
        if (model.NgayKham.Date < DateTime.Today.AddDays(1))
        {
            ModelState.AddModelError(nameof(model.NgayKham), "Vui long chon ngay tu ngay mai tro di");
        }

        // Parse "HH:mm" -> TimeSpan
        TimeSpan gioKham = default;
        if (ModelState.IsValid &&
            !TimeSpan.TryParseExact(model.GioKhamStr, @"hh\:mm", CultureInfo.InvariantCulture, out gioKham))
        {
            ModelState.AddModelError(nameof(model.GioKhamStr), "Khong doc duoc gio kham");
        }

        // Kiem tra bac si ton tai
        var bacSi = await _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.ChuyenKhoa)
            .FirstOrDefaultAsync(b => b.MaBacSi == model.MaBacSi);
        if (bacSi is null)
        {
            ModelState.AddModelError(nameof(model.MaBacSi), "Bac si khong ton tai");
        }

        // Trung khung gio (bo qua lich da huy)
        if (ModelState.IsValid && bacSi is not null)
        {
            bool trung = await _db.LichHens.AnyAsync(l =>
                l.MaBacSi == bacSi.MaBacSi &&
                l.NgayKham == model.NgayKham.Date &&
                l.GioKham == gioKham &&
                l.TrangThai != TrangThaiLichHen.DaHuy);
            if (trung)
            {
                ModelState.AddModelError(nameof(model.GioKhamStr), "Khung gio nay da co lich, vui long chon khung gio khac");
            }
        }

        if (!ModelState.IsValid)
        {
            model.DanhSachBacSi = await _db.BacSis
                .Include(b => b.NguoiDung)
                .Include(b => b.ChuyenKhoa)
                .OrderBy(b => b.NguoiDung!.HoTen)
                .ToListAsync();
            model.BacSiHienTai = model.DanhSachBacSi.FirstOrDefault(b => b.MaBacSi == model.MaBacSi);
            return View(model);
        }

        // Tim BenhNhan theo MaNguoiDung trong claim
        int maNguoiDung = LayMaNguoiDungHienTai();
        var benhNhan = await _db.BenhNhans.FirstOrDefaultAsync(b => b.MaNguoiDung == maNguoiDung);
        if (benhNhan is null)
        {
            TempData["LoiThongBao"] = "Tai khoan cua ban chua duoc tao ho so benh nhan, vui long lien he quan tri vien.";
            return RedirectToAction(nameof(LichHenCuaToi));
        }

        var lichHen = new LichHen
        {
            MaBenhNhan = benhNhan.MaBenhNhan,
            MaBacSi = model.MaBacSi,
            NgayKham = model.NgayKham.Date,
            GioKham = gioKham,
            LyDoKham = model.LyDoKham,
            TrangThai = TrangThaiLichHen.ChoXacNhan,
            NgayDat = DateTime.Now
        };
        _db.LichHens.Add(lichHen);
        await _db.SaveChangesAsync();

        // Gui mail "da nhan lich, cho xac nhan" cho benh nhan.
        // Nap day du navigation truoc khi render template.
        await _db.Entry(lichHen).Reference(l => l.BenhNhan).LoadAsync();
        await _db.Entry(lichHen.BenhNhan!).Reference(b => b.NguoiDung).LoadAsync();
        lichHen.BacSi = bacSi;
        await _email.GuiMailDatLichAsync(lichHen);

        TempData["ThongBao"] = $"Dat lich thanh cong! Lich hen #{lichHen.MaLichHen} dang cho bac si xac nhan.";
        return RedirectToAction(nameof(LichHenCuaToi));
    }

    // ---------- DANH SACH LICH HEN CUA BENH NHAN ----------

    // GET /LichHen/LichHenCuaToi
    public async Task<IActionResult> LichHenCuaToi()
    {
        int maNguoiDung = LayMaNguoiDungHienTai();
        var benhNhan = await _db.BenhNhans.FirstOrDefaultAsync(b => b.MaNguoiDung == maNguoiDung);
        if (benhNhan is null)
        {
            return View(new List<LichHen>());
        }

        var ds = await _db.LichHens
            .Include(l => l.BacSi).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.BacSi).ThenInclude(b => b!.ChuyenKhoa)
            .Include(l => l.HoSoBenhAn)
            .Where(l => l.MaBenhNhan == benhNhan.MaBenhNhan)
            .OrderByDescending(l => l.NgayKham)
            .ThenByDescending(l => l.GioKham)
            .ToListAsync();

        return View(ds);
    }

    // ---------- HUY LICH ----------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Huy(int id)
    {
        int maNguoiDung = LayMaNguoiDungHienTai();
        var benhNhan = await _db.BenhNhans.FirstOrDefaultAsync(b => b.MaNguoiDung == maNguoiDung);
        if (benhNhan is null)
        {
            return Forbid();
        }

        var lichHen = await _db.LichHens.FirstOrDefaultAsync(l => l.MaLichHen == id);
        if (lichHen is null)
        {
            return NotFound();
        }

        // Chi cho phep huy lich cua chinh minh
        if (lichHen.MaBenhNhan != benhNhan.MaBenhNhan)
        {
            return Forbid();
        }

        // Chi cho phep huy khi chua kham
        if (lichHen.TrangThai is TrangThaiLichHen.DaKham or TrangThaiLichHen.DaHuy)
        {
            TempData["LoiThongBao"] = "Khong the huy lich hen nay (da kham hoac da bi huy truoc do).";
            return RedirectToAction(nameof(LichHenCuaToi));
        }

        lichHen.TrangThai = TrangThaiLichHen.DaHuy;
        await _db.SaveChangesAsync();

        // Nap navigation va gui mail huy lich cho chinh benh nhan biet.
        await _db.Entry(lichHen).Reference(l => l.BenhNhan).LoadAsync();
        await _db.Entry(lichHen.BenhNhan!).Reference(b => b.NguoiDung).LoadAsync();
        await _db.Entry(lichHen).Reference(l => l.BacSi).LoadAsync();
        await _db.Entry(lichHen.BacSi!).Reference(b => b.NguoiDung).LoadAsync();
        await _email.GuiMailHuyLichAsync(lichHen, "Benh nhan tu huy");

        TempData["ThongBao"] = $"Da huy lich hen #{lichHen.MaLichHen}.";
        return RedirectToAction(nameof(LichHenCuaToi));
    }

    // ---------- HELPER ----------

    private int LayMaNguoiDungHienTai()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) ? id : 0;
    }
}
