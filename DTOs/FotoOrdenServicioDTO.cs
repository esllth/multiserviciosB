using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class FotoOrdenServicioDTO
    {
        public int IdFotoOrden { get; set; }

        [Required(ErrorMessage = "La orden es requerida")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El tipo de foto es requerido")]
        public string TipoFoto { get; set; } = "";

        public string? Descripcion { get; set; }

        public string? Ruta { get; set; }

        public string? NombreOriginal { get; set; }

        public string? TipoContenido { get; set; }

        public DateTime FechaCarga { get; set; }
    }
}
