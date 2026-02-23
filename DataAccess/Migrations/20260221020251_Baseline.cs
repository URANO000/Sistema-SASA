using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.InsertData(
                table: "Estatus",
                columns: new[] { "idEstatus", "nombreEstatus" },
                values: new object[,]
                {
                    { 1, "Creado" },
                    { 2, "En Proceso" },
                    { 3, "En Espera Del Usuario" },
                    { 4, "Cancelado" },
                    { 5, "Resuelto" }
                });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
