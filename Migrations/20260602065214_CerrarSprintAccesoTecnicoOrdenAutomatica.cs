using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class CerrarSprintAccesoTecnicoOrdenAutomatica : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_CotizacionId",
                table: "OrdenesServicio");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "OrdenesServicio",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CorreoElectronicoEmpleado",
                table: "Empleados",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "EstadoAcceso",
                table: "Empleados",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "PendienteRegistro");

            migrationBuilder.Sql(
                "UPDATE [Empleados] SET [EstadoAcceso] = CASE WHEN [TieneUsuario] = 1 THEN N'Aprobado' ELSE N'PendienteRegistro' END");

            migrationBuilder.Sql(
                "IF EXISTS (SELECT [CotizacionId] FROM [OrdenesServicio] GROUP BY [CotizacionId] HAVING COUNT(*) > 1) THROW 50001, 'Existen cotizaciones con más de una orden de servicio. Corrija los duplicados antes de aplicar la migración.', 1;");

            migrationBuilder.Sql(
                "IF EXISTS (SELECT [CorreoElectronicoEmpleado] FROM [Empleados] GROUP BY [CorreoElectronicoEmpleado] HAVING COUNT(*) > 1) THROW 50002, 'Existen empleados con correo duplicado. Corrija los duplicados antes de aplicar la migración.', 1;");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_CotizacionId",
                table: "OrdenesServicio",
                column: "CotizacionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_CorreoElectronicoEmpleado",
                table: "Empleados",
                column: "CorreoElectronicoEmpleado",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrdenesServicio_CotizacionId",
                table: "OrdenesServicio");

            migrationBuilder.DropIndex(
                name: "IX_Empleados_CorreoElectronicoEmpleado",
                table: "Empleados");

            migrationBuilder.DropColumn(
                name: "EstadoAcceso",
                table: "Empleados");

            migrationBuilder.AlterColumn<int>(
                name: "EmpleadoId",
                table: "OrdenesServicio",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CorreoElectronicoEmpleado",
                table: "Empleados",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_OrdenesServicio_CotizacionId",
                table: "OrdenesServicio",
                column: "CotizacionId");
        }
    }
}
