using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModulosCampoMaterialesEquiposFabricacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_EstadosCotizacion_EstadoCotizacionId",
                table: "Cotizaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_TiposServicio_TipoServicioId",
                table: "Cotizaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_EstadosOrden_EstadoOrdenId",
                table: "OrdenesServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TiposServicio",
                table: "TiposServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadosOrden",
                table: "EstadosOrden");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadosCotizacion",
                table: "EstadosCotizacion");

            migrationBuilder.RenameTable(
                name: "TiposServicio",
                newName: "TipoServicio");

            migrationBuilder.RenameTable(
                name: "EstadosOrden",
                newName: "EstadoOrden");

            migrationBuilder.RenameTable(
                name: "EstadosCotizacion",
                newName: "EstadoCotizacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TipoServicio",
                table: "TipoServicio",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoOrden",
                table: "EstadoOrden",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadoCotizacion",
                table: "EstadoCotizacion",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ConfiguracionSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Horarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiaSemana = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HoraInicio = table.Column<TimeSpan>(type: "time", nullable: false),
                    HoraFin = table.Column<TimeSpan>(type: "time", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Horarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zonas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Provincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Canton = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zonas", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_EstadoCotizacion_EstadoCotizacionId",
                table: "Cotizaciones",
                column: "EstadoCotizacionId",
                principalTable: "EstadoCotizacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_TipoServicio_TipoServicioId",
                table: "Cotizaciones",
                column: "TipoServicioId",
                principalTable: "TipoServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_EstadoOrden_EstadoOrdenId",
                table: "OrdenesServicio",
                column: "EstadoOrdenId",
                principalTable: "EstadoOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_EstadoCotizacion_EstadoCotizacionId",
                table: "Cotizaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_TipoServicio_TipoServicioId",
                table: "Cotizaciones");

            migrationBuilder.DropForeignKey(
                name: "FK_OrdenesServicio_EstadoOrden_EstadoOrdenId",
                table: "OrdenesServicio");

            migrationBuilder.DropTable(
                name: "ConfiguracionSistema");

            migrationBuilder.DropTable(
                name: "Horarios");

            migrationBuilder.DropTable(
                name: "Zonas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TipoServicio",
                table: "TipoServicio");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoOrden",
                table: "EstadoOrden");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstadoCotizacion",
                table: "EstadoCotizacion");

            migrationBuilder.RenameTable(
                name: "TipoServicio",
                newName: "TiposServicio");

            migrationBuilder.RenameTable(
                name: "EstadoOrden",
                newName: "EstadosOrden");

            migrationBuilder.RenameTable(
                name: "EstadoCotizacion",
                newName: "EstadosCotizacion");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TiposServicio",
                table: "TiposServicio",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadosOrden",
                table: "EstadosOrden",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstadosCotizacion",
                table: "EstadosCotizacion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_EstadosCotizacion_EstadoCotizacionId",
                table: "Cotizaciones",
                column: "EstadoCotizacionId",
                principalTable: "EstadosCotizacion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_TiposServicio_TipoServicioId",
                table: "Cotizaciones",
                column: "TipoServicioId",
                principalTable: "TiposServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrdenesServicio_EstadosOrden_EstadoOrdenId",
                table: "OrdenesServicio",
                column: "EstadoOrdenId",
                principalTable: "EstadosOrden",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
