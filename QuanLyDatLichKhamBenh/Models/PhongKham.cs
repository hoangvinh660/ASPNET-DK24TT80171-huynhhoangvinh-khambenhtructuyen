using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("PhongKham")]
    public class PhongKham
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaPhongKham { get; set; }

        [Required(ErrorMessage = "Vui long nhap ten phong kham")]
        [StringLength(200)]
        [Display(Name = "Ten phong kham")]
        public string TenPhongKham { get; set; }

        [StringLength(300)]
        [Display(Name = "Dia chi")]
        public string DiaChi { get; set; }

        [StringLength(20)]
        [Display(Name = "So dien thoai")]
        public string SoDienThoai { get; set; }

        [Display(Name = "Mo ta")]
        public string MoTa { get; set; }

        public virtual ICollection<BacSi> DanhSachBacSi { get; set; }

        public PhongKham()
        {
            DanhSachBacSi = new HashSet<BacSi>();
        }
    }
}