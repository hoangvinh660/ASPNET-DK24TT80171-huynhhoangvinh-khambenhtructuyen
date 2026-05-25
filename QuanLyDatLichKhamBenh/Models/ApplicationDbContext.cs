using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace QuanLyDatLichKhamBenh.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("QuanLyDatLichKhamBenh", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<ChuyenKhoa> ChuyenKhoa { get; set; }
        public DbSet<PhongKham> PhongKham { get; set; }
        public DbSet<BacSi> BacSi { get; set; }
        public DbSet<BenhNhan> BenhNhan { get; set; }
        public DbSet<LichLamViec> LichLamViec { get; set; }
        public DbSet<LichHen> LichHen { get; set; }
        public DbSet<HoSoBenhAn> HoSoBenhAn { get; set; }
        public DbSet<TinTuc> TinTuc { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // Phi kham: decimal(18,2)
            modelBuilder.Entity<BacSi>()
                .Property(b => b.PhiKham)
                .HasPrecision(18, 2);

            // BacSi / BenhNhan lien ket tai khoan (FK MaTaiKhoan tren BacSi/BenhNhan)
            modelBuilder.Entity<BacSi>()
                .HasOptional(b => b.TaiKhoan)
                .WithMany()
                .HasForeignKey(b => b.MaTaiKhoan);

            modelBuilder.Entity<BenhNhan>()
                .HasOptional(b => b.TaiKhoan)
                .WithMany()
                .HasForeignKey(b => b.MaTaiKhoan);

            // HoSoBenhAn - LichHen (1-1, FK MaLichHen tren HoSoBenhAn)
            modelBuilder.Entity<HoSoBenhAn>()
                .HasRequired(h => h.LichHen)
                .WithOptional(l => l.HoSoBenhAn)
                .HasForeignKey(h => h.MaLichHen);

            // Xoa cascade: khong xoa BacSi khi xoa ChuyenKhoa
            modelBuilder.Entity<BacSi>()
                .HasRequired(b => b.ChuyenKhoa)
                .WithMany(c => c.DanhSachBacSi)
                .HasForeignKey(b => b.MaChuyenKhoa)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<BacSi>()
                .HasRequired(b => b.PhongKham)
                .WithMany(p => p.DanhSachBacSi)
                .HasForeignKey(b => b.MaPhongKham)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LichLamViec>()
                .HasRequired(l => l.BacSi)
                .WithMany(b => b.DanhSachLichLamViec)
                .HasForeignKey(l => l.MaBacSi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LichHen>()
                .HasRequired(l => l.BenhNhan)
                .WithMany(b => b.DanhSachLichHen)
                .HasForeignKey(l => l.MaBenhNhan)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LichHen>()
                .HasRequired(l => l.BacSi)
                .WithMany(b => b.DanhSachLichHen)
                .HasForeignKey(l => l.MaBacSi)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<LichHen>()
                .HasRequired(l => l.LichLamViec)
                .WithMany(ll => ll.DanhSachLichHen)
                .HasForeignKey(l => l.MaLichLamViec)
                .WillCascadeOnDelete(false);
        }
    }
}