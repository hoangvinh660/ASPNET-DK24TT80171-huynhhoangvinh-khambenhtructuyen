using Microsoft.EntityFrameworkCore;

namespace DatLichKhamBenh.Models;

// DbContext chinh cua he thong
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<NguoiDung> NguoiDungs => Set<NguoiDung>();
    public DbSet<ChuyenKhoa> ChuyenKhoas => Set<ChuyenKhoa>();
    public DbSet<BacSi> BacSis => Set<BacSi>();
    public DbSet<BenhNhan> BenhNhans => Set<BenhNhan>();
    public DbSet<LichHen> LichHens => Set<LichHen>();
    public DbSet<HoSoBenhAn> HoSoBenhAns => Set<HoSoBenhAn>();
    public DbSet<CauHinhEmail> CauHinhEmails => Set<CauHinhEmail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TenDangNhap phai la duy nhat
        modelBuilder.Entity<NguoiDung>()
            .HasIndex(u => u.TenDangNhap)
            .IsUnique();

        modelBuilder.Entity<NguoiDung>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // NguoiDung 1-1 BacSi (1 tai khoan ung voi toi da 1 ho so bac si)
        modelBuilder.Entity<BacSi>()
            .HasOne(b => b.NguoiDung)
            .WithOne(u => u.BacSi)
            .HasForeignKey<BacSi>(b => b.MaNguoiDung)
            .OnDelete(DeleteBehavior.Cascade);

        // NguoiDung 1-1 BenhNhan
        modelBuilder.Entity<BenhNhan>()
            .HasOne(b => b.NguoiDung)
            .WithOne(u => u.BenhNhan)
            .HasForeignKey<BenhNhan>(b => b.MaNguoiDung)
            .OnDelete(DeleteBehavior.Cascade);

        // ChuyenKhoa 1-N BacSi - KHONG xoa day chuyen de tranh xoa nham bac si
        modelBuilder.Entity<BacSi>()
            .HasOne(b => b.ChuyenKhoa)
            .WithMany(ck => ck.DanhSachBacSi)
            .HasForeignKey(b => b.MaChuyenKhoa)
            .OnDelete(DeleteBehavior.Restrict);

        // BenhNhan 1-N LichHen - KHONG cascade (tranh multiple cascade paths gay loi SQL Server)
        modelBuilder.Entity<LichHen>()
            .HasOne(l => l.BenhNhan)
            .WithMany(bn => bn.DanhSachLichHen)
            .HasForeignKey(l => l.MaBenhNhan)
            .OnDelete(DeleteBehavior.Restrict);

        // BacSi 1-N LichHen - KHONG cascade
        modelBuilder.Entity<LichHen>()
            .HasOne(l => l.BacSi)
            .WithMany(bs => bs.DanhSachLichHen)
            .HasForeignKey(l => l.MaBacSi)
            .OnDelete(DeleteBehavior.Restrict);

        // LichHen 1-1 HoSoBenhAn
        modelBuilder.Entity<HoSoBenhAn>()
            .HasOne(h => h.LichHen)
            .WithOne(l => l.HoSoBenhAn)
            .HasForeignKey<HoSoBenhAn>(h => h.MaLichHen)
            .OnDelete(DeleteBehavior.Cascade);

        // 1 bac si khong the co 2 lich hen trung cung ngay cung gio (bo qua lich da huy)
        modelBuilder.Entity<LichHen>()
            .HasIndex(l => new { l.MaBacSi, l.NgayKham, l.GioKham })
            .IsUnique()
            .HasFilter("[TrangThai] <> 'DaHuy'");
    }
}
