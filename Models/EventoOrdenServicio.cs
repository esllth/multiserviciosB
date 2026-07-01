using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa el registro de eventos y observaciones de una orden de servicio (RT-001, RT-002, RT-003, RT-006, RT-007)
    /// </summary>
    [Table("EventoOrdenServicio")]
    public class EventoOrdenServicio : BaseModel
    {
        [Key]
        public int IdEvento { get; set; }

        [Required]
        public int OrdenId { get; set; }

        /// <summary>
        /// Tipo de evento: "LlegadaSitio", "InicioServicio", "ObservacionTecnica", "FinalizacionServicio", "ComentarioFinal", "AceptacionCliente"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TipoEvento { get; set; } = "";

        [Required]
        public DateTime FechaEvento { get; set; } = DateTime.Now;

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Latitud GPS (RT-001)
        /// </summary>
        [Column(TypeName = "decimal(10,7)")]
        public decimal? Latitud { get; set; }

        /// <summary>
        /// Longitud GPS (RT-001)
        /// </summary>
        [Column(TypeName = "decimal(10,7)")]
        public decimal? Longitud { get; set; }

        /// <summary>
        /// ID del usuario que registró el evento (técnico o cliente)
        /// </summary>
        [StringLength(450)]
        public string? UsuarioId { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }
    }
}
