using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaActivoTelefono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivoTelefono",
                columns: table => new
                {
                    IdTelefono = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreColaborador = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Departamento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Operador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NumeroCelular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CorreoSistemasAnaliticos = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IMEI = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Cargador = table.Column<bool>(type: "bit", nullable: false),
                    Auriculares = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivoTelefono", x => x.IdTelefono);
                });

            migrationBuilder.CreateIndex(
                name: "UX_ActivoTelefono_IMEI",
                table: "ActivoTelefono",
                column: "IMEI",
                unique: true,
                filter: "[IMEI] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivoTelefono");
        }
    }
}