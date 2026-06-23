using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa solicitudes de materiales faltantes en campo (RM-007)
    /// </summary>
    public class SolicitudMaterial : BaseModel
    {
        [Key]
        public int IdSolicitud { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadSolicitada { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; } = DateTime.Now;

        /// <summary>
        /// Estado: "Pendiente", "Aprobada", "Rechazada", "Entregada"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [StringLength(500)]
        public string? Justificacion { get; set; }

        [StringLength(500)]
        public string? RespuestaAdmin { get; set; }

        public DateTime? FechaRespuesta { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }

        [ForeignKey(nameof(EmpleadoId))]
        public Empleado? Empleado { get; set; }
    }
}
