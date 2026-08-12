using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.ViewModels
{
    public class RegistrarCotizacionAdministrativaViewModel
    {
        [Required(ErrorMessage = "Ingrese la cédula, el correo o el teléfono del cliente.")]
        [StringLength(150)]
        [Display(Name = "Cliente")]
        public string IdentificadorCliente { get; set; } = "";

        [Required(ErrorMessage = "Seleccione un tipo de servicio.")]
        [Display(Name = "Tipo de servicio")]
        public int? TipoServicioId { get; set; }

        [Required(ErrorMessage = "Describa el servicio solicitado.")]
        [StringLength(255)]
        [Display(Name = "Descripción de la solicitud")]
        public string Descripcion { get; set; } = "";

        [Display(Name = "Fecha solicitada para la visita")]
        public DateTime? FechaVisitaSolicitada { get; set; }
    }
}
