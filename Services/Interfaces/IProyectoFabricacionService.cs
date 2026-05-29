using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    public interface IProyectoFabricacionService
    {
        Task<IEnumerable<ProyectoFabricacionDTO>> GetAllAsync();
        Task<ProyectoFabricacionDTO?> GetByIdAsync(int id);
        Task<IEnumerable<ProyectoFabricacionDTO>> GetByClienteAsync(int clienteId);
        Task<ProyectoFabricacionDTO> CreateAsync(ProyectoFabricacionDTO proyectoDto);
        Task<bool> UpdateAsync(ProyectoFabricacionDTO proyectoDto);
        Task<bool> DeleteAsync(int id);
    }
}
