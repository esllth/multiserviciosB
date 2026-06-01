using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class HorarioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El día de la semana es obligatorio")]
        [Display(Name = "Día de la Semana")]
        public string DiaSemana { get; set; }

        [Required(ErrorMessage = "La hora de inicio es obligatoria")]
        [Display(Name = "Hora de Inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required(ErrorMessage = "La hora de fin es obligatoria")]
        [Display(Name = "Hora de Fin")]
        public TimeSpan HoraFin { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}