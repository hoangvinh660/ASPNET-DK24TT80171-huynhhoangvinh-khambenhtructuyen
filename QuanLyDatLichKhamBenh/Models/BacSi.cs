using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("BacSi")]
    public class BacSi
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBacSi { get; set; }

        [Required(ErrorMessage = "Vui long nhap ho ten")]
        [StringLength(100)]
        [Display(Name = "Ho ten")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui long chon chuyen khoa")]
        [Display(Name = "Chuyen khoa")]
        public int MaChuyenKhoa { get; set; }

        [Required(ErrorMessage = "Vui long chon phong kham")]
        [Display(Name = "Phong kham")]
        public int MaPhongKham { get; set; }

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "Email khong hop le")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(20)]
        [Display(Name = "So dien thoai")]
        public string SoDienThoai { get; set; }

        [StringLength(500)]
        [Display(Name = "Hinh anh")]
        public string HinhAnh { get; set; }

        [Display(Name = "Mo ta")]
        public string MoTa { get; set; }

        [Display(Name = "So nam kinh nghiem")]
        public int SoNamKinhNghiem { get; set; }

        [Display(Name = "Phi kham")]
        public decimal PhiKham { get; set; }

        [StringLength(128)]
        [Display(Name = "Tai khoan")]
        public string MaTaiKhoan { get; set; }

        [ForeignKey("MaChuyenKhoa")]
        public virtual ChuyenKhoa ChuyenKhoa { get; set; }

        [ForeignKey("MaPhongKham")]
        public virtual PhongKham PhongKham { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual ApplicationUser TaiKhoan { get; set; }

        public virtual ICollection<LichLamViec> DanhSachLichLamViec { get; set; }
        public virtual ICollection<LichHen> DanhSachLichHen { get; set; }

        public BacSi()
        {
            DanhSachLichLamViec = new HashSet<LichLamViec>();
            DanhSachLichHen = new HashSet<LichHen>();
        }
    }
}