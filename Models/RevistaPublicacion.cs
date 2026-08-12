using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.Models
{
    public class RevistaPublicacion : BaseModel
    {
        [Key]
        public int IdPublicacion { get; set; }

        [Required, StringLength(80)]
        public string Titulo { get; set; } = string.Empty;

        [Required, StringLength(250)]
        public string Descripcion { get; set; } = string.Empty;

        [Required, StringLength(300)]
        public string Imagen { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string TextoEnlace { get; set; } = string.Empty;

        public int Orden { get; set; }
        public bool Activo { get; set; } = true;
    }
}
