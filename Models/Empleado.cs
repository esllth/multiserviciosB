using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public static class EstadosEmpleado
    {
        public const string Pendiente = "Pendiente";
        public const string Activo = "Activo";
        public const string Inactivo = "Inactivo";

        public static string Obtener(Empleado empleado)
        {
            if (empleado.EstadoAcceso.Equals(Activo, StringComparison.OrdinalIgnoreCase) ||
                empleado.EstadoAcceso.Equals("Aprobado", StringComparison.OrdinalIgnoreCase))
            {
                return empleado.EstadoEmpleado ? Activo : Inactivo;
            }

            if (empleado.EstadoAcceso.Equals(Pendiente, StringComparison.OrdinalIgnoreCase) ||
                empleado.EstadoAcceso.StartsWith("Pendiente", StringComparison.OrdinalIgnoreCase))
            {
                return Pendiente;
            }

            return Inactivo;
        }

        public static bool PuedeAcceder(Empleado empleado) =>
            Obtener(empleado) == Activo;

        public static void Aplicar(Empleado empleado, string estado)
        {
            switch (estado)
            {
                case Activo:
                    empleado.EstadoEmpleado = true;
                    empleado.EstadoAcceso = Activo;
                    empleado.FechaFinalizacionEmpleado = null;
                    break;
                case Inactivo:
                    empleado.EstadoEmpleado = false;
                    empleado.EstadoAcceso = Inactivo;
                    empleado.FechaFinalizacionEmpleado ??= DateTime.UtcNow;
                    break;
                default:
                    empleado.EstadoEmpleado = false;
                    empleado.EstadoAcceso = Pendiente;
                    empleado.FechaFinalizacionEmpleado = null;
                    break;
            }
        }
    }

    public class Empleado : BaseModel
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Required]
        public string IdentificacionEmpleado { get; set; } = string.Empty;

        [Required]
        public string NombreEmpleado { get; set; } = string.Empty;

        [Required]
        public string ApellidosEmpleado { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string CorreoElectronicoEmpleado { get; set; } = string.Empty;

        [Required]
        public string TelefonoEmpleado { get; set; } = string.Empty;

        public int? DireccionId { get; set; }

        [Required]
        public bool EstadoEmpleado { get; set; } // true = Activo, false = Inactivo

        [Required]
        public bool TieneUsuario { get; set; }

        [Required]
        [StringLength(30)]
        public string EstadoAcceso { get; set; } = EstadosEmpleado.Pendiente;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal SalarioBase { get; set; }

        [Required]
        public DateTime FechaInicioEmpleado { get; set; }

        public DateTime? FechaFinalizacionEmpleado { get; set; }

        // Relación con Identity
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        [StringLength(300)]
        public string? FotoPerfil { get; set; }

        [NotMapped]
        [Display(Name = "Rol del personal")]
        public string RolInicial { get; set; } = "Empleado";
    }
}

