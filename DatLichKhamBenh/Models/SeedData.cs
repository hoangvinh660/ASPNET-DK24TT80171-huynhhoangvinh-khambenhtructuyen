using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Models;

// Nap du lieu mau cho he thong (chay 1 lan luc startup neu DB rong).
// Du lieu gom: 1 admin + 5 chuyen khoa + 5 bac si + 2 benh nhan.
public static class SeedData
{
    public static void Initialize(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Bao dam DB ton tai va co schema moi nhat
        db.Database.Migrate();

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
            new() { TenChuyenKhoa = "Noi tong quat", MoTa = "Kham va dieu tri cac benh ly noi khoa thong thuong." },
            new() { TenChuyenKhoa = "Ngoai tong quat", MoTa = "Phau thuat va dieu tri cac benh ly ngoai khoa." },
            new() { TenChuyenKhoa = "San phu khoa", MoTa = "Kham thai, san khoa va cac benh ly phu nu." },
            new() { TenChuyenKhoa = "Nhi khoa", MoTa = "Kham va dieu tri benh ly tre em duoi 15 tuoi." },
            new() { TenChuyenKhoa = "Mat", MoTa = "Kham, do mat va dieu tri cac benh ly ve mat." },
        };
        db.ChuyenKhoas.AddRange(dsChuyenKhoa);

        // Luu de co MaNguoiDung va MaChuyenKhoa
        db.SaveChanges();

        // ---------- 3. 5 Bac si (moi BS gan 1 NguoiDung + 1 ChuyenKhoa) ----------
        var dsTaiKhoanBacSi = new List<NguoiDung>
        {
            new() { TenDangNhap = "bs.an",  MatKhau = "Bacsi@123", HoTen = "Nguyen Van An",   Email = "an.nguyen@datlichkhambenh.local",   SoDienThoai = "0911111111", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.binh",MatKhau = "Bacsi@123", HoTen = "Tran Thi Binh",   Email = "binh.tran@datlichkhambenh.local",   SoDienThoai = "0922222222", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.cuong",MatKhau= "Bacsi@123", HoTen = "Le Van Cuong",    Email = "cuong.le@datlichkhambenh.local",    SoDienThoai = "0933333333", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.dung",MatKhau = "Bacsi@123", HoTen = "Pham Thi Dung",   Email = "dung.pham@datlichkhambenh.local",   SoDienThoai = "0944444444", VaiTro = "BacSi" },
            new() { TenDangNhap = "bs.em",  MatKhau = "Bacsi@123", HoTen = "Hoang Van Em",    Email = "em.hoang@datlichkhambenh.local",    SoDienThoai = "0955555555", VaiTro = "BacSi" },
        };
        db.NguoiDungs.AddRange(dsTaiKhoanBacSi);
        db.SaveChanges();

        var dsBacSi = new List<BacSi>
        {
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[0].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[0].MaChuyenKhoa, // Noi
                HocVi = "Tien si - Bac si",
                KinhNghiem = "15 nam kinh nghiem tai BV Bach Mai.",
                MoTa = "Chuyen ve cac benh ly tim mach va tieu hoa.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 300_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[1].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[1].MaChuyenKhoa, // Ngoai
                HocVi = "Thac si - Bac si",
                KinhNghiem = "10 nam kinh nghiem phau thuat tong quat.",
                MoTa = "Phau thuat tieu hoa, gan mat tuy, thoat vi.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 350_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[2].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[2].MaChuyenKhoa, // San
                HocVi = "Bac si chuyen khoa II",
                KinhNghiem = "12 nam san phu khoa tai BV Tu Du.",
                MoTa = "Theo doi thai ky, san khoa nguy co cao.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 400_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[3].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[3].MaChuyenKhoa, // Nhi
                HocVi = "Bac si chuyen khoa I",
                KinhNghiem = "8 nam kham nhi tai BV Nhi Trung uong.",
                MoTa = "So sinh, nhi tong quat, tu van dinh duong tre.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 250_000m
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBacSi[4].MaNguoiDung,
                MaChuyenKhoa = dsChuyenKhoa[4].MaChuyenKhoa, // Mat
                HocVi = "Thac si - Bac si",
                KinhNghiem = "9 nam tai BV Mat Trung uong.",
                MoTa = "Kham khuc xa, mo Phaco, glaucoma.",
                HinhAnh = "/images/bacsi/default.png",
                GiaKham = 280_000m
            },
        };
        db.BacSis.AddRange(dsBacSi);

        // ---------- 4. 2 Benh nhan ----------
        var dsTaiKhoanBenhNhan = new List<NguoiDung>
        {
            new() { TenDangNhap = "bn.hoa",  MatKhau = "Benhnhan@123", HoTen = "Vu Thi Hoa",  Email = "hoa.vu@gmail.com",  SoDienThoai = "0966666666", VaiTro = "BenhNhan" },
            new() { TenDangNhap = "bn.minh", MatKhau = "Benhnhan@123", HoTen = "Do Van Minh", Email = "minh.do@gmail.com", SoDienThoai = "0977777777", VaiTro = "BenhNhan" },
        };
        db.NguoiDungs.AddRange(dsTaiKhoanBenhNhan);
        db.SaveChanges();

        var dsBenhNhan = new List<BenhNhan>
        {
            new()
            {
                MaNguoiDung = dsTaiKhoanBenhNhan[0].MaNguoiDung,
                NgaySinh = new DateTime(1995, 5, 12),
                GioiTinh = "Nu",
                DiaChi = "123 Le Loi, Quan 1, TP HCM"
            },
            new()
            {
                MaNguoiDung = dsTaiKhoanBenhNhan[1].MaNguoiDung,
                NgaySinh = new DateTime(1990, 8, 20),
                GioiTinh = "Nam",
                DiaChi = "456 Tran Hung Dao, Hai Ba Trung, Ha Noi"
            },
        };
        db.BenhNhans.AddRange(dsBenhNhan);

        db.SaveChanges();
    }
}
