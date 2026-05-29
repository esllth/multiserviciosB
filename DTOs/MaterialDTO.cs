using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class MaterialDTO
    {
        public int IdMaterial { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Unidad de Medida")]
        public string? UnidadMedida { get; set; }

        [Display(Name = "Stock Actual")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int? StockActual { get; set; }

        [Display(Name = "Stock Mínimo")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int? StockMinimo { get; set; }

        [Display(Name = "Precio Unitario")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio no puede ser negativo")]
        public decimal? PrecioUnitario { get; set; }

        public bool BajoStock => StockActual.HasValue && StockMinimo.HasValue && StockActual < StockMinimo;
    }
}
