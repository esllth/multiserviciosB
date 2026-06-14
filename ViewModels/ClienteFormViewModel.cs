using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.ViewModels
{
    public class ClienteFormViewModel
    {
        public int IdCliente { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Identificación")]
        public string Identificacion { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; } = "";

        [StringLength(100)]
        public string? Apellidos { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Correo { get; set; }

        [StringLength(20)]
        [Display(Name = "Teléfono")]
        public string? Telefono { get; set; }

        [StringLength(150)]
        [Display(Name = "Nombre del negocio")]
        public string? NombreNegocio { get; set; }

        [StringLength(20)]
        public string? Estado { get; set; }

        [Required(ErrorMessage = "Seleccione una provincia.")]
        [Display(Name = "Provincia")]
        public int? ProvinciaId { get; set; }

        [Required(ErrorMessage = "Seleccione un cantón.")]
        [Display(Name = "Cantón")]
        public int? CantonId { get; set; }

        [Required(ErrorMessage = "Seleccione un distrito.")]
        [Display(Name = "Distrito")]
        public int? UbicacionDTAId { get; set; }

        // Nombres de la ubicación (para crear/actualizar UbicacionDTA desde el API externo)
        public string? NombreProvincia { get; set; }
        public string? NombreCanton { get; set; }
        public string? NombreDistrito { get; set; }
        public string? CodigoDTA { get; set; }

        [StringLength(255)]
        [Display(Name = "Otras señas")]
        public string? OtrasSenas { get; set; }
    }
}
