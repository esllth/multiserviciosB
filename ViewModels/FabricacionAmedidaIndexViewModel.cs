using MultiservicioB.DTOs;

namespace MultiservicioB.ViewModels
{
    public class FabricacionAmedidaIndexViewModel
    {
        public IEnumerable<ProyectoFabricacionDTO> Proyectos { get; set; } = Enumerable.Empty<ProyectoFabricacionDTO>();
        public IEnumerable<CotizacionListItemViewModel> Cotizaciones { get; set; } = Enumerable.Empty<CotizacionListItemViewModel>();
    }
}
