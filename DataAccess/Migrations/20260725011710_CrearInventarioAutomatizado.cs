using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CrearInventarioAutomatizado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventarioAutomatizado",
                columns: table => new
                {
                    IdInventarioAutomatizado = table.Column<int>(
                        type: "int",
                        nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),

                    NombreUsuario = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    NombreEquipo = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    Marca = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    Modelo = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    SerialPC = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    ModeloProcesador = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),

                    Mhz = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false),

                    SistemaOperativo = table.Column<string>(
                        type: "nvarchar(150)",
                        maxLength: 150,
                        nullable: false),

                    BuildNumber = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false),

                    TotalRam = table.Column<double>(
                        type: "float",
                        nullable: false),

                    SlotsRam = table.Column<int>(
                        type: "int",
                        nullable: false),

                    SlotDetalle = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    ModeloAlmacenamiento = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false),

                    AlmacenamientoDetalle = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    Redes = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    RedDetalle = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    CantMonitores = table.Column<int>(
                        type: "int",
                        nullable: false),

                    Monitores = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    MonitorDetalle = table.Column<string>(
                        type: "nvarchar(max)",
                        nullable: false),

                    Tiene7 = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    Tiene6 = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    TieneKonica = table.Column<bool>(
                        type: "bit",
                        nullable: false),

                    FechaRegistro = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false),

                    FechaActualizacion = table.Column<DateTime>(
                        type: "datetime2",
                        nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey(
                        "PK_InventarioAutomatizado",
                        x => x.IdInventarioAutomatizado);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventarioAutomatizado");
        }
    }
}