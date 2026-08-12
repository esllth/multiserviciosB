using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class RevistaNosotrosDTO
    {
        [Required(ErrorMessage = "La ubicación del taller es obligatoria.")]
        [StringLength(255)]
        [Display(Name = "Ubicación del taller")]
        public string UbicacionTaller { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Ingrese un correo electrónico válido.")]
        [StringLength(255)]
        [Display(Name = "Correo electrónico")]
        public string CorreoElectronico { get; set; } = "";

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "Ingrese un número de teléfono válido.")]
        [StringLength(50)]
        [Display(Name = "Número de teléfono")]
        public string NumeroTelefono { get; set; } = "";

        [Required(ErrorMessage = "La leyenda es obligatoria.")]
        [StringLength(255)]
        [Display(Name = "Leyenda de Nosotros")]
        public string Leyenda { get; set; } = "";
    }
}
