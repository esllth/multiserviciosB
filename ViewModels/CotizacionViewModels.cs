using System.ComponentModel.DataAnnotations;

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
        public DateTime? FechaSolicitud { get; set; }
        public bool AprobadaPorCliente { get; set; }
    }

    public class SolicitarCotizacionViewModel
    {
        [Required(ErrorMessage = "Seleccione un tipo de servicio.")]
        [Display(Name = "Tipo de servicio")]
        public int? TipoServicioId { get; set; }

        [Required(ErrorMessage = "Describa el servicio requerido.")]
        [StringLength(255)]
        [Display(Name = "Descripción de la solicitud")]
        public string Descripcion { get; set; } = "";
    }

    public class EvaluarCotizacionViewModel
    {
        public int IdCotizacion { get; set; }

        [Required(ErrorMessage = "Ingrese el monto presupuestado.")]
        [Range(typeof(decimal), "0.01", "9999999999", ErrorMessage = "Ingrese un monto válido.")]
        [Display(Name = "Monto presupuestado")]
        public decimal? MontoPresupuesto { get; set; }
    }
}
