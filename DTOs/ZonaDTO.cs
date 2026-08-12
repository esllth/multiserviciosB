using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class ZonaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La provincia es obligatoria")]
        [Display(Name = "Provincia")]
        public required string Provincia { get; set; }

        [Required(ErrorMessage = "El cantón es obligatorio")]
        [Display(Name = "Cantón")]
        public required string Canton { get; set; }

        [Required(ErrorMessage = "El distrito es obligatorio")]
        [Display(Name = "Distrito")]
        public required string Distrito { get; set; }

        [Required(ErrorMessage = "Seleccione un distrito de la DTA")]
        [RegularExpression(@"^\d-\d{2}-\d{2}$", ErrorMessage = "El código DTA no es válido")]
        [Display(Name = "Código DTA")]
        public string CodigoDTA { get; set; } = string.Empty;

        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
