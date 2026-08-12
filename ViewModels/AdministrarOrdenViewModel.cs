using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.ViewModels
{
    public class AdministrarOrdenViewModel
    {
        public int IdOrden { get; set; }

        [Display(Name = "Técnico asignado")]
        public int? EmpleadoId { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public int EstadoOrdenId { get; set; }

        [Display(Name = "Fecha y hora de compromiso")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaCompromiso { get; set; }

        [Display(Name = "Fecha de compromiso")]
        [DataType(DataType.Date)]
        public DateTime? FechaCalendario { get; set; }

        [Display(Name = "Hora de compromiso")]
        [DataType(DataType.Time)]
        public TimeSpan? HoraCalendario { get; set; }

        [Display(Name = "Confirmar compromiso")]
        public bool CompromisoConfirmado { get; set; }
    }
}
