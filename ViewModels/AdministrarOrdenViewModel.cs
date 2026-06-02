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
    }
}
