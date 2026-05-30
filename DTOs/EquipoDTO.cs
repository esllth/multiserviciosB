using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class EquipoDTO
    {
        public int IdEquipo { get; set; }

        [Display(Name = "Nombre")]
        public string? Nombre { get; set; }

        [Display(Name = "Categoría")]
        public string? Categoria { get; set; }

        [Display(Name = "Especificaciones")]
        public string? Especificaciones { get; set; }

        [Display(Name = "Estado")]
        public string? Estado { get; set; }

        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }

        public string? NombreCliente { get; set; }
    }
}
