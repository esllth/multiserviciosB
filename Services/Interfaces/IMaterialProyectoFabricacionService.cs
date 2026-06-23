using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar materiales de proyectos de fabricación (RF-004)
    /// </summary>
    public interface IMaterialProyectoFabricacionService
    {
        Task<IEnumerable<MaterialProyectoFabricacionDTO>> GetByProyectoIdAsync(int proyectoId);
        Task<MaterialProyectoFabricacionDTO?> GetByIdAsync(int id);
        Task<MaterialProyectoFabricacionDTO> CreateAsync(MaterialProyectoFabricacionDTO materialProyectoDto);
        Task<bool> UpdateAsync(MaterialProyectoFabricacionDTO materialProyectoDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> VerificarDisponibilidadMaterialesAsync(int proyectoId);
        Task<decimal> CalcularCostoTotalMaterialesAsync(int proyectoId);
    }
}
