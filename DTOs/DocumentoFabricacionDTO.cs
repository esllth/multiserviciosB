using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class DocumentoFabricacionDTO
    {
        public int IdDocumento { get; set; }

        [Required(ErrorMessage = "El proyecto es requerido")]
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "El nombre del documento es requerido")]
        [StringLength(100)]
        public string NombreDocumento { get; set; } = "";

        [Required(ErrorMessage = "El tipo de documento es requerido")]
        public string TipoDocumento { get; set; } = "";

        public string? Ruta { get; set; }

        public string? Descripcion { get; set; }

        public DateTime FechaCarga { get; set; }

        public string? CargadoPorUsuarioId { get; set; }

        public string? CargadoPorNombre { get; set; }
    }
}
