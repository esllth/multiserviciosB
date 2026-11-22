using Microsoft.EntityFrameworkCore.Migrations;
using MultiservicioB.Data;

#nullable disable

namespace MultiservicioB.Migrations
{
    [Migration("20261121235000_AgregarRevisionYContactoCotizacion")]
    public partial class AgregarRevisionYContactoCotizacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComentarioRevisionAdministrador",
                table: "Cotizaciones",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ContactoCliente",
                table: "Cotizaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComentarioRevisionAdministrador",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "ContactoCliente",
                table: "Cotizaciones");
        }
    }
}
