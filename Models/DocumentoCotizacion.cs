using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class DocumentoCotizacion : BaseModel
    {
        [Key]
        public int IdDocumento { get; set; }

        [Required]
        public int CotizacionId { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreOriginal { get; set; } = "";

        [Required]
        [StringLength(260)]
        public string Ruta { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string TipoContenido { get; set; } = "application/pdf";

        [Required]
        [StringLength(40)]
        public string TipoDocumento { get; set; } = "CotizacionFirmada";

        [Required]
        [StringLength(64)]
        public string HashSha256 { get; set; } = "";

        public long TamanoBytes { get; set; }

        public DateTime FechaCarga { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string CargadoPorUsuarioId { get; set; } = "";

        public bool EsReemplazable { get; set; } = true;

        [ForeignKey(nameof(CotizacionId))]
        public Cotizacion? Cotizacion { get; set; }
    }
}
