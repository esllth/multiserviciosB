using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.ViewModels
{
    public class ChatFabricacionViewModel
    {
        [Required(ErrorMessage = "Indique un nombre para el proyecto")]
        [StringLength(200)]
        [Display(Name = "Nombre del proyecto")]
        public string NombreProyecto { get; set; } = "";

        [Required(ErrorMessage = "Indique que desea fabricar")]
        [StringLength(120)]
        [Display(Name = "Que desea fabricar")]
        public string TipoTrabajo { get; set; } = "";

        [StringLength(150)]
        [Display(Name = "Medidas aproximadas")]
        public string? Medidas { get; set; }

        [StringLength(150)]
        [Display(Name = "Material preferido")]
        public string? MaterialPreferido { get; set; }

        [StringLength(150)]
        [Display(Name = "Acabado o color")]
        public string? AcabadoColor { get; set; }

        [StringLength(200)]
        [Display(Name = "Lugar de instalacion")]
        public string? UbicacionInstalacion { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha deseada")]
        public DateTime? FechaDeseada { get; set; }

        [DataType(DataType.Currency)]
        [Range(0, 999999999, ErrorMessage = "El presupuesto debe ser un monto valido")]
        [Display(Name = "Presupuesto aproximado")]
        public decimal? PresupuestoAproximado { get; set; }

        [StringLength(600)]
        [Display(Name = "Detalles adicionales")]
        public string? DetallesAdicionales { get; set; }
    }
}
