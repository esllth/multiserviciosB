using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa un proyecto de fabricación a medida (RF-001 a RF-010)
    /// </summary>
    public class ProyectoFabricacion : BaseModel
    {
        [Key]
        public int IdProyecto { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreProyecto { get; set; } = "";

        [StringLength(1000)]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Estado: "Pendiente", "EnDiseño", "CotizacionPendiente", "Aprobado", "EnProduccion", "Finalizado", "Cancelado"
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        public DateTime? FechaInicioEstimada { get; set; }

        public DateTime? FechaFinEstimada { get; set; }

        public DateTime? FechaInicioReal { get; set; }

        public DateTime? FechaFinReal { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? CostoEstimado { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? CostoReal { get; set; }

        /// <summary>
        /// Indica si el cliente aprobó el diseño y cotización (RF-007)
        /// </summary>
        public bool DiseñoAprobado { get; set; } = false;

        public DateTime? FechaAprobacionDiseño { get; set; }

        [StringLength(1000)]
        public string? ObservacionesCliente { get; set; }

        [StringLength(1000)]
        public string? ObservacionesInternas { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }
    }
}
