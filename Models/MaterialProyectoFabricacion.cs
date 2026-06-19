using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    /// <summary>
    /// Representa materiales requeridos para un proyecto de fabricación (RF-004)
    /// </summary>
    public class MaterialProyectoFabricacion : BaseModel
    {
        [Key]
        public int IdMaterialProyecto { get; set; }

        [Required]
        public int ProyectoId { get; set; }

        [Required]
        public int MaterialId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal CantidadRequerida { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? CantidadUsada { get; set; }

        [StringLength(500)]
        public string? Observaciones { get; set; }

        [ForeignKey(nameof(ProyectoId))]
        public ProyectoFabricacion? Proyecto { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }
    }
}
