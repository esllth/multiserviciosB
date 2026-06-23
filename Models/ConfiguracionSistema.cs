using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class ConfiguracionSistema : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Clave")]
        public required string Clave { get; set; }

        [Required]
        [StringLength(255)]
        [Display(Name = "Valor")]
        public required string Valor { get; set; }

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }
    }
}