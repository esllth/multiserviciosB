using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Notificacion : BaseModel
    {
        [Key]
        public int IdNotificacion { get; set; }

        public int? OrdenId { get; set; }

        public int? ClienteId { get; set; }

        public int? MaterialId { get; set; }

        [StringLength(100)]
        public string? Titulo { get; set; }

        [StringLength(255)]
        public string? Mensaje { get; set; }

        public DateTime? Fecha { get; set; } = DateTime.Now;

        public bool? Leida { get; set; } = false;

        [ForeignKey(nameof(OrdenId))]
        public OrdenServicio? Orden { get; set; }

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }
    }
}
