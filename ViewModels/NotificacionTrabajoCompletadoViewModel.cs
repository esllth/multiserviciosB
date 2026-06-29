using System;

namespace MultiservicioB.ViewModels
{
    public class NotificacionTrabajoCompletadoViewModel
    {
        public int IdNotificacion { get; set; }

        public int IdOrden { get; set; }

        public string? Mensaje { get; set; }

        public DateTime? Fecha { get; set; }

        public string? Cliente { get; set; }

        public string? Tecnico { get; set; }

        public string? EstadoOrden { get; set; }
    }
}
