using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class MaterialProyectoFabricacionDTO
    {
        public int IdMaterialProyecto { get; set; }

        [Required(ErrorMessage = "El proyecto es requerido")]
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "El material es requerido")]
        public int MaterialId { get; set; }

        public string? NombreMaterial { get; set; }

        public string? UnidadMedida { get; set; }

        [Required(ErrorMessage = "La cantidad requerida es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadRequerida { get; set; }

        public decimal? CantidadUsada { get; set; }

        public string? Observaciones { get; set; }

        public decimal? PrecioUnitario { get; set; }

        public decimal? CostoTotal { get; set; }
    }
}
