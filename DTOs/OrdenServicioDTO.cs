using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class OrdenServicioDTO
    {
        public int IdOrden { get; set; }

        [Display(Name = "Cotización")]
        public int CotizacionId { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public string? NombreCliente { get; set; }

        [Display(Name = "Técnico Asignado")]
        public int? EmpleadoId { get; set; }

        public string? NombreTecnico { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de compromiso")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaCompromiso { get; set; }

        [Display(Name = "Compromiso confirmado")]
        public bool CompromisoConfirmado { get; set; }

        [Display(Name = "Usa direccion del perfil")]
        public bool UsarDireccionPerfil { get; set; }

        public string? DireccionServicio { get; set; }

        public string? GoogleMapsUrl { get; set; }

        public string? EnlaceWaze { get; set; }

        [Display(Name = "Fecha de Llegada al Sitio")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaLlegadaSitio { get; set; }

        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "Fecha de Aceptación del Cliente")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaAceptacionCliente { get; set; }

        [Display(Name = "Estado")]
        public int EstadoOrdenId { get; set; }

        public string? NombreEstado { get; set; }

        public string? DescripcionServicio { get; set; }

        public decimal? MontoPresupuesto { get; set; }

        public bool RequiereAdelanto { get; set; }

        public int? PorcentajeAdelanto { get; set; }

        public string? FormaPagoAceptada { get; set; }

        [Display(Name = "Observaciones Técnicas")]
        public string? ObservacionesTecnicas { get; set; }

        [Display(Name = "Comentarios Finales")]
        public string? ComentariosFinales { get; set; }

        [Display(Name = "Requiere Fotos Obligatorias")]
        public bool RequiereFotosObligatorias { get; set; } = true;

        [Display(Name = "Llegada Confirmada")]
        public bool LlegadaConfirmada { get; set; }

        // Propiedades calculadas
        public int? TiempoEfectivoMinutos { get; set; }
        public bool TieneFotosInicio { get; set; }
        public bool TieneFotosFin { get; set; }
        public bool PuedeFinalizarse { get; set; }
        public bool AvisoTrabajoCompletadoEnviado { get; set; }
        public DateTime? FechaAvisoTrabajoCompletado { get; set; }
    }
}
