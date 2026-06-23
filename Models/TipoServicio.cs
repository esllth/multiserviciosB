using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class TipoServicio : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [StringLength(20)]
        public string? Estado { get; set; }
    }
}
