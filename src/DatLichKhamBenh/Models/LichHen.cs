using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatLichKhamBenh.Models;

// Cac trang thai cua lich hen - de tap trung mot cho cho de tra cuu/so sanh
public static class TrangThaiLichHen
{
    public const string ChoXacNhan = "ChoXacNhan";  // hien thi: Cho xac nhan
    public const string DaXacNhan = "DaXacNhan";    // hien thi: Da xac nhan
    public const string DaKham = "DaKham";          // hien thi: Da kham
    public const string DaHuy = "DaHuy";            // hien thi: Da huy
}

// Lich hen kham benh giua benh nhan va bac si
[Table("LichHen")]
public class LichHen
{
    [Key]
    public int MaLichHen { get; set; }

    [Required]
    public int MaBenhNhan { get; set; }

    [ForeignKey(nameof(MaBenhNhan))]
    public virtual BenhNhan? BenhNhan { get; set; }

    [Required]
    public int MaBacSi { get; set; }

    [ForeignKey(nameof(MaBacSi))]
    public virtual BacSi? BacSi { get; set; }

    [Required(ErrorMessage = "Vui long chon ngay kham")]
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime NgayKham { get; set; }

    // Khung gio kham (vi du: 08:00, 09:30...)
    [Required(ErrorMessage = "Vui long chon gio kham")]
    [Column(TypeName = "time")]
    public TimeSpan GioKham { get; set; }

    [StringLength(500)]
    public string? LyDoKham { get; set; }

    // Gia tri lay tu TrangThaiLichHen
    [Required]
    [StringLength(20)]
    public string TrangThai { get; set; } = TrangThaiLichHen.ChoXacNhan;

    public DateTime NgayDat { get; set; } = DateTime.Now;

    // Navigation: 1 lich hen co the dan toi 1 ho so benh an sau khi kham
    public virtual HoSoBenhAn? HoSoBenhAn { get; set; }
}
