using MultiservicioB.DTOs;

namespace MultiservicioB.ViewModels
{
    public class MaterialHistorialViewModel
    {
        public MaterialDTO Material { get; set; } = new();
        public List<ConsumoMaterialDTO> Movimientos { get; set; } = new();
    }
}
