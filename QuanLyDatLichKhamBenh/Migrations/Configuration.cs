namespace QuanLyDatLichKhamBenh.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using QuanLyDatLichKhamBenh.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            TaoVaiTro(roleManager, VaiTro.QuanTriVien);
            TaoVaiTro(roleManager, VaiTro.BacSi);
            TaoVaiTro(roleManager, VaiTro.BenhNhan);

            if (!context.ChuyenKhoa.Any())
            {
                var chuyenKhoas = new[]
                {
                    new ChuyenKhoa { TenChuyenKhoa = "Noi tong quat", MoTa = "Kham va dieu tri benh noi khoa tong quat." },
                    new ChuyenKhoa { TenChuyenKhoa = "Nhi khoa", MoTa = "Cham soc suc khoe tre em." },
                    new ChuyenKhoa { TenChuyenKhoa = "Da lieu", MoTa = "Dieu tri cac benh ve da." },
                    new ChuyenKhoa { TenChuyenKhoa = "Tai mui hong", MoTa = "Kham tai mui hong." },
                    new ChuyenKhoa { TenChuyenKhoa = "Tim mach", MoTa = "Kham tim mach." }
                };
                context.ChuyenKhoa.AddRange(chuyenKhoas);
                context.SaveChanges();
            }

            if (!context.PhongKham.Any())
            {
                context.PhongKham.AddRange(new[]
                {
                    new PhongKham
                    {
                        TenPhongKham = "Phong kham An Khang",
                        DiaChi = "123 Nguyen Hue, Q1, TP.HCM",
                        SoDienThoai = "02838223344",
                        MoTa = "Phong kham da khoa."
                    },
                    new PhongKham
                    {
                        TenPhongKham = "Phong kham Binh An",
                        DiaChi = "456 Le Loi, Q3, TP.HCM",
                        SoDienThoai = "02839334455",
                        MoTa = "Phong kham chuyen sau."
                    }
                });
                context.SaveChanges();
            }

            var ckList = context.ChuyenKhoa.OrderBy(c => c.MaChuyenKhoa).ToList();
            var pkList = context.PhongKham.OrderBy(p => p.MaPhongKham).ToList();

            TaoTaiKhoan(userManager, roleManager, "admin@gmail.com", "Admin@123", VaiTro.QuanTriVien, "Quan tri vien");

            string bacSiUserId = TaoTaiKhoan(userManager, roleManager, "bacsi1@gmail.com", "Bacsi@123", VaiTro.BacSi, "BS. Nguyen Van An");
            string benhNhanUserId = TaoTaiKhoan(userManager, roleManager, "benhnhan1@gmail.com", "Benhnhan@123", VaiTro.BenhNhan, "Tran Thi Mai");

            if (!context.BacSi.Any())
            {
                var tenBacSi = new[] { "BS. Nguyen Van An", "BS. Le Thi Binh", "BS. Pham Van Cuong", "BS. Hoang Thi Dung", "BS. Vo Van Em" };
                for (int i = 0; i < tenBacSi.Length && i < ckList.Count; i++)
                {
                    var bs = new BacSi
                    {
                        HoTen = tenBacSi[i],
                        MaChuyenKhoa = ckList[i].MaChuyenKhoa,
                        MaPhongKham = pkList[i % pkList.Count].MaPhongKham,
                        Email = i == 0 ? "bacsi1@gmail.com" : ("bacsi" + (i + 1) + "@gmail.com"),
                        SoDienThoai = "090100000" + i,
                        SoNamKinhNghiem = 5 + i,
                        PhiKham = 150000 + (i * 50000),
                        MoTa = "Bac si chuyen khoa " + ckList[i].TenChuyenKhoa
                    };
                    if (i == 0 && !string.IsNullOrEmpty(bacSiUserId))
                        bs.MaTaiKhoan = bacSiUserId;
                    context.BacSi.Add(bs);
                }
                context.SaveChanges();
            }

            if (!context.BenhNhan.Any())
            {
                context.BenhNhan.AddRange(new[]
                {
                    new BenhNhan
                    {
                        HoTen = "Tran Thi Mai",
                        NgaySinh = new DateTime(1995, 5, 15),
                        GioiTinh = GioiTinh.Nu,
                        SoDienThoai = "0912345678",
                        DiaChi = "Q1, TP.HCM",
                        MaTaiKhoan = benhNhanUserId
                    },
                    new BenhNhan
                    {
                        HoTen = "Nguyen Van Hung",
                        NgaySinh = new DateTime(1988, 8, 20),
                        GioiTinh = GioiTinh.Nam,
                        SoDienThoai = "0923456789",
                        DiaChi = "Q3, TP.HCM"
                    },
                    new BenhNhan
                    {
                        HoTen = "Le Thi Hoa",
                        NgaySinh = new DateTime(2000, 1, 10),
                        GioiTinh = GioiTinh.Nu,
                        SoDienThoai = "0934567890",
                        DiaChi = "Q7, TP.HCM"
                    }
                });
                context.SaveChanges();
            }

            var bacSiList = context.BacSi.OrderBy(b => b.MaBacSi).ToList();
            if (!context.LichLamViec.Any())
            {
                foreach (var bs in bacSiList)
                {
                    for (int d = 1; d <= 7; d++)
                    {
                        context.LichLamViec.Add(new LichLamViec
                        {
                            MaBacSi = bs.MaBacSi,
                            NgayLamViec = DateTime.Today.AddDays(d),
                            GioBatDau = new TimeSpan(8, 0, 0),
                            GioKetThuc = new TimeSpan(11, 30, 0),
                            ConTrong = true
                        });
                        context.LichLamViec.Add(new LichLamViec
                        {
                            MaBacSi = bs.MaBacSi,
                            NgayLamViec = DateTime.Today.AddDays(d),
                            GioBatDau = new TimeSpan(14, 0, 0),
                            GioKetThuc = new TimeSpan(17, 0, 0),
                            ConTrong = true
                        });
                    }
                }
                context.SaveChanges();
            }

            var benhNhanList = context.BenhNhan.OrderBy(b => b.MaBenhNhan).ToList();
            var lichLamList = context.LichLamViec.OrderBy(l => l.MaLichLamViec).ToList();

            if (!context.LichHen.Any() && benhNhanList.Any() && bacSiList.Any() && lichLamList.Any())
            {
                var bn = benhNhanList.First();
                var trangThais = new[]
                {
                    TrangThaiLichHen.ChoXacNhan,
                    TrangThaiLichHen.DaXacNhan,
                    TrangThaiLichHen.DaHuy,
                    TrangThaiLichHen.DaKham,
                    TrangThaiLichHen.ChoXacNhan
                };

                for (int i = 0; i < trangThais.Length; i++)
                {
                    var llv = lichLamList[i % lichLamList.Count];
                    var lh = new LichHen
                    {
                        MaBenhNhan = bn.MaBenhNhan,
                        MaBacSi = llv.MaBacSi,
                        MaLichLamViec = llv.MaLichLamViec,
                        NgayHen = llv.NgayLamViec,
                        GioHen = llv.GioBatDau,
                        TrieuChung = "Trieu chung mau " + (i + 1),
                        TrangThai = trangThais[i],
                        NgayTao = DateTime.Now.AddDays(-i),
                        GhiChu = "Lich hen mau"
                    };
                    context.LichHen.Add(lh);
                    context.SaveChanges();

                    if (lh.TrangThai == TrangThaiLichHen.DaKham)
                    {
                        context.HoSoBenhAn.Add(new HoSoBenhAn
                        {
                            MaLichHen = lh.MaLichHen,
                            ChanDoan = "Cam cum",
                            DonThuoc = "Paracetamol 500mg x 10 vien",
                            LoiDan = "Uong thuoc dung lieu luong, nghi ngoi.",
                            NgayTao = DateTime.Now
                        });
                        llv.ConTrong = false;
                    }
                    else if (lh.TrangThai == TrangThaiLichHen.DaXacNhan || lh.TrangThai == TrangThaiLichHen.ChoXacNhan)
                    {
                        llv.ConTrong = false;
                    }
                }
                context.SaveChanges();
            }

            if (!context.TinTuc.Any())
            {
                context.TinTuc.AddRange(new[]
                {
                    new TinTuc
                    {
                        TieuDe = "Huong dan dat lich kham truc tuyen",
                        NoiDung = "Benh nhan co the dat lich qua website trong 4 buoc don gian.",
                        NgayDang = DateTime.Now.AddDays(-3)
                    },
                    new TinTuc
                    {
                        TieuDe = "Luu y khi kham benh",
                        NoiDung = "Mang the BHYT va CMND khi den phong kham.",
                        NgayDang = DateTime.Now.AddDays(-2)
                    },
                    new TinTuc
                    {
                        TieuDe = "Mo rong gio kham cuoi tuan",
                        NoiDung = "Phong kham mo them ca thu 7 sang.",
                        NgayDang = DateTime.Now.AddDays(-1)
                    }
                });
                context.SaveChanges();
            }
        }

        private static void TaoVaiTro(RoleManager<IdentityRole> roleManager, string tenVaiTro)
        {
            if (!roleManager.RoleExists(tenVaiTro))
                roleManager.Create(new IdentityRole(tenVaiTro));
        }

        private static string TaoTaiKhoan(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            string email,
            string matKhau,
            string vaiTro,
            string hoTen)
        {
            var user = userManager.FindByName(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    HoTen = hoTen
                };
                var result = userManager.Create(user, matKhau);
                if (!result.Succeeded)
                    return null;
            }

            if (!userManager.IsInRole(user.Id, vaiTro))
                userManager.AddToRole(user.Id, vaiTro);

            return user.Id;
        }
    }
}
