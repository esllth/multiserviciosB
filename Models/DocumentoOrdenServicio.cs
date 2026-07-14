using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Documento técnico asociado a una orden de servicio (REI-002)
    /// </summary>
    public class DocumentoOrdenServicio : BaseModel
    {
        [Key]
        public int IdDocumento { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreOriginal { get; set; } = "";

        [Required]
        [StringLength(260)]
        public string Ruta { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string TipoContenido { get; set; } = "";

        /// <summary>
        /// Tipo: "Informe", "Manual", "Plano", "Procedimiento", "Otro"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TipoDocumento { get; set; } = "Otro";

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public DateTime FechaCarga { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string? CargadoPorUsuarioId { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }
    }
}
