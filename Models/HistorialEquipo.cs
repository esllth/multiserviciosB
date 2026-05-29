using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class HistorialEquipo : BaseModel
    {
        [Key]
        public int IdHistorial { get; set; }

        [Required]
        public int EquipoId { get; set; }

        [Required]
        public int OrdenId { get; set; }

        [Required]
        public DateTime FechaServicio { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        [ForeignKey(nameof(EquipoId))]
        public Equipo? Equipo { get; set; }

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }
    }
}
