using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("ChuyenKhoa")]
    public class ChuyenKhoa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaChuyenKhoa { get; set; }

        [Required(ErrorMessage = "Vui long nhap ten chuyen khoa")]
        [StringLength(200)]
        [Display(Name = "Ten chuyen khoa")]
        public string TenChuyenKhoa { get; set; }

        [Display(Name = "Mo ta")]
        public string MoTa { get; set; }

        [StringLength(500)]
        [Display(Name = "Hinh anh")]
        public string HinhAnh { get; set; }

        public virtual ICollection<BacSi> DanhSachBacSi { get; set; }

        public ChuyenKhoa()
        {
            DanhSachBacSi = new HashSet<BacSi>();
        }
    }
}