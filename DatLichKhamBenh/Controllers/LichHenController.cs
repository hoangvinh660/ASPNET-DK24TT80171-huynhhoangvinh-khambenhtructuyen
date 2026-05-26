using System.Globalization;
using System.Security.Claims;
using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using DatLichKhamBenh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

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
        // Validate khung giờ (phải nằm trong DatLichViewModel.KhungGioMau)
        if (!DatLichViewModel.KhungGioMau.Contains(model.GioKhamStr))
        {
            ModelState.AddModelError(nameof(model.GioKhamStr), "Khung giờ không hợp lệ");
        }

        // Validate ngay >= ngay mai
        if (model.NgayKham.Date < DateTime.Today.AddDays(1))
        {
            ModelState.AddModelError(nameof(model.NgayKham), "Vui lòng chọn ngày từ ngày mai trở đi");
        }

        // Parse "HH:mm" -> TimeSpan
        TimeSpan gioKham = default;
        if (ModelState.IsValid &&
            !TimeSpan.TryParseExact(model.GioKhamStr, @"hh\:mm", CultureInfo.InvariantCulture, out gioKham))
        {
            ModelState.AddModelError(nameof(model.GioKhamStr), "Không đọc được giờ khám");
        }

        // Kiem tra bac si ton tai
        var bacSi = await _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.ChuyenKhoa)
            .FirstOrDefaultAsync(b => b.MaBacSi == model.MaBacSi);
        if (bacSi is null)
        {
            ModelState.AddModelError(nameof(model.MaBacSi), "Bác sĩ không tồn tại");
        }

        var ngayKham = model.NgayKham.Date;

        // Trung khung gio (bo qua lich da huy)
        if (ModelState.IsValid && bacSi is not null)
        {
            if (await KhungGioDaDuocDatAsync(bacSi.MaBacSi, ngayKham, gioKham))
            {
                ModelState.AddModelError(nameof(model.GioKhamStr),
                    "Khung giờ này đã có lịch, vui lòng chọn khung giờ khác");
            }
        }

        if (!ModelState.IsValid)
        {
            return await TraVeFormDatLichAsync(model);
        }

        // Tim BenhNhan theo MaNguoiDung trong claim
        int maNguoiDung = LayMaNguoiDungHienTai();
        var benhNhan = await _db.BenhNhans.FirstOrDefaultAsync(b => b.MaNguoiDung == maNguoiDung);
        if (benhNhan is null)
        {
            TempData["LoiThongBao"] = "Tài khoản của bạn chưa được tạo hồ sơ bệnh nhân, vui lòng liên hệ quản trị viên.";
            return RedirectToAction(nameof(LichHenCuaToi));
        }

        var lichHen = new LichHen
        {
            MaBenhNhan = benhNhan.MaBenhNhan,
            MaBacSi = model.MaBacSi,
            NgayKham = ngayKham,
            GioKham = gioKham,
            LyDoKham = model.LyDoKham,
            TrangThai = TrangThaiLichHen.ChoXacNhan,
            NgayDat = DateTime.Now
        };
        _db.LichHens.Add(lichHen);

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (LaLoiTrungKhungGio(ex))
        {
            ModelState.AddModelError(nameof(model.GioKhamStr),
                "Khung giờ này vừa được người khác đặt, vui lòng chọn khung giờ khác");
            return await TraVeFormDatLichAsync(model);
        }

        // Gui mail "da nhan lich, cho xac nhan" cho benh nhan.
        // Nap day du navigation truoc khi render template.
        await _db.Entry(lichHen).Reference(l => l.BenhNhan).LoadAsync();
        await _db.Entry(lichHen.BenhNhan!).Reference(b => b.NguoiDung).LoadAsync();
        lichHen.BacSi = bacSi;
        await _email.GuiMailDatLichAsync(lichHen);

        TempData["ThongBao"] = $"Đặt lịch thành công! Lịch hẹn #{lichHen.MaLichHen} đang chờ bác sĩ xác nhận.";
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

    private async Task<bool> KhungGioDaDuocDatAsync(int maBacSi, DateTime ngayKham, TimeSpan gioKham)
    {
        return await _db.LichHens.AnyAsync(l =>
            l.MaBacSi == maBacSi &&
            l.NgayKham == ngayKham &&
            l.GioKham == gioKham &&
            l.TrangThai != TrangThaiLichHen.DaHuy);
    }

    private async Task<IActionResult> TraVeFormDatLichAsync(DatLichViewModel model)
    {
        model.DanhSachBacSi = await _db.BacSis
            .Include(b => b.NguoiDung)
            .Include(b => b.ChuyenKhoa)
            .OrderBy(b => b.NguoiDung!.HoTen)
            .ToListAsync();
        model.BacSiHienTai = model.DanhSachBacSi.FirstOrDefault(b => b.MaBacSi == model.MaBacSi);
        return View(model);
    }

    private static bool LaLoiTrungKhungGio(DbUpdateException ex)
    {
        if (ex.InnerException is not SqlException sqlEx)
            return false;

        return sqlEx.Number is 2601 or 2627;
    }

    private int LayMaNguoiDungHienTai()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(raw, out var id) ? id : 0;
    }
}
