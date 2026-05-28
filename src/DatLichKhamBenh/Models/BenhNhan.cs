using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatLichKhamBenh.Models;

// Ho so benh nhan - lien ket 1-1 voi NguoiDung (VaiTro = "BenhNhan")
[Table("BenhNhan")]
public class BenhNhan
{
    [Key]
    public int MaBenhNhan { get; set; }

    [Required]
    public int MaNguoiDung { get; set; }

    [ForeignKey(nameof(MaNguoiDung))]
    public virtual NguoiDung? NguoiDung { get; set; }

    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime? NgaySinh { get; set; }

    // "Nam" / "Nu" / "Khac"
    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    // Navigation: 1 benh nhan co nhieu lich hen
    public virtual ICollection<LichHen> DanhSachLichHen { get; set; } = new List<LichHen>();
}
