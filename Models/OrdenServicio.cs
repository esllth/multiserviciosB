using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa una orden de servicio técnico (RT-001 a RT-010)
    /// </summary>
    public class OrdenServicio : BaseModel
    {
        [Key]
        public int IdOrden { get; set; }

        [Required]
        public int CotizacionId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        public int? EmpleadoId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaCompromiso { get; set; }

        public bool CompromisoConfirmado { get; set; } = false;

        public bool UsarDireccionPerfil { get; set; } = false;

        [StringLength(500)]
        public string? EnlaceWaze { get; set; }

        /// <summary>
        /// Fecha/hora de llegada del técnico al sitio (RT-001)
        /// </summary>
        public DateTime? FechaLlegadaSitio { get; set; }

        /// <summary>
        /// Fecha/hora de inicio efectivo del servicio (RT-002)
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha/hora de finalización del servicio (RT-006)
        /// </summary>
        public DateTime? FechaFin { get; set; }

        /// <summary>
        /// Fecha/hora de aceptación del cliente (RT-008)
        /// </summary>
        public DateTime? FechaAceptacionCliente { get; set; }

        [Required]
        public int EstadoOrdenId { get; set; }

        /// <summary>
        /// Observaciones técnicas generales (RT-003)
        /// </summary>
        [StringLength(2000)]
        public string? ObservacionesTecnicas { get; set; }

        /// <summary>
        /// Comentarios finales del técnico (RT-007)
        /// </summary>
        [StringLength(1000)]
        public string? ComentariosFinales { get; set; }

        /// <summary>
        /// Indica si se requieren fotos obligatorias para cierre (RT-004)
        /// </summary>
        public bool RequiereFotosObligatorias { get; set; } = true;

        /// <summary>
        /// Indica si el técnico confirmó la llegada con geolocalización (RT-001)
        /// </summary>
        public bool LlegadaConfirmada { get; set; } = false;

        [ForeignKey(nameof(CotizacionId))]
        public Cotizacion? Cotizacion { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public Empleado? Empleado { get; set; }

        [ForeignKey(nameof(EstadoOrdenId))]
        public EstadoOrden? EstadoOrden { get; set; }
    }
}
