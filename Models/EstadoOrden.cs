using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class EstadoOrden : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; } = string.Empty;
    }
}
