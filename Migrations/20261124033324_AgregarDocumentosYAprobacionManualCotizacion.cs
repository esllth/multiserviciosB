using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDocumentosYAprobacionManualCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DocumentoCotizacionId",
                table: "DocumentosOrdenServicio",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsEvidenciaContractual",
                table: "DocumentosOrdenServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "AprobadaPorUsuarioId",
                table: "Cotizaciones",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacion",
                table: "Cotizaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRechazo",
                table: "Cotizaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazo",
                table: "Cotizaciones",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RechazadaPorUsuarioId",
                table: "Cotizaciones",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DocumentosCotizacion",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    NombreOriginal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    TipoContenido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    HashSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    TamanoBytes = table.Column<long>(type: "bigint", nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CargadoPorUsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    EsReemplazable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosCotizacion", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_DocumentosCotizacion_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "IdCotizacion",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosCotizacion_CotizacionId_TipoDocumento",
                table: "DocumentosCotizacion",
                columns: new[] { "CotizacionId", "TipoDocumento" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentosCotizacion");

            migrationBuilder.DropColumn(
                name: "DocumentoCotizacionId",
                table: "DocumentosOrdenServicio");

            migrationBuilder.DropColumn(
                name: "EsEvidenciaContractual",
                table: "DocumentosOrdenServicio");

            migrationBuilder.DropColumn(
                name: "AprobadaPorUsuarioId",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FechaAprobacion",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FechaRechazo",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "MotivoRechazo",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "RechazadaPorUsuarioId",
                table: "Cotizaciones");
        }
    }
}
