using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class FotoCotizacion
    {
        [Key]
        public int IdFotoCotizacion { get; set; }

        public int CotizacionId { get; set; }

        [Required]
        [StringLength(260)]
        public string Ruta { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string NombreOriginal { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string TipoContenido { get; set; } = "";

        public DateTime FechaCarga { get; set; }

        [ForeignKey(nameof(CotizacionId))]
        public Cotizacion? Cotizacion { get; set; }
    }
}
