using DatLichKhamBenh.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatLichKhamBenh.Models;

// Nap du lieu mau cho he thong (chay 1 lan luc startup neu DB rong).
// Du lieu gom: 1 admin + 5 chuyen khoa + 5 bac si + 2 benh nhan + 1 cau hinh email.
public static class SeedData
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Bao dam DB ton tai va co schema moi nhat
        db.Database.Migrate();

        // ---------- Seed cau hinh email (doc lap voi seed nguoi dung) ----------
        if (!db.CauHinhEmails.Any())
        {
            var emailFallback = scope.ServiceProvider.GetRequiredService<IOptions<EmailSettings>>().Value;
            db.CauHinhEmails.Add(new CauHinhEmail
            {
                BatEmail = true,
                SmtpServer = emailFallback.SmtpServer,
                Port = emailFallback.Port,
                SenderName = emailFallback.SenderName,
                SenderEmail = emailFallback.SenderEmail,
                SenderPassword = emailFallback.SenderPassword,
                NgayCapNhat = DateTime.Now
            });
            db.SaveChanges();
        }

        // Neu da co nguoi dung -> coi nhu da seed roi, bo qua
        if (db.NguoiDungs.Any())
        {
            return;
        }

        // ---------- 1. Tai khoan Admin ----------
        var admin = new NguoiDung
        {
            TenDangNhap = "admin",
            MatKhau = "Admin@123",
            HoTen = "Quan tri vien",
            Email = "admin@datlichkhambenh.local",
            SoDienThoai = "0900000000",
            VaiTro = "Admin",
            NgayTao = DateTime.Now
        };
        db.NguoiDungs.Add(admin);

        // ---------- 2. Cac chuyen khoa ----------
        var dsChuyenKhoa = new List<ChuyenKhoa>
        {
            new() { TenChuyenKhoa = "Nội tổng quát", MoTa = "Khám và điều trị các bệnh lý nội khoa thông thường." },
            new() { TenChuyenKhoa = "Ngoại tổng quát", MoTa = "Phẫu thuật và điều trị các bệnh lý ngoại khoa." },
            new() { TenChuyenKhoa = "Sản phụ khoa", MoTa = "Khám thai, sản khoa và các bệnh lý phụ nữ." },
            new() { TenChuyenKhoa = "Nhi khoa", MoTa = "Khám và điều trị bệnh lý trẻ em dưới 15 tuổi." },
            new() { TenChuyenKhoa = "Mắt", MoTa = "Khám, đo mắt và điều trị các bệnh lý về mắt." },
        };
        db.ChuyenKhoas.AddRange(dsChuyenKhoa);

        // Luu de co MaNguoiDung va MaChuyenKhoa
        db.SaveChanges();

        // ---------- 3. 5 Bac si (moi BS gan 1 NguoiDung + 1 ChuyenKhoa) ----------
        var dsTaiKhoanBacSi = new List<NguoiDung>
        {
            new() { TenDangNhap = "bs.an",   MatKhau = "Bacsi@123", HoTen = "Nguyễn Văn An",    Email = "an.nguyen@datlichkhambenh.local",    SoDienThoai = "0911111111", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.binh", MatKhau = "Bacsi@123", HoTen = "Trần Thị Bình",    Email = "binh.tran@datlichkhambenh.local",    SoDienThoai = "0922222222", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.cuong",MatKhau = "Bacsi@123", HoTen = "Lê Văn Cường",     Email = "cuong.le@datlichkhambenh.local",     SoDienThoai = "0933333333", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.dung", MatKhau = "Bacsi@123", HoTen = "Phạm Thị Dung",    Email = "dung.pham@datlichkhambenh.local",    SoDienThoai = "0944444444", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.em",   MatKhau = "Bacsi@123", HoTen = "Hoàng Văn Em",     Email = "em.hoang@datlichkhambenh.local",     SoDienThoai = "0955555555", VaiTro = "BacSi" },
        };
        db.NguoiDungs.AddRange(dsTaiKhoanBacSi);
        db.SaveChanges();

        var dsBacSi = new List<BacSi>
        {
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[0].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[0].MaChuyenKhoa, // Noi
                HocVi = "Tiến sĩ - Bác sĩ",
                KinhNghiem = "15 năm kinh nghiệm tại BV Bạch Mai.",
                MoTa = "Chuyên về các bệnh lý tim mạch và tiêu hoá.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 300_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[1].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[1].MaChuyenKhoa, // Ngoai
                HocVi = "Thạc sĩ - Bác sĩ",
                KinhNghiem = "10 năm kinh nghiệm phẫu thuật tổng quát.",
                MoTa = "Phẫu thuật tiêu hoá, gan mật tuỵ, thoát vị.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 350_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[2].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[2].MaChuyenKhoa, // San
                HocVi = "Bác sĩ chuyên khoa II",
                KinhNghiem = "12 năm sản phụ khoa tại BV Từ Dũ.",
                MoTa = "Theo dõi thai kỳ, sản khoa nguy cơ cao.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 400_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[3].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[3].MaChuyenKhoa, // Nhi
                HocVi = "Bác sĩ chuyên khoa I",
                KinhNghiem = "8 năm khám nhi tại BV Nhi Trung ương.",
                MoTa = "Sơ sinh, nhi tổng quát, tư vấn dinh dưỡng trẻ.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 250_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[4].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[4].MaChuyenKhoa, // Mat
                HocVi = "Thạc sĩ - Bác sĩ",
                KinhNghiem = "9 năm tại BV Mắt Trung ương.",
                MoTa = "Khám khúc xạ, mổ Phaco, glaucoma.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 280_000m
            },
        };
        db.BacSis.AddRange(dsBacSi);

        // ---------- 4. 2 Benh nhan ----------
        var dsTaiKhoanBenhNhan = new List<NguoiDung>
        {
            new() { TenDangNhap = "bn.hoa",  MatKhau = "Benhnhan@123", HoTen = "Vũ Thị Hoa",   Email = "hoa.vu@gmail.com",  SoDienThoai = "0966666666", VaiTro = "BenhNhan" },
            new() { TenDangNhap = "bn.minh", MatKhau = "Benhnhan@123", HoTen = "Đỗ Văn Minh",  Email = "minh.do@gmail.com", SoDienThoai = "0977777777", VaiTro = "BenhNhan" },
        };
        db.NguoiDungs.AddRange(dsTaiKhoanBenhNhan);
        db.SaveChanges();

        var dsBenhNhan = new List<BenhNhan>
        {
            new()
            {
                MaNguoiDung = dsTaiKhoanBenhNhan[0].MaNguoiDung,
                NgaySinh = new DateTime(1995, 5, 12),
                GioiTinh = "Nữ",
                DiaChi = "123 Lê Lợi, Quận 1, TP.HCM"
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBenhNhan[1].MaNguoiDung,
                NgaySinh = new DateTime(1990, 8, 20),
                GioiTinh = "Nam",
                DiaChi = "456 Trần Hưng Đạo, Hai Bà Trưng, Hà Nội"
            },
        };
        db.BenhNhans.AddRange(dsBenhNhan);

        db.SaveChanges();
    }
}
