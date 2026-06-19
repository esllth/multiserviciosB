using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class TipoServicioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres")]
        [Display(Name = "Nombre")]
        public required string Nombre { get; set; }

        [StringLength(20, ErrorMessage = "El estado no puede superar 20 caracteres")]
        [Display(Name = "Estado")]
        public string? Estado { get; set; } = "Activo";
    }
}