using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class ConfiguracionSistemaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La clave es obligatoria")]
        [Display(Name = "Clave")]
        public required string Clave { get; set; }

        [Required(ErrorMessage = "El valor es obligatorio")]
        [Display(Name = "Valor")]
        public required string Valor { get; set; }

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
}