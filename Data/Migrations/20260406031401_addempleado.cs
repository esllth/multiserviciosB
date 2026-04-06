using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Data.Migrations
{
    /// <inheritdoc />
    public partial class addempleado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentificacionEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApellidosEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorreoElectronicoEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelefonoEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DireccionId = table.Column<int>(type: "int", nullable: true),
                    EstadoEmpleado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TieneUsuario = table.Column<bool>(type: "bit", nullable: false),
                    SalarioBase = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaInicioEmpleado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinalizacionEmpleado = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Empleados");
        }
    }
}
