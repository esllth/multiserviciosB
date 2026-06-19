using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class ConsumoMaterial : BaseModel
    {
        [Key]
        public int IdConsumo { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CantidadUsada { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }
    }
}
