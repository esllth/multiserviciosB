using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.ViewModels
{
    public class CrearOrdenAdministrativaViewModel
    {
        public int? ClienteId { get; set; }

        [Required(ErrorMessage = "Ingrese la cédula, el correo o el teléfono del cliente.")]
        [StringLength(150)]
        [Display(Name = "Buscar cliente")]
        public string IdentificadorCliente { get; set; } = "";

        [Required(ErrorMessage = "Seleccione el tipo de servicio.")]
        [Display(Name = "Tipo de servicio")]
        public int? TipoServicioId { get; set; }

        [Required(ErrorMessage = "Describa el trabajo solicitado.")]
        [StringLength(255)]
        [Display(Name = "Descripción del servicio")]
        public string Descripcion { get; set; } = "";

        [Display(Name = "Requerir fotografías para cerrar la orden")]
        public bool RequiereFotosObligatorias { get; set; } = true;
    }
}
