namespace MultiservicioB.ViewModels
{
    public class EncuestasIndexViewModel
    {
        public bool EsCliente { get; set; }
        public int TotalRespuestas { get; set; }
        public double PromedioServicio { get; set; }
        public double PromedioTecnico { get; set; }
        public List<EncuestaResultadoViewModel> Resultados { get; set; } = new();
        public List<EncuestaClienteViewModel> EncuestasCliente { get; set; } = new();
    }

    public class EncuestaResultadoViewModel
    {
        public int OrdenId { get; set; }
        public string Cliente { get; set; } = "";
        public string Tecnico { get; set; } = "";
        public int CalificacionServicio { get; set; }
        public int CalificacionTecnico { get; set; }
        public string? Comentarios { get; set; }
        public DateTime? Fecha { get; set; }
    }

    public class EncuestaClienteViewModel
    {
        public int OrdenId { get; set; }
        public string Servicio { get; set; } = "Servicio";
        public string Tecnico { get; set; } = "Sin técnico asignado";
        public int? CalificacionServicio { get; set; }
        public int? CalificacionTecnico { get; set; }
        public string? Comentarios { get; set; }
        public DateTime? Fecha { get; set; }
        public bool Respondida { get; set; }
    }

}
