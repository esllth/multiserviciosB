using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Encuesta : BaseModel
    {
        [Key] public int IdEncuesta { get; set; }
        public int OrdenId { get; set; }
        public int ClienteId { get; set; }
        [Range(1, 5)] public int? CalificacionServicio { get; set; }
        [Range(1, 5)] public int? CalificacionTecnico { get; set; }
        [StringLength(255)] public string? Comentarios { get; set; }
        [Column(TypeName = "date")] public DateTime? Fecha { get; set; }
        [ForeignKey(nameof(OrdenId))] public OrdenServicio? Orden { get; set; }
        [ForeignKey(nameof(ClienteId))] public Cliente? Cliente { get; set; }
    }
}
