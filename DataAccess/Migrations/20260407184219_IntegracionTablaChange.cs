using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IntegracionTablaChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PesoArchivo",
                table: "IntegracionHistorial",
                type: "BIGINT",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PesoArchivo",
                table: "IntegracionHistorial",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT");
        }
    }
}
