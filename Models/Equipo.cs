using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa un equipo registrado en el sistema (RE-001 a RE-010)
    /// </summary>
    public class Equipo : BaseModel
    {
        [Key]
        public int IdEquipo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [StringLength(50)]
        public string? Codigo { get; set; }

        [StringLength(100)]
        public string? Categoria { get; set; }

        [StringLength(100)]
        public string? TipoEquipo { get; set; }

        [StringLength(2000)]
        public string? Especificaciones { get; set; }

        /// <summary>
        /// Estado: "Operativo", "EnMantenimiento", "DadoDeBaja", "EnReparacion"
        /// </summary>
        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Operativo";

        public int? ClienteId { get; set; }

        [StringLength(100)]
        public string? Marca { get; set; }

        [StringLength(100)]
        public string? Modelo { get; set; }

        [StringLength(100)]
        public string? NumeroSerie { get; set; }

        public DateTime? FechaAdquisicion { get; set; }

        /// <summary>
        /// Frecuencia de mantenimiento preventivo en días (RE-009)
        /// </summary>
        public int? FrecuenciaMantenimientoDias { get; set; }

        public DateTime? UltimoMantenimiento { get; set; }

        public DateTime? ProximoMantenimiento { get; set; }

        [StringLength(1000)]
        public string? Observaciones { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }
    }
}
