using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MultiservicioB.ViewModels
{
    public class CotizacionListItemViewModel
    {
        public int IdCotizacion { get; set; }
        public string Cliente { get; set; } = "";
        public string TipoServicio { get; set; } = "";
        public string Estado { get; set; } = "";
        public string? Descripcion { get; set; }
        public decimal? MontoPresupuesto { get; set; }
        public bool RequiereAdelanto { get; set; }
        public int? PorcentajeAdelanto { get; set; }
        public DateTime? FechaSolicitud { get; set; }
        public DateTime? FechaVisitaSolicitada { get; set; }
        public bool AprobadaPorCliente { get; set; }
    }

    public class SolicitarCotizacionViewModel
    {
        [Display(Name = "Fotografías de referencia")]
        public List<IFormFile> FotosReferencia { get; set; } = new();

        [Required(ErrorMessage = "Seleccione un tipo de servicio.")]
        [Display(Name = "Tipo de servicio")]
        public int? TipoServicioId { get; set; }

        [Required(ErrorMessage = "Describa el servicio requerido.")]
        [StringLength(255)]
        [Display(Name = "Descripción de la solicitud")]
        public string Descripcion { get; set; } = "";

        [Display(Name = "Fecha requerida para la visita")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaVisitaSolicitada { get; set; }
    }

    public class EvaluarCotizacionViewModel
    {
        public int IdCotizacion { get; set; }

        [Required(ErrorMessage = "Ingrese el monto presupuestado.")]
        [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "Ingrese un monto válido.")]
        [Display(Name = "Monto presupuestado (colones)")]
        public decimal? MontoPresupuesto { get; set; }

        [Display(Name = "Solicitar adelanto")]
        public bool RequiereAdelanto { get; set; }

        [Display(Name = "Porcentaje de adelanto")]
        public int? PorcentajeAdelanto { get; set; }
    }

    public class AgendarCitaViewModel
    {
        public int IdCotizacion { get; set; }

        [Required(ErrorMessage = "Seleccione la fecha y hora de la cita.")]
        [Display(Name = "Fecha y hora solicitada")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaCompromiso { get; set; }

        [Display(Name = "Usar la direccion de mi perfil para este servicio")]
        public bool UsarDireccionPerfil { get; set; }

        [StringLength(500)]
        [Display(Name = "Enlace de Waze")]
        public string? EnlaceWaze { get; set; }

        [Required(ErrorMessage = "Seleccione la forma de pago acordada.")]
        [Display(Name = "Forma de pago")]
        public string FormaPagoAceptada { get; set; } = "";

        public string? DireccionPerfilResumen { get; set; }
    }

    public class CompromisoCalendarioViewModel
    {
        public int IdOrden { get; set; }
        public DateTime FechaCompromiso { get; set; }
        public string Cliente { get; set; } = "";
        public string TipoServicio { get; set; } = "";
        public string? Tecnico { get; set; }
        public bool Confirmado { get; set; }
    }
}
