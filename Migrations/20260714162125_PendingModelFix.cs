using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class PendingModelFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FechaInicio",
                table: "ProyectosFabricacion",
                newName: "FechaInicioReal");

            migrationBuilder.RenameColumn(
                name: "FechaFin",
                table: "ProyectosFabricacion",
                newName: "FechaInicioEstimada");

            migrationBuilder.AddColumn<bool>(
                name: "RequiereVisita",
                table: "TipoServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "ProyectosFabricacion",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ProyectosFabricacion",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoEstimado",
                table: "ProyectosFabricacion",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CostoReal",
                table: "ProyectosFabricacion",
                type: "decimal(12,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DiseñoAprobado",
                table: "ProyectosFabricacion",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAprobacionDiseño",
                table: "ProyectosFabricacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinEstimada",
                table: "ProyectosFabricacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFinReal",
                table: "ProyectosFabricacion",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaSolicitud",
                table: "ProyectosFabricacion",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "NombreProyecto",
                table: "ProyectosFabricacion",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesCliente",
                table: "ProyectosFabricacion",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesInternas",
                table: "ProyectosFabricacion",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComentariosFinales",
                table: "OrdenesServicio",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CompromisoConfirmado",
                table: "OrdenesServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "EnlaceWaze",
                table: "OrdenesServicio",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAceptacionCliente",
                table: "OrdenesServicio",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaCompromiso",
                table: "OrdenesServicio",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaLlegadaSitio",
                table: "OrdenesServicio",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "LlegadaConfirmada",
                table: "OrdenesServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesTecnicas",
                table: "OrdenesServicio",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereFotosObligatorias",
                table: "OrdenesServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsarDireccionPerfil",
                table: "OrdenesServicio",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Materiales",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AlertaStockActiva",
                table: "Materiales",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "Materiales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Materiales",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Estado",
                table: "Materiales",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "HistorialEquipos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoAnterior",
                table: "HistorialEquipos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoPosterior",
                table: "HistorialEquipos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ObservacionesTecnico",
                table: "HistorialEquipos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoServicio",
                table: "HistorialEquipos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Equipos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Equipos",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Especificaciones",
                table: "Equipos",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Codigo",
                table: "Equipos",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAdquisicion",
                table: "Equipos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FrecuenciaMantenimientoDias",
                table: "Equipos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Marca",
                table: "Equipos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Modelo",
                table: "Equipos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NumeroSerie",
                table: "Equipos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Observaciones",
                table: "Equipos",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProximoMantenimiento",
                table: "Equipos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoEquipo",
                table: "Equipos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UltimoMantenimiento",
                table: "Equipos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnlaceWaze",
                table: "Cotizaciones",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVisitaSolicitada",
                table: "Cotizaciones",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormaPagoAceptada",
                table: "Cotizaciones",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PorcentajeAdelanto",
                table: "Cotizaciones",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiereAdelanto",
                table: "Cotizaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UsarDireccionPerfil",
                table: "Cotizaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRegistro",
                table: "ConsumoMaterial",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "AlertasMantenimiento",
                columns: table => new
                {
                    IdAlerta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EquipoId = table.Column<int>(type: "int", nullable: false),
                    FechaMantenimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TipoMantenimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaNotificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaRealizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertasMantenimiento", x => x.IdAlerta);
                    table.ForeignKey(
                        name: "FK_AlertasMantenimiento_Equipos_EquipoId",
                        column: x => x.EquipoId,
                        principalTable: "Equipos",
                        principalColumn: "IdEquipo",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosFabricacion",
                columns: table => new
                {
                    IdDocumento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    NombreDocumento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TipoDocumento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CargadoPorUsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosFabricacion", x => x.IdDocumento);
                    table.ForeignKey(
                        name: "FK_DocumentosFabricacion_ProyectosFabricacion_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "ProyectosFabricacion",
                        principalColumn: "IdProyecto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventoOrdenServicio",
                columns: table => new
                {
                    IdEvento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    TipoEvento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitud = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitud = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventoOrdenServicio", x => x.IdEvento);
                    table.ForeignKey(
                        name: "FK_EventoOrdenServicio_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FotoOrden",
                columns: table => new
                {
                    IdFotoOrden = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    Ruta = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    NombreOriginal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoContenido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TipoFoto = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCarga = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FotoOrden", x => x.IdFotoOrden);
                    table.ForeignKey(
                        name: "FK_FotoOrden_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MaterialesProyectoFabricacion",
                columns: table => new
                {
                    IdMaterialProyecto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProyectoId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    CantidadRequerida = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CantidadUsada = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialesProyectoFabricacion", x => x.IdMaterialProyecto);
                    table.ForeignKey(
                        name: "FK_MaterialesProyectoFabricacion_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "IdMaterial",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MaterialesProyectoFabricacion_ProyectosFabricacion_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "ProyectosFabricacion",
                        principalColumn: "IdProyecto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notificaciones",
                columns: table => new
                {
                    IdNotificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: true),
                    ClienteId = table.Column<int>(type: "int", nullable: true),
                    MaterialId = table.Column<int>(type: "int", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mensaje = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Leida = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificaciones", x => x.IdNotificacion);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "IdCliente",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificaciones_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "IdMaterial",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notificaciones_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesMaterial",
                columns: table => new
                {
                    IdSolicitud = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrdenId = table.Column<int>(type: "int", nullable: false),
                    MaterialId = table.Column<int>(type: "int", nullable: false),
                    EmpleadoId = table.Column<int>(type: "int", nullable: false),
                    CantidadSolicitada = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    FechaSolicitud = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Justificacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RespuestaAdmin = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaRespuesta = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesMaterial", x => x.IdSolicitud);
                    table.ForeignKey(
                        name: "FK_SolicitudesMaterial_Empleados_EmpleadoId",
                        column: x => x.EmpleadoId,
                        principalTable: "Empleados",
                        principalColumn: "IdEmpleado",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesMaterial_Materiales_MaterialId",
                        column: x => x.MaterialId,
                        principalTable: "Materiales",
                        principalColumn: "IdMaterial",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SolicitudesMaterial_OrdenesServicio_OrdenId",
                        column: x => x.OrdenId,
                        principalTable: "OrdenesServicio",
                        principalColumn: "IdOrden",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlertasMantenimiento_EquipoId",
                table: "AlertasMantenimiento",
                column: "EquipoId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosFabricacion_ProyectoId",
                table: "DocumentosFabricacion",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_EventoOrdenServicio_OrdenId",
                table: "EventoOrdenServicio",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_FotoOrden_OrdenId",
                table: "FotoOrden",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialesProyectoFabricacion_MaterialId",
                table: "MaterialesProyectoFabricacion",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_MaterialesProyectoFabricacion_ProyectoId",
                table: "MaterialesProyectoFabricacion",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_ClienteId",
                table: "Notificaciones",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_MaterialId",
                table: "Notificaciones",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificaciones_OrdenId",
                table: "Notificaciones",
                column: "OrdenId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesMaterial_EmpleadoId",
                table: "SolicitudesMaterial",
                column: "EmpleadoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesMaterial_MaterialId",
                table: "SolicitudesMaterial",
                column: "MaterialId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesMaterial_OrdenId",
                table: "SolicitudesMaterial",
                column: "OrdenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlertasMantenimiento");

            migrationBuilder.DropTable(
                name: "DocumentosFabricacion");

            migrationBuilder.DropTable(
                name: "EventoOrdenServicio");

            migrationBuilder.DropTable(
                name: "FotoOrden");

            migrationBuilder.DropTable(
                name: "MaterialesProyectoFabricacion");

            migrationBuilder.DropTable(
                name: "Notificaciones");

            migrationBuilder.DropTable(
                name: "SolicitudesMaterial");

            migrationBuilder.DropColumn(
                name: "RequiereVisita",
                table: "TipoServicio");

            migrationBuilder.DropColumn(
                name: "CostoEstimado",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "CostoReal",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "DiseñoAprobado",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "FechaAprobacionDiseño",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "FechaFinEstimada",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "FechaFinReal",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "FechaSolicitud",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "NombreProyecto",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "ObservacionesCliente",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "ObservacionesInternas",
                table: "ProyectosFabricacion");

            migrationBuilder.DropColumn(
                name: "ComentariosFinales",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "CompromisoConfirmado",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "EnlaceWaze",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FechaAceptacionCliente",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FechaCompromiso",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "FechaLlegadaSitio",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "LlegadaConfirmada",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "ObservacionesTecnicas",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "RequiereFotosObligatorias",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "UsarDireccionPerfil",
                table: "OrdenesServicio");

            migrationBuilder.DropColumn(
                name: "AlertaStockActiva",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "Estado",
                table: "Materiales");

            migrationBuilder.DropColumn(
                name: "EstadoAnterior",
                table: "HistorialEquipos");

            migrationBuilder.DropColumn(
                name: "EstadoPosterior",
                table: "HistorialEquipos");

            migrationBuilder.DropColumn(
                name: "ObservacionesTecnico",
                table: "HistorialEquipos");

            migrationBuilder.DropColumn(
                name: "TipoServicio",
                table: "HistorialEquipos");

            migrationBuilder.DropColumn(
                name: "Codigo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "FechaAdquisicion",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "FrecuenciaMantenimientoDias",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Marca",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Modelo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "NumeroSerie",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "Observaciones",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "ProximoMantenimiento",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "TipoEquipo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "UltimoMantenimiento",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "EnlaceWaze",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FechaVisitaSolicitada",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FormaPagoAceptada",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "PorcentajeAdelanto",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "RequiereAdelanto",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "UsarDireccionPerfil",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "FechaRegistro",
                table: "ConsumoMaterial");

            migrationBuilder.RenameColumn(
                name: "FechaInicioReal",
                table: "ProyectosFabricacion",
                newName: "FechaInicio");

            migrationBuilder.RenameColumn(
                name: "FechaInicioEstimada",
                table: "ProyectosFabricacion",
                newName: "FechaFin");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "ProyectosFabricacion",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "ProyectosFabricacion",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "Materiales",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descripcion",
                table: "HistorialEquipos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Equipos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Equipos",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Especificaciones",
                table: "Equipos",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);
        }
    }
}
