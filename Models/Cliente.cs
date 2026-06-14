using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MultiservicioB.Models
{
    public class Cliente : BaseModel
    {
        [Key]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(50)]
        public string Identificacion { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(100)]
        public string? Apellidos { get; set; }

        [StringLength(150)]
        [EmailAddress]
        public string? Correo { get; set; }

        [StringLength(20)]
        public string? Telefono { get; set; }

        [StringLength(150)]
        public string? NombreNegocio { get; set; }

        public int? DireccionId { get; set; }

        [StringLength(20)]
        public string? Estado { get; set; }

        [ForeignKey(nameof(DireccionId))]
        public Direccion? Direccion { get; set; }
    }
}
