using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Material : BaseModel
    {
        [Key]
        public int IdMaterial { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        [StringLength(50)]
        public string? UnidadMedida { get; set; }

        public int? StockActual { get; set; }

        public int? StockMinimo { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PrecioUnitario { get; set; }
    }
}
