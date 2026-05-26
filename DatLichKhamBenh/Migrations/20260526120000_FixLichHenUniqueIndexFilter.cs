using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatLichKhamBenh.Migrations
{
    /// <inheritdoc />
    public partial class FixLichHenUniqueIndexFilter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHen_MaBacSi_NgayKham_GioKham",
                table: "LichHen");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBacSi_NgayKham_GioKham",
                table: "LichHen",
                columns: new[] { "MaBacSi", "NgayKham", "GioKham" },
                unique: true,
                filter: "[TrangThai] <> 'DaHuy'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHen_MaBacSi_NgayKham_GioKham",
                table: "LichHen");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaBacSi_NgayKham_GioKham",
                table: "LichHen",
                columns: new[] { "MaBacSi", "NgayKham", "GioKham" },
                unique: true);
        }
    }
}
