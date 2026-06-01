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
        public string Provincia { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Cantón")]
        public string Canton { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Distrito")]
        public string Distrito { get; set; }

        [StringLength(255)]
        [Display(Name = "Descripción")]
        public string? Descripcion { get; set; }

        [Required]
        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}