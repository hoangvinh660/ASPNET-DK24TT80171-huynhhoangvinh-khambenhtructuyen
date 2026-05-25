using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatLichKhamBenh.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuyenKhoa",
                columns: table => new
                {
                    MaChuyenKhoa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuyenKhoa = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenKhoa", x => x.MaChuyenKhoa);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    VaiTro = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DaKhoa = table.Column<bool>(type: "bit", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaNguoiDung);
                });

            migrationBuilder.CreateTable(
                name: "BacSi",
                columns: table => new
                {
                    MaBacSi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaChuyenKhoa = table.Column<int>(type: "int", nullable: false),
                    HocVi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    KinhNghiem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HinhAnh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GiaKham = table.Column<decimal>(type: "decimal(18,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSi", x => x.MaBacSi);
                    table.ForeignKey(
                        name: "FK_BacSi_ChuyenKhoa_MaChuyenKhoa",
                        column: x => x.MaChuyenKhoa,
                        principalTable: "ChuyenKhoa",
                        principalColumn: "MaChuyenKhoa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BacSi_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhan",
                columns: table => new
                {
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NgaySinh = table.Column<DateTime>(type: "date", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhan", x => x.MaBenhNhan);
                    table.ForeignKey(
                        name: "FK_BenhNhan_NguoiDung_MaNguoiDung",
                        column: x => x.MaNguoiDung,
                        principalTable: "NguoiDung",
                        principalColumn: "MaNguoiDung",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LichHen",
                columns: table => new
                {
                    MaLichHen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaBenhNhan = table.Column<int>(type: "int", nullable: false),
                    MaBacSi = table.Column<int>(type: "int", nullable: false),
                    NgayKham = table.Column<DateTime>(type: "date", nullable: false),
                    GioKham = table.Column<TimeSpan>(type: "time", nullable: false),
                    LyDoKham = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHen", x => x.MaLichHen);
                    table.ForeignKey(
                        name: "FK_LichHen_BacSi_MaBacSi",
                        column: x => x.MaBacSi,
                        principalTable: "BacSi",
                        principalColumn: "MaBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_BenhNhan_MaBenhNhan",
                        column: x => x.MaBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "MaBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoBenhAn",
                columns: table => new
                {
                    MaHoSo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLichHen = table.Column<int>(type: "int", nullable: false),
                    ChanDoan = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DonThuoc = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    LoiKhuyen = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoBenhAn", x => x.MaHoSo);
                    table.ForeignKey(
                        name: "FK_HoSoBenhAn_LichHen_MaLichHen",
                        column: x => x.MaLichHen,
                        principalTable: "LichHen",
                        principalColumn: "MaLichHen",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacSi_MaChuyenKhoa",
                table: "BacSi",
                column: "MaChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_BacSi_MaNguoiDung",
                table: "BacSi",
                column: "MaNguoiDung",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenhNhan_MaNguoiDung",
                table: "BenhNhan",
                column: "MaNguoiDung",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoBenhAn_MaLichHen",
                table: "HoSoBenhAn",
                column: "MaLichHen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBacSi_NgayKham_GioKham",
                table: "LichHen",
                columns: new[] { "MaBacSi", "NgayKham", "GioKham" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBenhNhan",
                table: "LichHen",
                column: "MaBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_TenDangNhap",
                table: "NguoiDung",
                column: "TenDangNhap",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HoSoBenhAn");

            migrationBuilder.DropTable(
                name: "LichHen");

            migrationBuilder.DropTable(
                name: "BacSi");

            migrationBuilder.DropTable(
                name: "BenhNhan");

            migrationBuilder.DropTable(
                name: "ChuyenKhoa");

            migrationBuilder.DropTable(
                name: "NguoiDung");
        }
    }
}
