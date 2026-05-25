using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatLichKhamBenh.Migrations
{
    /// <inheritdoc />
    public partial class AddCauHinhEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CauHinhEmail",
                columns: table => new
                {
                    MaCauHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatEmail = table.Column<bool>(type: "bit", nullable: false),
                    SmtpServer = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Port = table.Column<int>(type: "int", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SenderPassword = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CauHinhEmail", x => x.MaCauHinh);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CauHinhEmail");
        }
    }
}
