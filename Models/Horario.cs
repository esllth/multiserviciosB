using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class Horario : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Día de la Semana")]
        public string DiaSemana { get; set; }

        [Required]
        [Display(Name = "Hora de Inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Display(Name = "Hora de Fin")]
        public TimeSpan HoraFin { get; set; }

        [Required]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}