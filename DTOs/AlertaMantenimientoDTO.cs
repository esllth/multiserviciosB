using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class AlertaMantenimientoDTO
    {
        public int IdAlerta { get; set; }

        [Required(ErrorMessage = "El equipo es requerido")]
        public int EquipoId { get; set; }

        public string? NombreEquipo { get; set; }

        public string? ClienteNombre { get; set; }

        [Required(ErrorMessage = "La fecha de mantenimiento es requerida")]
        public DateTime FechaMantenimiento { get; set; }

        [Required(ErrorMessage = "El tipo de mantenimiento es requerido")]
        public string TipoMantenimiento { get; set; } = "";

        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; } = "Pendiente";

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaNotificacion { get; set; }

        public DateTime? FechaRealizacion { get; set; }
    }
}
