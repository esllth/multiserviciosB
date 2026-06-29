using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCompromisosYVisitas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.TipoServicio', 'RequiereVisita') IS NULL
                    ALTER TABLE dbo.TipoServicio ADD RequiereVisita bit NOT NULL CONSTRAINT DF_TipoServicio_RequiereVisita DEFAULT (0);

                IF COL_LENGTH('dbo.Cotizaciones', 'FechaVisitaSolicitada') IS NULL
                    ALTER TABLE dbo.Cotizaciones ADD FechaVisitaSolicitada datetime2 NULL;

                IF COL_LENGTH('dbo.OrdenesServicio', 'FechaCompromiso') IS NULL
                    ALTER TABLE dbo.OrdenesServicio ADD FechaCompromiso datetime2 NULL;

                IF COL_LENGTH('dbo.OrdenesServicio', 'CompromisoConfirmado') IS NULL
                    ALTER TABLE dbo.OrdenesServicio ADD CompromisoConfirmado bit NOT NULL CONSTRAINT DF_OrdenesServicio_CompromisoConfirmado DEFAULT (0);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF COL_LENGTH('dbo.TipoServicio', 'RequiereVisita') IS NOT NULL
                BEGIN
                    IF OBJECT_ID('dbo.DF_TipoServicio_RequiereVisita', 'D') IS NOT NULL
                        ALTER TABLE dbo.TipoServicio DROP CONSTRAINT DF_TipoServicio_RequiereVisita;
                    ALTER TABLE dbo.TipoServicio DROP COLUMN RequiereVisita;
                END;

                IF COL_LENGTH('dbo.Cotizaciones', 'FechaVisitaSolicitada') IS NOT NULL
                    ALTER TABLE dbo.Cotizaciones DROP COLUMN FechaVisitaSolicitada;

                IF COL_LENGTH('dbo.OrdenesServicio', 'FechaCompromiso') IS NOT NULL
                    ALTER TABLE dbo.OrdenesServicio DROP COLUMN FechaCompromiso;

                IF COL_LENGTH('dbo.OrdenesServicio', 'CompromisoConfirmado') IS NOT NULL
                BEGIN
                    IF OBJECT_ID('dbo.DF_OrdenesServicio_CompromisoConfirmado', 'D') IS NOT NULL
                        ALTER TABLE dbo.OrdenesServicio DROP CONSTRAINT DF_OrdenesServicio_CompromisoConfirmado;
                    ALTER TABLE dbo.OrdenesServicio DROP COLUMN CompromisoConfirmado;
                END;
                """);
        }
    }
}
