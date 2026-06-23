using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class SolicitudMaterialDTO
    {
        public int IdSolicitud { get; set; }

        [Required(ErrorMessage = "La orden es requerida")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El material es requerido")]
        public int MaterialId { get; set; }

        public string? NombreMaterial { get; set; }

        [Required(ErrorMessage = "El empleado es requerido")]
        public int EmpleadoId { get; set; }

        public string? NombreEmpleado { get; set; }

        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadSolicitada { get; set; }

        public DateTime FechaSolicitud { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public string Estado { get; set; } = "Pendiente";

        public string? Justificacion { get; set; }

        public string? RespuestaAdmin { get; set; }

        public DateTime? FechaRespuesta { get; set; }
    }
}
