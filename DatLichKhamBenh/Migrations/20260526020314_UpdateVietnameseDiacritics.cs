using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatLichKhamBenh.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVietnameseDiacritics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Chuyen khoa
            migrationBuilder.Sql(@"
UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Nội tổng quát'
WHERE [TenChuyenKhoa] = N'Noi tong quat';

UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Ngoại tổng quát'
WHERE [TenChuyenKhoa] = N'Ngoai tong quat';

UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Sản phụ khoa'
WHERE [TenChuyenKhoa] = N'San phu khoa';

UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Mắt'
WHERE [TenChuyenKhoa] = N'Mat';

UPDATE [ChuyenKhoa] SET [MoTa] = N'Khám và điều trị các bệnh lý nội khoa thông thường.'
WHERE [MoTa] = N'Kham va dieu tri cac benh ly noi khoa thong thuong.';

UPDATE [ChuyenKhoa] SET [MoTa] = N'Phẫu thuật và điều trị các bệnh lý ngoại khoa.'
WHERE [MoTa] = N'Phau thuat va dieu tri cac benh ly ngoai khoa.';

UPDATE [ChuyenKhoa] SET [MoTa] = N'Khám thai, sản khoa và các bệnh lý phụ nữ.'
WHERE [MoTa] = N'Kham thai, san khoa va cac benh ly phu nu.';

UPDATE [ChuyenKhoa] SET [MoTa] = N'Khám và điều trị bệnh lý trẻ em dưới 15 tuổi.'
WHERE [MoTa] = N'Kham va dieu tri benh ly tre em duoi 15 tuoi.';

UPDATE [ChuyenKhoa] SET [MoTa] = N'Khám, đo mắt và điều trị các bệnh lý về mắt.'
WHERE [MoTa] = N'Kham, do mat va dieu tri cac benh ly ve mat.';
");

            // Nguoi dung (HoTen)
            migrationBuilder.Sql(@"
UPDATE [NguoiDung] SET [HoTen] = N'Nguyễn Văn An' WHERE [HoTen] = N'Nguyen Van An';
UPDATE [NguoiDung] SET [HoTen] = N'Trần Thị Bình' WHERE [HoTen] = N'Tran Thi Binh';
UPDATE [NguoiDung] SET [HoTen] = N'Lê Văn Cường' WHERE [HoTen] = N'Le Van Cuong';
UPDATE [NguoiDung] SET [HoTen] = N'Phạm Thị Dung' WHERE [HoTen] = N'Pham Thi Dung';
UPDATE [NguoiDung] SET [HoTen] = N'Hoàng Văn Em' WHERE [HoTen] = N'Hoang Van Em';
UPDATE [NguoiDung] SET [HoTen] = N'Vũ Thị Hoa' WHERE [HoTen] = N'Vu Thi Hoa';
UPDATE [NguoiDung] SET [HoTen] = N'Đỗ Văn Minh' WHERE [HoTen] = N'Do Van Minh';
");

            // Bac si (HocVi / KinhNghiem / MoTa)
            migrationBuilder.Sql(@"
UPDATE [BacSi] SET [HocVi] = N'Tiến sĩ - Bác sĩ' WHERE [HocVi] = N'Tien si - Bac si';
UPDATE [BacSi] SET [HocVi] = N'Thạc sĩ - Bác sĩ' WHERE [HocVi] = N'Thac si - Bac si';
UPDATE [BacSi] SET [HocVi] = N'Bác sĩ chuyên khoa II' WHERE [HocVi] = N'Bac si chuyen khoa II';
UPDATE [BacSi] SET [HocVi] = N'Bác sĩ chuyên khoa I' WHERE [HocVi] = N'Bac si chuyen khoa I';

UPDATE [BacSi] SET [KinhNghiem] = N'15 năm kinh nghiệm tại BV Bạch Mai.'
WHERE [KinhNghiem] = N'15 nam kinh nghiem tai BV Bach Mai.';

UPDATE [BacSi] SET [KinhNghiem] = N'10 năm kinh nghiệm phẫu thuật tổng quát.'
WHERE [KinhNghiem] = N'10 nam kinh nghiem phau thuat tong quat.';

UPDATE [BacSi] SET [KinhNghiem] = N'12 năm sản phụ khoa tại BV Từ Dũ.'
WHERE [KinhNghiem] = N'12 nam san phu khoa tai BV Tu Du.';

UPDATE [BacSi] SET [KinhNghiem] = N'8 năm khám nhi tại BV Nhi Trung ương.'
WHERE [KinhNghiem] = N'8 nam kham nhi tai BV Nhi Trung uong.';

UPDATE [BacSi] SET [KinhNghiem] = N'9 năm tại BV Mắt Trung ương.'
WHERE [KinhNghiem] = N'9 nam tai BV Mat Trung uong.';

UPDATE [BacSi] SET [MoTa] = N'Chuyên về các bệnh lý tim mạch và tiêu hoá.'
WHERE [MoTa] = N'Chuyen ve cac benh ly tim mach va tieu hoa.';

UPDATE [BacSi] SET [MoTa] = N'Phẫu thuật tiêu hoá, gan mật tuỵ, thoát vị.'
WHERE [MoTa] = N'Phau thuat tieu hoa, gan mat tuy, thoat vi.';

UPDATE [BacSi] SET [MoTa] = N'Theo dõi thai kỳ, sản khoa nguy cơ cao.'
WHERE [MoTa] = N'Theo doi thai ky, san khoa nguy co cao.';

UPDATE [BacSi] SET [MoTa] = N'Sơ sinh, nhi tổng quát, tư vấn dinh dưỡng trẻ.'
WHERE [MoTa] = N'So sinh, nhi tong quat, tu van dinh duong tre.';
");

            // Benh nhan
            migrationBuilder.Sql(@"
UPDATE [BenhNhan] SET [GioiTinh] = N'Nữ' WHERE [GioiTinh] = N'Nu';

UPDATE [BenhNhan] SET [DiaChi] = N'123 Lê Lợi, Quận 1, TP.HCM'
WHERE [DiaChi] = N'123 Le Loi, Quan 1, TP HCM';

UPDATE [BenhNhan] SET [DiaChi] = N'456 Trần Hưng Đạo, Hai Bà Trưng, Hà Nội'
WHERE [DiaChi] = N'456 Tran Hung Dao, Hai Ba Trung, Ha Noi';
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rollback ve chuoi khong dau (chi cho dung exact mapping)
            migrationBuilder.Sql(@"
UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Noi tong quat' WHERE [TenChuyenKhoa] = N'Nội tổng quát';
UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Ngoai tong quat' WHERE [TenChuyenKhoa] = N'Ngoại tổng quát';
UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'San phu khoa' WHERE [TenChuyenKhoa] = N'Sản phụ khoa';
UPDATE [ChuyenKhoa] SET [TenChuyenKhoa] = N'Mat' WHERE [TenChuyenKhoa] = N'Mắt';
");
        }
    }
}
