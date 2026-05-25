using System.Security.Claims;
using DatLichKhamBenh.Models;
using DatLichKhamBenh.Models.ViewModels;
using DatLichKhamBenh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers;

// Controller cho bac si xu ly cac lich hen den voi minh.
// Chuc nang: xem danh sach, xac nhan, tu choi, nhap ket qua kham (tao HoSoBenhAn).
[Authorize(Roles = "BacSi")]
public class BacSiLichHenController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public BacSiLichHenController(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    // GET /BacSiLichHen?trangThai=ChoXacNhan
    public async Task<IActionResult> Index(string? trangThai)
    {
        var maBacSi = await LayMaBacSiHienTaiAsync();
        if (maBacSi is null)
        {
            TempData["LoiThongBao"] = "Tai khoan cua ban chua duoc gan voi ho so bac si nao.";
            return RedirectToAction("Index", "TrangChu");
        }

        var query = _db.LichHens
            .Include(l => l.BenhNhan).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.HoSoBenhAn)
            .Where(l => l.MaBacSi == maBacSi.Value)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(trangThai) &&
            new[] { TrangThaiLichHen.ChoXacNhan, TrangThaiLichHen.DaXacNhan, TrangThaiLichHen.DaKham, TrangThaiLichHen.DaHuy }
                .Contains(trangThai))
        {
            query = query.Where(l => l.TrangThai == trangThai);
        }

        var ds = await query
            .OrderByDescending(l => l.NgayKham)
            .ThenByDescending(l => l.GioKham)
            .ToListAsync();

        ViewBag.TrangThaiLoc = trangThai;
        return View(ds);
    }

    // POST /BacSiLichHen/XacNhan/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> XacNhan(int id)
    {
        var lichHen = await LayLichHenCuaBacSiAsync(id);
        if (lichHen is null) return NotFound();

        if (lichHen.TrangThai != TrangThaiLichHen.ChoXacNhan)
        {
            TempData["LoiThongBao"] = "Chi co the xac nhan lich dang o trang thai 'Cho xac nhan'.";
            return RedirectToAction(nameof(Index));
        }

        lichHen.TrangThai = TrangThaiLichHen.DaXacNhan;
        await _db.SaveChangesAsync();

        // Nap navigation roi gui mail xac nhan cho benh nhan
        var lichDayDu = await _db.LichHens
            .Include(l => l.BenhNhan).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.BacSi).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.BacSi).ThenInclude(b => b!.ChuyenKhoa)
            .FirstAsync(l => l.MaLichHen == lichHen.MaLichHen);
        await _email.GuiMailXacNhanLichAsync(lichDayDu);

        TempData["ThongBao"] = $"Da xac nhan lich hen #{lichHen.MaLichHen}.";
        return RedirectToAction(nameof(Index));
    }

    // POST /BacSiLichHen/TuChoi/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TuChoi(int id)
    {
        var lichHen = await LayLichHenCuaBacSiAsync(id);
        if (lichHen is null) return NotFound();

        if (lichHen.TrangThai is TrangThaiLichHen.DaKham or TrangThaiLichHen.DaHuy)
        {
            TempData["LoiThongBao"] = "Khong the tu choi lich da kham hoac da huy.";
            return RedirectToAction(nameof(Index));
        }

        lichHen.TrangThai = TrangThaiLichHen.DaHuy;
        await _db.SaveChangesAsync();

        var lichDayDu = await _db.LichHens
            .Include(l => l.BenhNhan).ThenInclude(b => b!.NguoiDung)
            .Include(l => l.BacSi).ThenInclude(b => b!.NguoiDung)
            .FirstAsync(l => l.MaLichHen == lichHen.MaLichHen);
        await _email.GuiMailHuyLichAsync(lichDayDu, "Bac si tu choi/huy lich");

        TempData["ThongBao"] = $"Da tu choi (huy) lich hen #{lichHen.MaLichHen}.";
        return RedirectToAction(nameof(Index));
    }

    // GET /BacSiLichHen/NhapKetQua/5
    [HttpGet]
    public async Task<IActionResult> NhapKetQua(int id)
    {
        var lichHen = await LayLichHenCuaBacSiAsync(id, loadFull: true);
        if (lichHen is null) return NotFound();

        if (lichHen.TrangThai is TrangThaiLichHen.DaHuy)
        {
            TempData["LoiThongBao"] = "Lich hen da huy, khong the nhap ket qua.";
            return RedirectToAction(nameof(Index));
        }

        var vm = new NhapKetQuaViewModel
        {
            MaLichHen = lichHen.MaLichHen,
            LichHen = lichHen,
            // Neu da co ho so truoc do thi load len de sua
            ChanDoan = lichHen.HoSoBenhAn?.ChanDoan ?? string.Empty,
            DonThuoc = lichHen.HoSoBenhAn?.DonThuoc,
            LoiKhuyen = lichHen.HoSoBenhAn?.LoiKhuyen
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NhapKetQua(NhapKetQuaViewModel model)
    {
        var lichHen = await LayLichHenCuaBacSiAsync(model.MaLichHen, loadFull: true);
        if (lichHen is null) return NotFound();

        if (lichHen.TrangThai is TrangThaiLichHen.DaHuy)
        {
            TempData["LoiThongBao"] = "Lich hen da huy, khong the nhap ket qua.";
            return RedirectToAction(nameof(Index));
        }

        if (!ModelState.IsValid)
        {
            model.LichHen = lichHen;
            return View(model);
        }

        if (lichHen.HoSoBenhAn is null)
        {
            // Lan dau nhap -> tao moi
            var hoSo = new HoSoBenhAn
            {
                MaLichHen = lichHen.MaLichHen,
                ChanDoan = model.ChanDoan,
                DonThuoc = model.DonThuoc,
                LoiKhuyen = model.LoiKhuyen,
                NgayTao = DateTime.Now
            };
            _db.HoSoBenhAns.Add(hoSo);
        }
        else
        {
            // Da co ho so -> cap nhat
            lichHen.HoSoBenhAn.ChanDoan = model.ChanDoan;
            lichHen.HoSoBenhAn.DonThuoc = model.DonThuoc;
            lichHen.HoSoBenhAn.LoiKhuyen = model.LoiKhuyen;
        }

        // Chuyen trang thai sang Da kham
        lichHen.TrangThai = TrangThaiLichHen.DaKham;
        await _db.SaveChangesAsync();

        TempData["ThongBao"] = $"Da luu ket qua kham cho lich hen #{lichHen.MaLichHen}.";
        return RedirectToAction(nameof(Index));
    }

    // ---------- HELPER ----------

    // Lay MaBacSi cua nguoi dung dang dang nhap (dua tren claim MaNguoiDung)
    private async Task<int?> LayMaBacSiHienTaiAsync()
    {
        var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(raw, out var maNguoiDung)) return null;
        var bacSi = await _db.BacSis.FirstOrDefaultAsync(b => b.MaNguoiDung == maNguoiDung);
        return bacSi?.MaBacSi;
    }

    // Lay 1 lich hen va kiem tra no thuoc ve bac si dang dang nhap
    private async Task<LichHen?> LayLichHenCuaBacSiAsync(int maLichHen, bool loadFull = false)
    {
        var maBacSi = await LayMaBacSiHienTaiAsync();
        if (maBacSi is null) return null;

        IQueryable<LichHen> q = _db.LichHens;
        if (loadFull)
        {
            q = q.Include(l => l.BenhNhan).ThenInclude(b => b!.NguoiDung)
                 .Include(l => l.BacSi).ThenInclude(b => b!.ChuyenKhoa)
                 .Include(l => l.HoSoBenhAn);
        }

        var lichHen = await q.FirstOrDefaultAsync(l => l.MaLichHen == maLichHen);
        if (lichHen is null || lichHen.MaBacSi != maBacSi.Value) return null;
        return lichHen;
    }
}
