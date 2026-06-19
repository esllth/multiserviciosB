using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class EquipoDTO
    {
        public int IdEquipo { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [Display(Name = "Código")]
        [StringLength(50)]
        public string? Codigo { get; set; }

        [Display(Name = "Categoría")]
        [StringLength(100)]
        public string? Categoria { get; set; }

        [Display(Name = "Tipo de Equipo")]
        [StringLength(100)]
        public string? TipoEquipo { get; set; }

        [Display(Name = "Especificaciones")]
        [StringLength(2000)]
        public string? Especificaciones { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Operativo";

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        public string? NombreCliente { get; set; }

        [Display(Name = "Marca")]
        [StringLength(100)]
        public string? Marca { get; set; }

        [Display(Name = "Modelo")]
        [StringLength(100)]
        public string? Modelo { get; set; }

        [Display(Name = "Número de Serie")]
        [StringLength(100)]
        public string? NumeroSerie { get; set; }

        [Display(Name = "Fecha de Adquisición")]
        [DataType(DataType.Date)]
        public DateTime? FechaAdquisicion { get; set; }

        [Display(Name = "Frecuencia de Mantenimiento (Días)")]
        [Range(1, 3650, ErrorMessage = "La frecuencia debe estar entre 1 y 3650 días")]
        public int? FrecuenciaMantenimientoDias { get; set; }

        [Display(Name = "Último Mantenimiento")]
        [DataType(DataType.Date)]
        public DateTime? UltimoMantenimiento { get; set; }

        [Display(Name = "Próximo Mantenimiento")]
        [DataType(DataType.Date)]
        public DateTime? ProximoMantenimiento { get; set; }

        [Display(Name = "Observaciones")]
        [StringLength(1000)]
        public string? Observaciones { get; set; }

        // Propiedades calculadas
        public int? DiasParaMantenimiento { get; set; }
        public bool RequiereMantenimiento { get; set; }
        public bool MantenimientoVencido { get; set; }
    }
}
