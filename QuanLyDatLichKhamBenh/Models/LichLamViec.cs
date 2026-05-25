using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("LichLamViec")]
    public class LichLamViec
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaLichLamViec { get; set; }

        [Required]
        [Display(Name = "Bac si")]
        public int MaBacSi { get; set; }

        [Required]
        [Column(TypeName = "date")]
        [Display(Name = "Ngay lam viec")]
        [DataType(DataType.Date)]
        public DateTime NgayLamViec { get; set; }

        [Required]
        [Column(TypeName = "time")]
        [Display(Name = "Gio bat dau")]
        public TimeSpan GioBatDau { get; set; }

        [Required]
        [Column(TypeName = "time")]
        [Display(Name = "Gio ket thuc")]
        public TimeSpan GioKetThuc { get; set; }

        [Display(Name = "Con trong")]
        public bool ConTrong { get; set; }

        [ForeignKey("MaBacSi")]
        public virtual BacSi BacSi { get; set; }

        public virtual ICollection<LichHen> DanhSachLichHen { get; set; }

        public LichLamViec()
        {
            ConTrong = true;
            DanhSachLichHen = new HashSet<LichHen>();
        }
    }
}