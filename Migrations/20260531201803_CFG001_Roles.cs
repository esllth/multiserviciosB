using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class CFG001_Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Empleados",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EstadosCotizacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosCotizacion", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EstadosOrden",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstadosOrden", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Materiales",
                columns: table => new
                {
                    IdMaterial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnidadMedida = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    StockActual = table.Column<int>(type: "int", nullable: true),
                    StockMinimo = table.Column<int>(type: "int", nullable: true),
                    PrecioUnitario = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materiales", x => x.IdMaterial);
                });

            migrationBuilder.CreateTable(
                name: "TiposServicio",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposServicio", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UbicacionDTA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdProvincia = table.Column<int>(type: "int", nullable: false),
                    Provincia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdCanton = table.Column<int>(type: "int", nullable: false),
                    Canton = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdDistrito = table.Column<int>(type: "int", nullable: false),
                    Distrito = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CodigoDTA = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UbicacionDTA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Direcciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UbicacionDTAId = table.Column<int>(type: "int", nullable: false),
                    OtrasSenas = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Direcciones_UbicacionDTA_UbicacionDTAId",
                        column: x => x.UbicacionDTAId,
                        principalTable: "UbicacionDTA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    IdCliente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Identificacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    DireccionId = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.IdCliente);
                    table.ForeignKey(
                        name: "FK_Clientes_Direcciones_DireccionId",
                        column: x => x.DireccionId,
                        principalTable: "Direcciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cotizaciones",
                columns: table => new
                {
                    IdCotizacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    TipoServicioId = table.Column<int>(type: "int", nullable: false),
                    EstadoCotizacionId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MontoPresupuesto = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AprobadaPorCliente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cotizaciones", x => x.IdCotizacion);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_EstadosCotizacion_EstadoCotizacionId",
                        column: x => x.EstadoCotizacionId,
                        principalTable: "EstadosCotizacion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cotizaciones_TiposServicio_TipoServicioId",
                        column: x => x.TipoServicioId,
                        principalTable: "TiposServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Equipos",
                columns: table => new
                {
                    IdEquipo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Especificaciones = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipos", x => x.IdEquipo);
                    table.ForeignKey(
                        name: "FK_Equipos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectosFabricacion",
                columns: table => new
                {
                    IdProyecto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectosFabricacion", x => x.IdProyecto);
                    table.ForeignKey(
                        name: "FK_ProyectosFabricacion_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrdenesServicio",
                columns: table => new
                {
                    IdOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CotizacionId = table.Column<int>(type: "int", nullable: false),
                    ClienteId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoOrdenId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdenesServicio", x => x.IdOrden);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Cotizaciones_CotizacionId",
                        column: x => x.CotizacionId,
                        principalTable: "Cotizaciones",
                        principalColumn: "IdCotizacion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "IdEmpleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrdenesServicio_EstadosOrden_EstadoOrdenId",
                        column: x => x.EstadoOrdenId,
                        principalTable: "EstadosOrden",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConsumosMaterial",
                columns: table => new
                {
                    IdConsumo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    CantidadUsada = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumosMaterial", x => x.IdConsumo);
                    table.ForeignKey(
                        name: "FK_ConsumosMaterial_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "IdMaterial",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ConsumosMaterial_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HistorialEquipos",
                columns: table => new
                {
                    IdHistorial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipoId = table.Column<int>(type: "int", nullable: false),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    FechaServicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorialEquipos", x => x.IdHistorial);
                    table.ForeignKey(
                        name: "FK_HistorialEquipos_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "IdEquipo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HistorialEquipos_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_UserId",
                table: "Empleados",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clientes_DireccionId",
                table: "Clientes",
                column: "DireccionId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosMaterial_MaterialId",
                table: "ConsumosMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsumosMaterial_OrdenId",
                table: "ConsumosMaterial",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_ClienteId",
                table: "Cotizaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_EstadoCotizacionId",
                table: "Cotizaciones",
                column: "EstadoCotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_TipoServicioId",
                table: "Cotizaciones",
                column: "TipoServicioId");

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_UbicacionDTAId",
                table: "Direcciones",
                column: "UbicacionDTAId");

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_ClienteId",
                table: "Equipos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEquipos_EquipoId",
                table: "HistorialEquipos",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_HistorialEquipos_OrdenId",
                table: "HistorialEquipos",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_ClienteId",
                table: "OrdenesServicio",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_CotizacionId",
                table: "OrdenesServicio",
                column: "CotizacionId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_EmpleadoId",
                table: "OrdenesServicio",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_EstadoOrdenId",
                table: "OrdenesServicio",
                column: "EstadoOrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectosFabricacion_ClienteId",
                table: "ProyectosFabricacion",
                column: "ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Empleados_AspNetUsers_UserId",
                table: "Empleados",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleados_AspNetUsers_UserId",
                table: "Empleados");

            migrationBuilder.DropTable(
                name: "ConsumosMaterial");

            migrationBuilder.DropTable(
                name: "HistorialEquipos");

            migrationBuilder.DropTable(
                name: "ProyectosFabricacion");

            migrationBuilder.DropTable(
                name: "Materiales");

            migrationBuilder.DropTable(
                name: "Equipos");

            migrationBuilder.DropTable(
                name: "OrdenesServicio");

            migrationBuilder.DropTable(
                name: "Cotizaciones");

            migrationBuilder.DropTable(
                name: "EstadosOrden");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "EstadosCotizacion");

            migrationBuilder.DropTable(
                name: "TiposServicio");

            migrationBuilder.DropTable(
                name: "Direcciones");

            migrationBuilder.DropTable(
                name: "UbicacionDTA");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_UserId",
                table: "Empleados");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Empleados",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
