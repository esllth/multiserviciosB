using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class UbicacionDTA : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdProvincia { get; set; }

        [Required]
        [StringLength(100)]
        public string Provincia { get; set; }

        [Required]
        public int IdCanton { get; set; }

        [Required]
        [StringLength(100)]
        public string Canton { get; set; }

        [Required]
        public int IdDistrito { get; set; }

        [Required]
        [StringLength(100)]
        public string Distrito { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoDTA { get; set; }
    }
}
