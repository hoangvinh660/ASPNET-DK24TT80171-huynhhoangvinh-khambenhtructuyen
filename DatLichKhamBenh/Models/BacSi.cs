using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatLichKhamBenh.Models;

// Ho so bac si - lien ket 1-1 voi NguoiDung (VaiTro = "BacSi")
public class BacSi
{
    [Key]
    public int MaBacSi { get; set; }

    [Required]
    public int MaNguoiDung { get; set; }

    [ForeignKey(nameof(MaNguoiDung))]
    public virtual NguoiDung? NguoiDung { get; set; }

    [Required]
    public int MaChuyenKhoa { get; set; }

    [ForeignKey(nameof(MaChuyenKhoa))]
    public virtual ChuyenKhoa? ChuyenKhoa { get; set; }

    // Vi du: Bac si, Thac si, Tien si, GS.TS...
    [Required(ErrorMessage = "Vui long nhap hoc vi")]
    [StringLength(100)]
    public string HocVi { get; set; } = string.Empty;

    // Vi du: "10 nam kinh nghiem tai BV Bach Mai"
    [StringLength(500)]
    public string? KinhNghiem { get; set; }

    [StringLength(2000)]
    public string? MoTa { get; set; }

    // Duong dan toi anh trong wwwroot (vi du: /images/bacsi/bs01.jpg)
    [StringLength(255)]
    public string? HinhAnh { get; set; }

    // Don vi: VND (luu nguyen tien, khong co phan thap phan)
    [Required(ErrorMessage = "Vui long nhap gia kham")]
    [Range(0, 100_000_000, ErrorMessage = "Gia kham phai >= 0")]
    [Column(TypeName = "decimal(18,0)")]
    public decimal GiaKham { get; set; }

    // Navigation: 1 bac si co nhieu lich hen
    public virtual ICollection<LichHen> DanhSachLichHen { get; set; } = new List<LichHen>();
}
