using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa un material del inventario (RM-001 a RM-010)
    /// </summary>
    public class Material : BaseModel
    {
        [Key]
        public int IdMaterial { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [StringLength(50)]
        public string? Codigo { get; set; }

        [StringLength(100)]
        public string? Categoria { get; set; }

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string? UnidadMedida { get; set; }

        public int? StockActual { get; set; }

        /// <summary>
        /// Stock mínimo para generar alerta (RM-008)
        /// </summary>
        public int? StockMinimo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioUnitario { get; set; }

        /// <summary>
        /// Indica si el material genera alertas de stock (RM-008)
        /// </summary>
        public bool AlertaStockActiva { get; set; } = true;

        /// <summary>
        /// Indica el estado del material: "Activo", "Inactivo", "Descontinuado"
        /// </summary>
        [StringLength(20)]
        public string Estado { get; set; } = "Activo";
    }
}
