namespace DatLichKhamBenh.Services;

public interface IBacSiImageService
{
    /// <summary>URL hien thi (kiem tra file ton tai, fallback anh mac dinh)</summary>
    string LayUrlAnh(string? duongDan);

    /// <summary>Luu file upload; tra ve duong dan web (/images/bacsi/...) hoac loi</summary>
    Task<(string? DuongDan, string? Loi)> LuuAnhAsync(IFormFile? file, string? anhCu = null, CancellationToken ct = default);

    /// <summary>Xoa file anh cu (bo qua anh mac dinh)</summary>
    void XoaAnh(string? duongDan);
}
