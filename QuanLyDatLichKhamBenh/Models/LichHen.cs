using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("LichHen")]
    public class LichHen
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLichHen { get; set; }

        [Required]
        [Display(Name = "Benh nhan")]
        public int MaBenhNhan { get; set; }

        [Required]
        [Display(Name = "Bac si")]
        public int MaBacSi { get; set; }

        [Required]
        [Display(Name = "Lich lam viec")]
        public int MaLichLamViec { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "Ngay hen")]
        [DataType(DataType.Date)]
        public DateTime NgayHen { get; set; }

        [Required]
        [Column(TypeName = "time")]
        [Display(Name = "Gio hen")]
        public TimeSpan GioHen { get; set; }

        [Display(Name = "Trieu chung")]
        public string TrieuChung { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Trang thai")]
        public string TrangThai { get; set; }

        [Display(Name = "Ngay tao")]
        public DateTime NgayTao { get; set; }

        [Display(Name = "Ghi chu")]
        public string GhiChu { get; set; }

        [ForeignKey("MaBenhNhan")]
        public virtual BenhNhan BenhNhan { get; set; }

        [ForeignKey("MaBacSi")]
        public virtual BacSi BacSi { get; set; }

        [ForeignKey("MaLichLamViec")]
        public virtual LichLamViec LichLamViec { get; set; }

        public virtual HoSoBenhAn HoSoBenhAn { get; set; }

        public LichHen()
        {
            TrangThai = TrangThaiLichHen.ChoXacNhan;
            NgayTao = DateTime.Now;
        }
    }
}