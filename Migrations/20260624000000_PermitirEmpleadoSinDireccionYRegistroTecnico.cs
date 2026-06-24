using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MultiservicioB.Data;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico")]
    public partial class PermitirEmpleadoSinDireccionYRegistroTecnico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NOT NULL
                    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Direccion];
                """);

            migrationBuilder.AlterColumn<int>(
                name: "DireccionId",
                table: "Empleados",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.Sql("""
                UPDATE [Empleados]
                SET [DireccionId] = NULL
                WHERE [DireccionId] = 0;

                IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NULL
                   AND OBJECT_ID(N'[dbo].[Direccion]', N'U') IS NOT NULL
                    ALTER TABLE [dbo].[Empleados] WITH CHECK
                    ADD CONSTRAINT [FK_Empleados_Direccion]
                    FOREIGN KEY ([DireccionId]) REFERENCES [dbo].[Direccion] ([Id]);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NOT NULL
                    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Direccion];
                """);

            migrationBuilder.Sql("UPDATE [Empleados] SET [DireccionId] = 0 WHERE [DireccionId] IS NULL");

            migrationBuilder.AlterColumn<int>(
                name: "DireccionId",
                table: "Empleados",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
