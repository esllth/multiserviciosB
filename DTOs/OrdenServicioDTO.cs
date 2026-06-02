using System;
using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class OrdenServicioDTO
    {
        public int IdOrden { get; set; }

        [Display(Name = "Cotización")]
        public int CotizacionId { get; set; }

        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        public string? NombreCliente { get; set; }

        [Display(Name = "Técnico Asignado")]
        public int? EmpleadoId { get; set; }

        public string? NombreTecnico { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [Display(Name = "Fecha de Inicio")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaInicio { get; set; }

        [Display(Name = "Fecha de Fin")]
        [DataType(DataType.DateTime)]
        public DateTime? FechaFin { get; set; }

        [Display(Name = "Estado")]
        public int EstadoOrdenId { get; set; }

        public string? NombreEstado { get; set; }

        public string? DescripcionServicio { get; set; }
    }
}
