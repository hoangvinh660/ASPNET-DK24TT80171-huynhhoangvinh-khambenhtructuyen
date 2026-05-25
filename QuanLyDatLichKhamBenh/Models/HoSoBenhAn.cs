using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("HoSoBenhAn")]
    public class HoSoBenhAn
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaHoSo { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [Display(Name = "Lich hen")]
        public int MaLichHen { get; set; }

        [Display(Name = "Chan doan")]
        public string ChanDoan { get; set; }

        [Display(Name = "Don thuoc")]
        public string DonThuoc { get; set; }

        [Display(Name = "Loi dan")]
        public string LoiDan { get; set; }

        [Display(Name = "Ngay tao")]
        public DateTime NgayTao { get; set; }

        public virtual LichHen LichHen { get; set; }

        public HoSoBenhAn()
        {
            NgayTao = DateTime.Now;
        }
    }
}