using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.ViewModels
{
    public class ResponderEncuestaViewModel
    {
        public int OrdenId { get; set; }
        [Required(ErrorMessage = "Califique el servicio")]
        [Range(1, 5, ErrorMessage = "Seleccione una calificación entre 1 y 5")]
        [Display(Name = "Calificación del servicio")]
        public int? CalificacionServicio { get; set; }
        [Required(ErrorMessage = "Califique la atención del técnico")]
        [Range(1, 5, ErrorMessage = "Seleccione una calificación entre 1 y 5")]
        [Display(Name = "Calificación del técnico")]
        public int? CalificacionTecnico { get; set; }
        [StringLength(255)]
        [Display(Name = "Comentarios adicionales")]
        public string? Comentarios { get; set; }
    }
}
