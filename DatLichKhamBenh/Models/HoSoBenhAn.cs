using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatLichKhamBenh.Models;

// Ho so benh an - sinh ra sau khi bac si kham xong cho 1 LichHen
[Table("HoSoBenhAn")]
public class HoSoBenhAn
{
    [Key]
    public int MaHoSo { get; set; }

    [Required]
    public int MaLichHen { get; set; }

    [ForeignKey(nameof(MaLichHen))]
    public virtual LichHen? LichHen { get; set; }

    [Required(ErrorMessage = "Vui long nhap chan doan")]
    [StringLength(1000)]
    public string ChanDoan { get; set; } = string.Empty;

    [StringLength(2000)]
    public string? DonThuoc { get; set; }

    [StringLength(2000)]
    public string? LoiKhuyen { get; set; }

    public DateTime NgayTao { get; set; } = DateTime.Now;
}
