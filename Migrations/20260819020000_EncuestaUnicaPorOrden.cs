using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MultiservicioB.Data;

#nullable disable

namespace MultiservicioB.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260819020000_EncuestaUnicaPorOrden")]
    public partial class EncuestaUnicaPorOrden : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID(N'dbo.Encuestas', N'U') IS NULL
                BEGIN
                    CREATE TABLE [dbo].[Encuestas] (
                        [IdEncuesta] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        [OrdenId] INT NOT NULL,
                        [ClienteId] INT NOT NULL,
                        [CalificacionServicio] INT NULL,
                        [CalificacionTecnico] INT NULL,
                        [Comentarios] NVARCHAR(255) NULL,
                        [Fecha] DATE NULL,
                        CONSTRAINT [CK_Encuestas_CalificacionServicio] CHECK ([CalificacionServicio] BETWEEN 1 AND 5),
                        CONSTRAINT [CK_Encuestas_CalificacionTecnico] CHECK ([CalificacionTecnico] BETWEEN 1 AND 5),
                        CONSTRAINT [FK_Encuestas_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden]),
                        CONSTRAINT [FK_Encuestas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente])
                    );
                END;

                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Encuestas_OrdenId' AND object_id = OBJECT_ID(N'dbo.Encuestas'))
                    CREATE UNIQUE INDEX [IX_Encuestas_OrdenId] ON [dbo].[Encuestas] ([OrdenId]);
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Encuestas_OrdenId' AND object_id = OBJECT_ID(N'dbo.Encuestas'))
                    DROP INDEX [IX_Encuestas_OrdenId] ON [dbo].[Encuestas];
                """);
        }
    }
}
