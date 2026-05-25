namespace QuanLyDatLichKhamBenh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BacSi",
                c => new
                    {
                        MaBacSi = c.Int(nullable: false, identity: true),
                        HoTen = c.String(nullable: false, maxLength: 100),
                        MaChuyenKhoa = c.Int(nullable: false),
                        MaPhongKham = c.Int(nullable: false),
                        Email = c.String(maxLength: 100),
                        SoDienThoai = c.String(maxLength: 20),
                        HinhAnh = c.String(maxLength: 500),
                        MoTa = c.String(),
                        SoNamKinhNghiem = c.Int(nullable: false),
                        PhiKham = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaTaiKhoan = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaBacSi)
                .ForeignKey("dbo.ChuyenKhoa", t => t.MaChuyenKhoa)
                .ForeignKey("dbo.PhongKham", t => t.MaPhongKham)
                .ForeignKey("dbo.AspNetUsers", t => t.MaTaiKhoan)
                .Index(t => t.MaChuyenKhoa)
                .Index(t => t.MaPhongKham)
                .Index(t => t.MaTaiKhoan);
            
            CreateTable(
                "dbo.ChuyenKhoa",
                c => new
                    {
                        MaChuyenKhoa = c.Int(nullable: false, identity: true),
                        TenChuyenKhoa = c.String(nullable: false, maxLength: 200),
                        MoTa = c.String(),
                        HinhAnh = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.MaChuyenKhoa);
            
            CreateTable(
                "dbo.LichHen",
                c => new
                    {
                        MaLichHen = c.Int(nullable: false, identity: true),
                        MaBenhNhan = c.Int(nullable: false),
                        MaBacSi = c.Int(nullable: false),
                        MaLichLamViec = c.Int(nullable: false),
                        NgayHen = c.DateTime(nullable: false, storeType: "date"),
                        GioHen = c.Time(nullable: false, precision: 7),
                        TrieuChung = c.String(),
                        TrangThai = c.String(nullable: false, maxLength: 30),
                        NgayTao = c.DateTime(nullable: false),
                        GhiChu = c.String(),
                    })
                .PrimaryKey(t => t.MaLichHen)
                .ForeignKey("dbo.BacSi", t => t.MaBacSi)
                .ForeignKey("dbo.BenhNhan", t => t.MaBenhNhan)
                .ForeignKey("dbo.LichLamViec", t => t.MaLichLamViec)
                .Index(t => t.MaBenhNhan)
                .Index(t => t.MaBacSi)
                .Index(t => t.MaLichLamViec);
            
            CreateTable(
                "dbo.BenhNhan",
                c => new
                    {
                        MaBenhNhan = c.Int(nullable: false, identity: true),
                        HoTen = c.String(nullable: false, maxLength: 100),
                        NgaySinh = c.DateTime(nullable: false),
                        GioiTinh = c.String(nullable: false, maxLength: 10),
                        SoDienThoai = c.String(maxLength: 20),
                        DiaChi = c.String(maxLength: 300),
                        MaTaiKhoan = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.MaBenhNhan)
                .ForeignKey("dbo.AspNetUsers", t => t.MaTaiKhoan)
                .Index(t => t.MaTaiKhoan);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        HoTen = c.String(maxLength: 100),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.HoSoBenhAn",
                c => new
                    {
                        MaHoSo = c.Int(nullable: false, identity: true),
                        MaLichHen = c.Int(nullable: false),
                        ChanDoan = c.String(),
                        DonThuoc = c.String(),
                        LoiDan = c.String(),
                        NgayTao = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MaHoSo)
                .ForeignKey("dbo.LichHen", t => t.MaHoSo)
                .Index(t => t.MaHoSo)
                .Index(t => t.MaLichHen, unique: true);
            
            CreateTable(
                "dbo.LichLamViec",
                c => new
                    {
                        MaLichLamViec = c.Int(nullable: false, identity: true),
                        MaBacSi = c.Int(nullable: false),
                        NgayLamViec = c.DateTime(nullable: false, storeType: "date"),
                        GioBatDau = c.Time(nullable: false, precision: 7),
                        GioKetThuc = c.Time(nullable: false, precision: 7),
                        ConTrong = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MaLichLamViec)
                .ForeignKey("dbo.BacSi", t => t.MaBacSi)
                .Index(t => t.MaBacSi);
            
            CreateTable(
                "dbo.PhongKham",
                c => new
                    {
                        MaPhongKham = c.Int(nullable: false, identity: true),
                        TenPhongKham = c.String(nullable: false, maxLength: 200),
                        DiaChi = c.String(maxLength: 300),
                        SoDienThoai = c.String(maxLength: 20),
                        MoTa = c.String(),
                    })
                .PrimaryKey(t => t.MaPhongKham);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.TinTuc",
                c => new
                    {
                        MaTinTuc = c.Int(nullable: false, identity: true),
                        TieuDe = c.String(nullable: false, maxLength: 300),
                        NoiDung = c.String(nullable: false),
                        HinhAnh = c.String(maxLength: 500),
                        NgayDang = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.MaTinTuc);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.BacSi", "MaTaiKhoan", "dbo.AspNetUsers");
            DropForeignKey("dbo.BacSi", "MaPhongKham", "dbo.PhongKham");
            DropForeignKey("dbo.LichHen", "MaLichLamViec", "dbo.LichLamViec");
            DropForeignKey("dbo.LichLamViec", "MaBacSi", "dbo.BacSi");
            DropForeignKey("dbo.HoSoBenhAn", "MaHoSo", "dbo.LichHen");
            DropForeignKey("dbo.LichHen", "MaBenhNhan", "dbo.BenhNhan");
            DropForeignKey("dbo.BenhNhan", "MaTaiKhoan", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.LichHen", "MaBacSi", "dbo.BacSi");
            DropForeignKey("dbo.BacSi", "MaChuyenKhoa", "dbo.ChuyenKhoa");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.LichLamViec", new[] { "MaBacSi" });
            DropIndex("dbo.HoSoBenhAn", new[] { "MaLichHen" });
            DropIndex("dbo.HoSoBenhAn", new[] { "MaHoSo" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.BenhNhan", new[] { "MaTaiKhoan" });
            DropIndex("dbo.LichHen", new[] { "MaLichLamViec" });
            DropIndex("dbo.LichHen", new[] { "MaBacSi" });
            DropIndex("dbo.LichHen", new[] { "MaBenhNhan" });
            DropIndex("dbo.BacSi", new[] { "MaTaiKhoan" });
            DropIndex("dbo.BacSi", new[] { "MaPhongKham" });
            DropIndex("dbo.BacSi", new[] { "MaChuyenKhoa" });
            DropTable("dbo.TinTuc");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.PhongKham");
            DropTable("dbo.LichLamViec");
            DropTable("dbo.HoSoBenhAn");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.BenhNhan");
            DropTable("dbo.LichHen");
            DropTable("dbo.ChuyenKhoa");
            DropTable("dbo.BacSi");
        }
    }
}
