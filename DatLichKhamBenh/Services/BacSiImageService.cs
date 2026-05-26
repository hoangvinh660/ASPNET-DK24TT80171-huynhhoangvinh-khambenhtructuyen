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

    public void DamBaoAnhMacDinhTonTai()
    {
        var thuMucVatLy = Path.Combine(_env.WebRootPath, _opt.ThuMucLuu.Replace('/', Path.DirectorySeparatorChar));
        Directory.CreateDirectory(thuMucVatLy);

        var duongDanMacDinh = Path.Combine(_env.WebRootPath,
            _opt.AnhMacDinh.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
        if (File.Exists(duongDanMacDinh))
            return;

        var thuMucAnhMacDinh = Path.GetDirectoryName(duongDanMacDinh);
        if (!string.IsNullOrEmpty(thuMucAnhMacDinh))
            Directory.CreateDirectory(thuMucAnhMacDinh);

        File.WriteAllText(duongDanMacDinh, NoiDungAnhMacDinhSvg);
        _logger.LogInformation("Da tao anh mac dinh: {Path}", _opt.AnhMacDinh);
    }

    private bool LaAnhMacDinh(string duongDan)
    {
        var a = duongDan.Trim().TrimEnd('/');
        var b = _opt.AnhMacDinh.Trim().TrimEnd('/');
        return string.Equals(a, b, StringComparison.OrdinalIgnoreCase)
               || a.EndsWith("/default.png", StringComparison.OrdinalIgnoreCase)
               || a.EndsWith("/default-avatar.svg", StringComparison.OrdinalIgnoreCase);
    }

    private const string NoiDungAnhMacDinhSvg = """
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 128 128" role="img" aria-label="Anh mac dinh bac si">
          <defs>
            <linearGradient id="bg" x1="0%" y1="0%" x2="100%" y2="100%">
              <stop offset="0%" stop-color="#e8f0fe"/>
              <stop offset="100%" stop-color="#cfe2ff"/>
            </linearGradient>
          </defs>
          <rect width="128" height="128" fill="url(#bg)"/>
          <circle cx="64" cy="64" r="58" fill="none" stroke="#0d6efd" stroke-width="2" opacity="0.15"/>
          <circle cx="64" cy="44" r="20" fill="#0d6efd" opacity="0.85"/>
          <path d="M28 108c7-22 24-34 36-34s29 12 36 34" fill="#0d6efd" opacity="0.85"/>
          <circle cx="64" cy="72" r="7" fill="none" stroke="#ffffff" stroke-width="2.5"/>
          <path d="M64 79v6M58 85h12" stroke="#ffffff" stroke-width="2" stroke-linecap="round"/>
          <path d="M71 72c4-6 10-8 14-6" fill="none" stroke="#ffffff" stroke-width="2" stroke-linecap="round"/>
        </svg>
        """;
}
