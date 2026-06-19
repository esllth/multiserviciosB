using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiservicioB.Migrations
{
    /// <inheritdoc />
    public partial class ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Direcciones_DireccionId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumosMaterial_Materiales_MaterialId",
                table: "ConsumosMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumosMaterial_OrdenesServicio_OrdenId",
                table: "ConsumosMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_Direcciones_UbicacionDTA_UbicacionDTAId",
                table: "Direcciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Direcciones",
                table: "Direcciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsumosMaterial",
                table: "ConsumosMaterial");

            migrationBuilder.RenameTable(
                name: "Direcciones",
                newName: "Direccion");

            migrationBuilder.RenameTable(
                name: "ConsumosMaterial",
                newName: "ConsumoMaterial");

            migrationBuilder.RenameIndex(
                name: "IX_Direcciones_UbicacionDTAId",
                table: "Direccion",
                newName: "IX_Direccion_UbicacionDTAId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsumosMaterial_OrdenId",
                table: "ConsumoMaterial",
                newName: "IX_ConsumoMaterial_OrdenId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsumosMaterial_MaterialId",
                table: "ConsumoMaterial",
                newName: "IX_ConsumoMaterial_MaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Direccion",
                table: "Direccion",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsumoMaterial",
                table: "ConsumoMaterial",
                column: "IdConsumo");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Direccion_DireccionId",
                table: "Clientes",
                column: "DireccionId",
                principalTable: "Direccion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumoMaterial_Materiales_MaterialId",
                table: "ConsumoMaterial",
                column: "MaterialId",
                principalTable: "Materiales",
                principalColumn: "IdMaterial",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumoMaterial_OrdenesServicio_OrdenId",
                table: "ConsumoMaterial",
                column: "OrdenId",
                principalTable: "OrdenesServicio",
                principalColumn: "IdOrden",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Direccion_UbicacionDTA_UbicacionDTAId",
                table: "Direccion",
                column: "UbicacionDTAId",
                principalTable: "UbicacionDTA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clientes_Direccion_DireccionId",
                table: "Clientes");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumoMaterial_Materiales_MaterialId",
                table: "ConsumoMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_ConsumoMaterial_OrdenesServicio_OrdenId",
                table: "ConsumoMaterial");

            migrationBuilder.DropForeignKey(
                name: "FK_Direccion_UbicacionDTA_UbicacionDTAId",
                table: "Direccion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Direccion",
                table: "Direccion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConsumoMaterial",
                table: "ConsumoMaterial");

            migrationBuilder.RenameTable(
                name: "Direccion",
                newName: "Direcciones");

            migrationBuilder.RenameTable(
                name: "ConsumoMaterial",
                newName: "ConsumosMaterial");

            migrationBuilder.RenameIndex(
                name: "IX_Direccion_UbicacionDTAId",
                table: "Direcciones",
                newName: "IX_Direcciones_UbicacionDTAId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsumoMaterial_OrdenId",
                table: "ConsumosMaterial",
                newName: "IX_ConsumosMaterial_OrdenId");

            migrationBuilder.RenameIndex(
                name: "IX_ConsumoMaterial_MaterialId",
                table: "ConsumosMaterial",
                newName: "IX_ConsumosMaterial_MaterialId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Direcciones",
                table: "Direcciones",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConsumosMaterial",
                table: "ConsumosMaterial",
                column: "IdConsumo");

            migrationBuilder.AddForeignKey(
                name: "FK_Clientes_Direcciones_DireccionId",
                table: "Clientes",
                column: "DireccionId",
                principalTable: "Direcciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumosMaterial_Materiales_MaterialId",
                table: "ConsumosMaterial",
                column: "MaterialId",
                principalTable: "Materiales",
                principalColumn: "IdMaterial",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ConsumosMaterial_OrdenesServicio_OrdenId",
                table: "ConsumosMaterial",
                column: "OrdenId",
                principalTable: "OrdenesServicio",
                principalColumn: "IdOrden",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Direcciones_UbicacionDTA_UbicacionDTAId",
                table: "Direcciones",
                column: "UbicacionDTAId",
                principalTable: "UbicacionDTA",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
