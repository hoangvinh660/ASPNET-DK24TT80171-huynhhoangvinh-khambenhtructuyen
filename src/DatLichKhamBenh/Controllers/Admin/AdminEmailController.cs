using DatLichKhamBenh.Models;
using DatLichKhamBenh.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Admin/Email/{action=Index}/{id?}")]
public class AdminEmailController : Controller
{
    private readonly AppDbContext _db;
    private readonly IEmailService _email;

    public AdminEmailController(AppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cfg = await _db.CauHinhEmails.OrderBy(c => c.MaCauHinh).FirstOrDefaultAsync();
        if (cfg is null)
        {
            // Truong hop hi huu: SeedData chua chay -> tao record trang
            cfg = new CauHinhEmail
            {
                BatEmail = true,
                NgayCapNhat = DateTime.Now
            };
            _db.CauHinhEmails.Add(cfg);
            await _db.SaveChangesAsync();
        }
        return View(cfg);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Luu(CauHinhEmail model)
    {
        // MatKhau co the de trong khi sua -> giu nguyen
        ModelState.Remove(nameof(model.SenderPassword));

        if (!ModelState.IsValid)
        {
            return View(nameof(Index), model);
        }

        var cfg = await _db.CauHinhEmails.OrderBy(c => c.MaCauHinh).FirstOrDefaultAsync();
        if (cfg is null)
        {
            cfg = new CauHinhEmail();
            _db.CauHinhEmails.Add(cfg);
        }

        cfg.BatEmail = model.BatEmail;
        cfg.SmtpServer = (model.SmtpServer ?? "").Trim();
        cfg.Port = model.Port;
        cfg.SenderName = (model.SenderName ?? "").Trim();
        cfg.SenderEmail = (model.SenderEmail ?? "").Trim();
        // Chi cap nhat mat khau neu admin nhap moi (khac chuoi rong)
        if (!string.IsNullOrWhiteSpace(model.SenderPassword))
        {
            cfg.SenderPassword = model.SenderPassword.Trim();
        }
        cfg.NgayCapNhat = DateTime.Now;

        await _db.SaveChangesAsync();

        TempData["ThongBao"] = "Da luu cau hinh email.";
        return RedirectToAction(nameof(Index));
    }

    // Bat tat nhanh chi voi 1 click
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BatTat()
    {
        var cfg = await _db.CauHinhEmails.OrderBy(c => c.MaCauHinh).FirstOrDefaultAsync();
        if (cfg is null) return RedirectToAction(nameof(Index));

        cfg.BatEmail = !cfg.BatEmail;
        cfg.NgayCapNhat = DateTime.Now;
        await _db.SaveChangesAsync();

        TempData["ThongBao"] = cfg.BatEmail
            ? "Da BAT gui email."
            : "Da TAT gui email - moi thao tac se chi log thay vi gui that.";
        return RedirectToAction(nameof(Index));
    }

    // Gui mail thu nghiem den 1 dia chi (de Admin kiem tra cau hinh)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GuiThu(string emailNhan)
    {
        if (string.IsNullOrWhiteSpace(emailNhan))
        {
            TempData["LoiThongBao"] = "Vui long nhap email nhan thu.";
            return RedirectToAction(nameof(Index));
        }

        var html = $@"
            <div style='font-family:Segoe UI,Arial,sans-serif;font-size:14px;color:#333'>
                <h2 style='color:#2563eb'>Email kiem tra cau hinh</h2>
                <p>Day la mail kiem tra cau hinh SMTP cua he thong DatLichKhamBenh.</p>
                <p>Neu ban nhan duoc thu nay, cau hinh email da hoat dong dung.</p>
                <p style='color:#666;font-size:12px'>Thoi gian gui: {DateTime.Now:dd/MM/yyyy HH:mm:ss}</p>
            </div>";

        await _email.SendAsync(
            emailNhan.Trim(),
            "Quan tri vien",
            "[DatLichKhamBenh] Email kiem tra cau hinh",
            html);

        TempData["ThongBao"] =
            $"Da goi yeu cau gui mail thu toi '{emailNhan}'. Kiem tra log/hop thu de xac nhan ket qua.";
        return RedirectToAction(nameof(Index));
    }
}
