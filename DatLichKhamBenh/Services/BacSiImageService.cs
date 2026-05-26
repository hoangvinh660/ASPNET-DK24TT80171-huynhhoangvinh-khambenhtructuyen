using Microsoft.Extensions.Options;

namespace DatLichKhamBenh.Services;

public class BacSiImageService : IBacSiImageService
{
    private readonly BacSiUploadSettings _opt;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<BacSiImageService> _logger;

    public BacSiImageService(
        IOptions<BacSiUploadSettings> opt,
        IWebHostEnvironment env,
        ILogger<BacSiImageService> logger)
    {
        _opt = opt.Value;
        _env = env;
        _logger = logger;
    }

    public string LayUrlAnh(string? duongDan)
    {
        if (string.IsNullOrWhiteSpace(duongDan))
            return _opt.AnhMacDinh;

        var normalized = duongDan.Trim();
        if (normalized.StartsWith('/'))
        {
            var physical = Path.Combine(_env.WebRootPath, normalized.TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(physical))
                return normalized;
        }

        return _opt.AnhMacDinh;
    }

    public async Task<(string? DuongDan, string? Loi)> LuuAnhAsync(
        IFormFile? file,
        string? anhCu = null,
        CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            return (null, null);

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_opt.DinhDangChoPhep.Contains(ext, StringComparer.OrdinalIgnoreCase))
        {
            var ds = string.Join(", ", _opt.DinhDangChoPhep);
            return (null, $"Chỉ chấp nhận file ảnh: {ds}");
        }

        if (file.Length > _opt.KichThuocToiDaBytes)
        {
            return (null, $"Ảnh không được vượt quá {_opt.KichThuocToiDaMb} MB.");
        }

        var thuMucVatLy = Path.Combine(_env.WebRootPath, _opt.ThuMucLuu.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(thuMucVatLy);

        var tenFile = $"bs_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{ext}";
        var duongDanVatLy = Path.Combine(thuMucVatLy, tenFile);

        await using (var stream = new FileStream(duongDanVatLy, FileMode.Create))
        {
            await file.CopyToAsync(stream, ct);
        }

        var duongDanWeb = $"/{_opt.ThuMucLuu.Trim('/')}/{tenFile}".Replace('\\', '/');

        if (!string.IsNullOrWhiteSpace(anhCu) && !LaAnhMacDinh(anhCu))
        {
            XoaAnh(anhCu);
        }

        _logger.LogInformation("Da luu anh bac si: {Path}", duongDanWeb);
        return (duongDanWeb, null);
    }

    public void XoaAnh(string? duongDan)
    {
        if (string.IsNullOrWhiteSpace(duongDan) || LaAnhMacDinh(duongDan))
            return;

        try
        {
            var physical = Path.Combine(_env.WebRootPath, duongDan.TrimStart('/')
                .Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(physical))
                File.Delete(physical);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Khong xoa duoc anh: {Path}", duongDan);
        }
    }

    private bool LaAnhMacDinh(string duongDan)
    {
        var a = duongDan.Trim().TrimEnd('/');
        var b = _opt.AnhMacDinh.Trim().TrimEnd('/');
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase)
               || a.EndsWith("/default.png", StringComparison.OrdinalIgnoreCase)
               || a.EndsWith("/default-avatar.svg", StringComparison.OrdinalIgnoreCase);
    }
}
