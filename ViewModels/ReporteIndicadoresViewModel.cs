using System;
using System.Collections.Generic;

namespace MultiservicioB.ViewModels
{

    /// REP-001: Indicadores del panel de órdenes de servicio para Secretaría.

    public class ReporteIndicadoresViewModel
    {
        // --- Conteos por estado ---
        public int Pendientes { get; set; }
        public int EnProgreso { get; set; }
        public int Completadas { get; set; }
        public int Canceladas { get; set; }
        public int Total { get; set; }

        // --- Filtro por rango de fechas  ---
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        // --- Desgloses  ---
        public List<ConteoPorCategoria> PorTecnico { get; set; } = new();
        public List<ConteoPorCategoria> PorTipoServicio { get; set; } = new();
    }


    /// Par nombre/cantidad usado en los desgloses por técnico y por tipo de servicio.
    /// 
    public class ConteoPorCategoria
    {
        public string Nombre { get; set; } = string.Empty;
        public int Cantidad { get; set; }
    }
}
