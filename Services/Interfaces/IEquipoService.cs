using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    public interface IEquipoService
    {
        Task<IEnumerable<EquipoDTO>> GetAllAsync();
        Task<EquipoDTO?> GetByIdAsync(int id);
        Task<IEnumerable<EquipoDTO>> GetByClienteAsync(int clienteId);
        Task<EquipoDTO> CreateAsync(EquipoDTO equipoDto);
        Task<bool> UpdateAsync(EquipoDTO equipoDto);
        Task<bool> DeleteAsync(int id);
    }
}
