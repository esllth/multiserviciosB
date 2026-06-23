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
        public required string Provincia { get; set; }

        [Required]
        public int IdCanton { get; set; }

        [Required]
        [StringLength(100)]
        public required string Canton { get; set; }

        [Required]
        public int IdDistrito { get; set; }

        [Required]
        [StringLength(100)]
        public required string Distrito { get; set; }

        [Required]
        [StringLength(20)]
        public required string CodigoDTA { get; set; }
    }
}
