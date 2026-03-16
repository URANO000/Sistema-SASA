using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CrearActivoInventarioTiquete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivoInventarioTiquete",
                columns: table => new
                {
                    IdActivoInventarioTiquete = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdActivo = table.Column<int>(type: "int", nullable: false),
                    IdTiquete = table.Column<int>(type: "int", nullable: false),
                    FechaAsociacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivoInventarioTiquete", x => x.IdActivoInventarioTiquete);
                    table.ForeignKey(
                        name: "FK_ActivoInventarioTiquete_ActivoInventario_IdActivo",
                        column: x => x.IdActivo,
                        principalTable: "ActivoInventario",
                        principalColumn: "IdActivo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ActivoInventarioTiquete_Tiquete_IdTiquete",
                        column: x => x.IdTiquete,
                        principalTable: "Tiquete",
                        principalColumn: "idTiquete",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivoInventarioTiquete_IdActivo_IdTiquete",
                table: "ActivoInventarioTiquete",
                columns: new[] { "IdActivo", "IdTiquete" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ActivoInventarioTiquete_IdTiquete",
                table: "ActivoInventarioTiquete",
                column: "IdTiquete");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivoInventarioTiquete");
        }
    }
}