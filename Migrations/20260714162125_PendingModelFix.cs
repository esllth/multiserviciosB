using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <summary>
    /// Retains the historical migration ID without replaying the schema changes
    /// already applied by REI002_DocumentosOrdenServicio immediately before it.
    /// </summary>
    public partial class PendingModelFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: REI002 contains the identical Up operations.
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty: REI002 remains responsible for reverting them.
        }
    }
}
