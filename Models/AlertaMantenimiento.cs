using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa una alerta de mantenimiento preventivo de equipo (RE-009, RE-010)
    /// </summary>
    public class AlertaMantenimiento : BaseModel
    {
        [Key]
        public int IdAlerta { get; set; }

        [Required]
        public int EquipoId { get; set; }

        [Required]
        public DateTime FechaMantenimiento { get; set; }

        /// <summary>
        /// Tipo: "Preventivo", "Correctivo", "Calibración"
        /// </summary>
        [Required]
        [StringLength(50)]
        public string TipoMantenimiento { get; set; } = "";

        [StringLength(500)]
        public string? Descripcion { get; set; }

        /// <summary>
        /// Estado: "Pendiente", "Notificada", "Realizada", "Cancelada"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaNotificacion { get; set; }

        public DateTime? FechaRealizacion { get; set; }

        [ForeignKey(nameof(EquipoId))]
        public Equipo? Equipo { get; set; }
    }
}
