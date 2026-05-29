using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Equipo : BaseModel
    {
        [Key]
        public int IdEquipo { get; set; }

        [StringLength(100)]
        public string? Nombre { get; set; }

        [StringLength(100)]
        public string? Categoria { get; set; }

        [StringLength(255)]
        public string? Especificaciones { get; set; }

        [StringLength(20)]
        public string? Estado { get; set; }

        public int? ClienteId { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }
    }
}
