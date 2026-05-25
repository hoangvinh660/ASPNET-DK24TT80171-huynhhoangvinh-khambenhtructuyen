using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyDatLichKhamBenh.Models
{
    [Table("BenhNhan")]
    public class BenhNhan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaBenhNhan { get; set; }

        [Required(ErrorMessage = "Vui long nhap ho ten")]
        [StringLength(100)]
        [Display(Name = "Ho ten")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Vui long nhap ngay sinh")]
        [Display(Name = "Ngay sinh")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime NgaySinh { get; set; }

        [Required(ErrorMessage = "Vui long chon gioi tinh")]
        [StringLength(10)]
        [Display(Name = "Gioi tinh")]
        public string GioiTinh { get; set; }

        [StringLength(20)]
        [Display(Name = "So dien thoai")]
        public string SoDienThoai { get; set; }

        [StringLength(300)]
        [Display(Name = "Dia chi")]
        public string DiaChi { get; set; }

        [StringLength(128)]
        [Display(Name = "Tai khoan")]
        public string MaTaiKhoan { get; set; }

        [ForeignKey("MaTaiKhoan")]
        public virtual ApplicationUser TaiKhoan { get; set; }

        public virtual ICollection<LichHen> DanhSachLichHen { get; set; }

        public BenhNhan()
        {
            DanhSachLichHen = new HashSet<LichHen>();
        }
    }
}