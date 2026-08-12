using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class Zona : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Provincia")]
        public required string Provincia { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Cantón")]
        public required string Canton { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Distrito")]
        public required string Distrito { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Código DTA")]
        public string CodigoDTA { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
