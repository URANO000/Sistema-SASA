using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class IntegracionTabla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Archivo",
                table: "IntegracionHistorial",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PesoArchivo",
                table: "IntegracionHistorial",
                type: "BIGINT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TipoMime",
                table: "IntegracionHistorial",
                type: "varchar(100)",
                nullable: false,
                defaultValue: "");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Archivo",
                table: "IntegracionHistorial");

            migrationBuilder.DropColumn(
                name: "PesoArchivo",
                table: "IntegracionHistorial");

            migrationBuilder.DropColumn(
                name: "TipoMime",
                table: "IntegracionHistorial");

        }
    }
}
