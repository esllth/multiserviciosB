using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa el historial de servicios y mantenimientos de equipos (RE-005, RE-007)
    /// </summary>
    public class HistorialEquipo : BaseModel
    {
        [Key]
        public int IdHistorial { get; set; }

        [Required]
        public int EquipoId { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [Required]
        public DateTime FechaServicio { get; set; }

        /// <summary>
        /// Tipo de servicio: "Preventivo", "Correctivo", "Instalacion", "Calibracion", "Otro"
        /// </summary>
        [StringLength(50)]
        public string? TipoServicio { get; set; }

        [StringLength(2000)]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Estado del equipo antes del servicio
        /// </summary>
        [StringLength(30)]
        public string? EstadoAnterior { get; set; }

        /// <summary>
        /// Estado del equipo después del servicio (RE-007)
        /// </summary>
        [StringLength(30)]
        public string? EstadoPosterior { get; set; }

        [StringLength(1000)]
        public string? ObservacionesTecnico { get; set; }

        [ForeignKey(nameof(EquipoId))]
        public Equipo? Equipo { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }
    }
}
