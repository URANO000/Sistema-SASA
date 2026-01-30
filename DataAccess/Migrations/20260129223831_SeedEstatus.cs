using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedEstatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Estatus",
                columns: new[] { "idEstatus", "nombreEstatus" },
                values: new object[,]
                {
                    { 1, "Abierto" },
                    { 2, "En Progreso" },
                    { 3, "Cancelado" },
                    { 4, "Resuelto" },
                    { 5, "Cerrado" }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Estatus",
                keyColumn: "idEstatus",
                keyValues: new object[] { 1, 2, 3, 4, 5 });
        }
    }
}
