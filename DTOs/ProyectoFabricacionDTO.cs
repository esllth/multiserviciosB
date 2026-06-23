using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class ProyectoFabricacionDTO
    {
        public int IdProyecto { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public string? NombreCliente { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio")]
        [Display(Name = "Nombre del Proyecto")]
        [StringLength(200)]
        public string NombreProyecto { get; set; } = "";

        [Display(Name = "Descripción")]
        [StringLength(1000)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Estado")]
        public string Estado { get; set; } = "Pendiente";

        [Display(Name = "Fecha de Solicitud")]
        [DataType(DataType.DateTime)]
        public DateTime FechaSolicitud { get; set; }

        [Display(Name = "Fecha de Inicio Estimada")]
        [DataType(DataType.Date)]
        public DateTime? FechaInicioEstimada { get; set; }

        [Display(Name = "Fecha de Fin Estimada")]
        [DataType(DataType.Date)]
        public DateTime? FechaFinEstimada { get; set; }

        [Display(Name = "Fecha de Inicio Real")]
        [DataType(DataType.Date)]
        public DateTime? FechaInicioReal { get; set; }

        [Display(Name = "Fecha de Fin Real")]
        [DataType(DataType.Date)]
        public DateTime? FechaFinReal { get; set; }

        [Display(Name = "Costo Estimado")]
        [DataType(DataType.Currency)]
        public decimal? CostoEstimado { get; set; }

        [Display(Name = "Costo Real")]
        [DataType(DataType.Currency)]
        public decimal? CostoReal { get; set; }

        [Display(Name = "Diseño Aprobado")]
        public bool DiseñoAprobado { get; set; }

        [Display(Name = "Fecha de Aprobación")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaAprobacionDiseño { get; set; }

        [Display(Name = "Observaciones del Cliente")]
        public string? ObservacionesCliente { get; set; }

        [Display(Name = "Observaciones Internas")]
        public string? ObservacionesInternas { get; set; }

        // Propiedades calculadas
        public int DiasTranscurridos { get; set; }
        public int? DiasRestantes { get; set; }
        public decimal? PorcentajeCumplimiento { get; set; }
    }
}
