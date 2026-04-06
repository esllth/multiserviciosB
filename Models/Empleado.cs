using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Empleado
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Required]
        public string IdentificacionEmpleado { get; set; }

        [Required]
        public string NombreEmpleado { get; set; }

        [Required]
        public string ApellidosEmpleado { get; set; }

        [Required]
        [EmailAddress]
        public string CorreoElectronicoEmpleado { get; set; }

        [Required]
        public string TelefonoEmpleado { get; set; }

        [Required]
        public int DireccionId { get; set; }

        [Required]
        public string EstadoEmpleado { get; set; } // Pendiente / Activo / Inactivo

        [Required]
        public bool TieneUsuario { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SalarioBase { get; set; }

        [Required]
        public DateTime FechaInicioEmpleado { get; set; }

        public DateTime? FechaFinalizacionEmpleado { get; set; }

        // Relación con Identity
        public string? UserId { get; set; }
    }
}

