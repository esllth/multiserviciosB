using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa las evidencias fotográficas de una orden de servicio (RT-004)
    /// </summary>
    [Table("FotoOrden")]
    public class FotoOrdenServicio : BaseModel
    {
        [Key]
        public int IdFotoOrden { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [Required]
        [StringLength(260)]
        public string Ruta { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string NombreOriginal { get; set; } = "";

        [Required]
        [StringLength(50)]
        public string TipoContenido { get; set; } = "";

        /// <summary>
        /// Tipo de foto: "Inicial" o "Final"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string TipoFoto { get; set; } = "";

        public DateTime FechaCarga { get; set; } = DateTime.Now;

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }
    }
}
