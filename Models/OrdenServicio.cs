using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class OrdenServicio : BaseModel
    {
        [Key]
        public int IdOrden { get; set; }

        [Required]
        public int CotizacionId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int EmpleadoId { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [Required]
        public int EstadoOrdenId { get; set; }

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
