namespace DatLichKhamBenh.Services;

// Cau hinh upload anh dai dien bac si (doc tu appsettings.json -> BacSiUpload)
public class BacSiUploadSettings
{
    /// <summary>Thu muc luu trong wwwroot, vi du: images/bacsi</summary>
    public string ThuMucLuu { get; set; } = "images/bacsi";

    /// <summary>Kich thuoc toi da (MB)</summary>
    public int KichThuocToiDaMb { get; set; } = 2;

    /// <summary>Dinh dang file cho phep (co dau cham)</summary>
    public string[] DinhDangChoPhep { get; set; } = [".jpg", ".jpeg", ".png", ".webp"];

    /// <summary>Duong dan anh mac dinh khi chua upload</summary>
    public string AnhMacDinh { get; set; } = "/images/bacsi/default-avatar.svg";

    public long KichThuocToiDaBytes => KichThuocToiDaMb * 1024L * 1024L;
}
