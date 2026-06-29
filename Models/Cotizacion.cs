using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Cotizacion : BaseModel
    {
        [Key]
        public int IdCotizacion { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int TipoServicioId { get; set; }

        [Required]
        public int EstadoCotizacionId { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        [Column(TypeName = "decimal(12,2)")]
        public decimal? MontoPresupuesto { get; set; }

        public bool RequiereAdelanto { get; set; } = false;

        public int? PorcentajeAdelanto { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        public DateTime? FechaVisitaSolicitada { get; set; }

        public bool UsarDireccionPerfil { get; set; } = false;

        [StringLength(500)]
        public string? EnlaceWaze { get; set; }

        [StringLength(40)]
        public string? FormaPagoAceptada { get; set; }

        public bool AprobadaPorCliente { get; set; } = false;

        [ForeignKey(nameof(ClienteId))]
        public Cliente? Cliente { get; set; }

        [ForeignKey(nameof(TipoServicioId))]
        public TipoServicio? TipoServicio { get; set; }

        [ForeignKey(nameof(EstadoCotizacionId))]
        public EstadoCotizacion? EstadoCotizacion { get; set; }

        public ICollection<FotoCotizacion> Fotos { get; set; } = new List<FotoCotizacion>();
    }
}
