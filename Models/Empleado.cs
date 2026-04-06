using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace MultiservicioB.Models
{
    public class Empleado
    {

        public int Id { get; set; }

        public string IdentificacionEmpleado { get; set; }

        public string NombreEmpleado { get; set; }

        public string ApellidosEmpleado { get; set; }

        public string CorreoElectronicoEmpleado { get; set; }

        public string TelefonoEmpleado { get; set; }

        public int? DireccionId { get; set; }

        public string EstadoEmpleado { get; set; } // Pendiente / Activo / Inactivo

        public bool TieneUsuario { get; set; } = false;


        [Column(TypeName = "decimal(10,2)")]
        public decimal SalarioBase { get; set; }

    public DateTime FechaInicioEmpleado { get; set; }

        public DateTime? FechaFinalizacionEmpleado { get; set; }

        public string? UserId { get; set; } // Relación con Identity
    }
}