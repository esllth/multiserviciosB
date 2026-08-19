namespace MultiservicioB.ViewModels
{
    public class EncuestasIndexViewModel
    {
        public int TotalRespuestas { get; set; }
        public double PromedioServicio { get; set; }
        public double PromedioTecnico { get; set; }
        public List<EncuestaResultadoViewModel> Resultados { get; set; } = new();
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
}
