using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("TinTuc")]
    public class TinTuc
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaTinTuc { get; set; }

        [Required(ErrorMessage = "Vui long nhap tieu de")]
        [StringLength(300)]
        [Display(Name = "Tieu de")]
        public string TieuDe { get; set; }

        [Required(ErrorMessage = "Vui long nhap noi dung")]
        [Display(Name = "Noi dung")]
        public string NoiDung { get; set; }

        [StringLength(500)]
        [Display(Name = "Hinh anh")]
        public string HinhAnh { get; set; }

        [Display(Name = "Ngay dang")]
        public DateTime NgayDang { get; set; }

        public TinTuc()
        {
            NgayDang = DateTime.Now;
        }
    }
}