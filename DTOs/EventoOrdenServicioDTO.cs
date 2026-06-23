using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class EventoOrdenServicioDTO
    {
        public int IdEvento { get; set; }

        [Required(ErrorMessage = "La orden es requerida")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El tipo de evento es requerido")]
        public string TipoEvento { get; set; } = "";

        public DateTime FechaEvento { get; set; }

        public string? Descripcion { get; set; }

        public decimal? Latitud { get; set; }

        public decimal? Longitud { get; set; }

        public string? UsuarioId { get; set; }

        public string? NombreUsuario { get; set; }
    }
}
