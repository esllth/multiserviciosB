using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Direccion : BaseModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UbicacionDTAId { get; set; }

        [StringLength(255)]
        public string? OtrasSenas { get; set; }

        [ForeignKey(nameof(UbicacionDTAId))]
        public UbicacionDTA? UbicacionDTA { get; set; }
    }
}
