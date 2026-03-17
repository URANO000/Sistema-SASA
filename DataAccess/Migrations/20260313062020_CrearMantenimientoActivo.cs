using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CrearMantenimientoActivo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MantenimientoActivo",
                columns: table => new
                {
                    IdMantenimiento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdActivo = table.Column<int>(type: "int", nullable: false),
                    FechaMantenimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoMantenimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MantenimientoActivo", x => x.IdMantenimiento);
                    table.ForeignKey(
                        name: "FK_MantenimientoActivo_ActivoInventario_IdActivo",
                        column: x => x.IdActivo,
                        principalTable: "ActivoInventario",
                        principalColumn: "IdActivo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MantenimientoActivo_IdActivo",
                table: "MantenimientoActivo",
                column: "IdActivo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MantenimientoActivo");
        }
    }
}
