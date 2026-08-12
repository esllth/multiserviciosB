using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class MaterialDTO
    {
        public int IdMaterial { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [Display(Name = "Código")]
        [StringLength(50)]
        public string? Codigo { get; set; }

        [Required(ErrorMessage = "La categoría es obligatoria")]
        [Display(Name = "Categoría")]
        [StringLength(100)]
        public string? Categoria { get; set; }

        [Display(Name = "Descripción")]
        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Display(Name = "Unidad de Medida")]
        [StringLength(50)]
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

        [Display(Name = "Alerta de Stock Activa")]
        public bool AlertaStockActiva { get; set; } = true;

        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Activo";

        public bool BajoStock => StockActual.HasValue && StockMinimo.HasValue && StockActual < StockMinimo;

        public bool EnStockCritico => StockActual.HasValue && StockMinimo.HasValue && StockActual <= (StockMinimo * 0.5m);

        public int TotalOrdenesUtilizado { get; set; }
        public decimal CantidadTotalUtilizada { get; set; }
        public List<int> UltimasOrdenesUtilizadas { get; set; } = new();
    }
}
