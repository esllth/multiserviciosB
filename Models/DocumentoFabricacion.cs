using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa documentos de diseño para proyectos de fabricación (RF-002, RF-008)
    /// </summary>
    public class DocumentoFabricacion : BaseModel
    {
        [Key]
        public int IdDocumento { get; set; }

        [Required]
        public int ProyectoId { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreDocumento { get; set; } = "";

        /// <summary>
        /// Tipo: "Diseño", "Especificaciones", "Plano", "Otro"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TipoDocumento { get; set; } = "";

        [Required]
        [StringLength(260)]
        public string Ruta { get; set; } = "";

        [StringLength(500)]
        public string? Descripcion { get; set; }

        public DateTime FechaCarga { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string? CargadoPorUsuarioId { get; set; }

        [ForeignKey(nameof(ProyectoId))]
        public ProyectoFabricacion? Proyecto { get; set; }
    }
}
