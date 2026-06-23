using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDTO>> GetAllAsync();
        Task<IEnumerable<MaterialDTO>> GetActivosAsync();
        Task<MaterialDTO?> GetByIdAsync(int id);
        Task<MaterialDTO> CreateAsync(MaterialDTO materialDto);
        Task<bool> UpdateAsync(MaterialDTO materialDto);
        Task<bool> DeleteAsync(int id);

        // Métodos para historias de usuario RM
        Task<IEnumerable<MaterialDTO>> GetBajoStockAsync();
        Task<IEnumerable<MaterialDTO>> GetStockCriticoAsync();
        Task<bool> ActualizarStockAsync(int materialId, int cantidad);
        Task<bool> DescontarStockAsync(int materialId, decimal cantidad);
        Task<bool> VerificarDisponibilidadAsync(int materialId, decimal cantidadRequerida);
        Task<IEnumerable<ConsumoMaterialDTO>> GetHistorialConsumoAsync(int materialId);
        Task<IEnumerable<ConsumoMaterialDTO>> GetConsumosPorClienteAsync(int clienteId);
        Task VerificarYGenerarAlertasStockAsync();
    }
}

