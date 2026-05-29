using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class ProyectoFabricacion : BaseModel
    {
        [Key]
        public int IdProyecto { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        [StringLength(20)]
        public string? Estado { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }
    }
}
