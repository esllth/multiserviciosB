using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MultiservicioB.Data;

#nullable disable

namespace MultiservicioB.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261122220000_BackfillEncuestasOrdenesCompletadas")]
    public partial class BackfillEncuestasOrdenesCompletadas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO [dbo].[Encuestas] ([OrdenId], [ClienteId])
                SELECT o.[IdOrden], o.[ClienteId]
                FROM [dbo].[OrdenesServicio] o
                INNER JOIN [dbo].[EstadoOrden] eo ON eo.[Id] = o.[EstadoOrdenId]
                WHERE eo.[Nombre] = N'Completada'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM [dbo].[Encuestas] e
                      WHERE e.[OrdenId] = o.[IdOrden]
                  );
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No se eliminan encuestas, porque podrían contener respuestas del cliente.
        }
    }
}
