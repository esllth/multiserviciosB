using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class AgregarDireccionPerfilEnCompromisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Cotizaciones', 'UsarDireccionPerfil') IS NULL
                    ALTER TABLE dbo.Cotizaciones ADD UsarDireccionPerfil bit NOT NULL CONSTRAINT DF_Cotizaciones_UsarDireccionPerfil DEFAULT (0);

                IF COL_LENGTH('dbo.OrdenesServicio', 'UsarDireccionPerfil') IS NULL
                    ALTER TABLE dbo.OrdenesServicio ADD UsarDireccionPerfil bit NOT NULL CONSTRAINT DF_OrdenesServicio_UsarDireccionPerfil DEFAULT (0);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Cotizaciones', 'UsarDireccionPerfil') IS NOT NULL
                BEGIN
                    IF OBJECT_ID('dbo.DF_Cotizaciones_UsarDireccionPerfil', 'D') IS NOT NULL
                        ALTER TABLE dbo.Cotizaciones DROP CONSTRAINT DF_Cotizaciones_UsarDireccionPerfil;
                    ALTER TABLE dbo.Cotizaciones DROP COLUMN UsarDireccionPerfil;
                END;

                IF COL_LENGTH('dbo.OrdenesServicio', 'UsarDireccionPerfil') IS NOT NULL
                BEGIN
                    IF OBJECT_ID('dbo.DF_OrdenesServicio_UsarDireccionPerfil', 'D') IS NOT NULL
                        ALTER TABLE dbo.OrdenesServicio DROP CONSTRAINT DF_OrdenesServicio_UsarDireccionPerfil;
                    ALTER TABLE dbo.OrdenesServicio DROP COLUMN UsarDireccionPerfil;
                END;
                """);
        }
    }
}
