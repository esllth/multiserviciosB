using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class AgregarAdelantoWazeYPagoCotizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Cotizaciones', 'RequiereAdelanto') IS NULL
                    ALTER TABLE dbo.Cotizaciones ADD RequiereAdelanto bit NOT NULL CONSTRAINT DF_Cotizaciones_RequiereAdelanto DEFAULT (0);

                IF COL_LENGTH('dbo.Cotizaciones', 'PorcentajeAdelanto') IS NULL
                    ALTER TABLE dbo.Cotizaciones ADD PorcentajeAdelanto int NULL;

                IF COL_LENGTH('dbo.Cotizaciones', 'EnlaceWaze') IS NULL
                    ALTER TABLE dbo.Cotizaciones ADD EnlaceWaze nvarchar(500) NULL;

                IF COL_LENGTH('dbo.Cotizaciones', 'FormaPagoAceptada') IS NULL
                    ALTER TABLE dbo.Cotizaciones ADD FormaPagoAceptada nvarchar(40) NULL;

                IF COL_LENGTH('dbo.OrdenesServicio', 'EnlaceWaze') IS NULL
                    ALTER TABLE dbo.OrdenesServicio ADD EnlaceWaze nvarchar(500) NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.Cotizaciones', 'RequiereAdelanto') IS NOT NULL
                BEGIN
                    IF OBJECT_ID('dbo.DF_Cotizaciones_RequiereAdelanto', 'D') IS NOT NULL
                        ALTER TABLE dbo.Cotizaciones DROP CONSTRAINT DF_Cotizaciones_RequiereAdelanto;
                    ALTER TABLE dbo.Cotizaciones DROP COLUMN RequiereAdelanto;
                END;

                IF COL_LENGTH('dbo.Cotizaciones', 'PorcentajeAdelanto') IS NOT NULL
                    ALTER TABLE dbo.Cotizaciones DROP COLUMN PorcentajeAdelanto;

                IF COL_LENGTH('dbo.Cotizaciones', 'EnlaceWaze') IS NOT NULL
                    ALTER TABLE dbo.Cotizaciones DROP COLUMN EnlaceWaze;

                IF COL_LENGTH('dbo.Cotizaciones', 'FormaPagoAceptada') IS NOT NULL
                    ALTER TABLE dbo.Cotizaciones DROP COLUMN FormaPagoAceptada;

                IF COL_LENGTH('dbo.OrdenesServicio', 'EnlaceWaze') IS NOT NULL
                    ALTER TABLE dbo.OrdenesServicio DROP COLUMN EnlaceWaze;
                """);
        }
    }
}
