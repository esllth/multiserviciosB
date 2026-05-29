using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    public interface IMaterialService
    {
        Task<IEnumerable<MaterialDTO>> GetAllAsync();
        Task<MaterialDTO?> GetByIdAsync(int id);
        Task<MaterialDTO> CreateAsync(MaterialDTO materialDto);
        Task<bool> UpdateAsync(MaterialDTO materialDto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<MaterialDTO>> GetBajoStockAsync();
    }
}
