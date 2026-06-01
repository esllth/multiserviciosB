using MultiservicioB.DTOs;

namespace MultiservicioB.Services.Interfaces
{
    public interface ITipoServicioService
    {
        Task<IEnumerable<TipoServicioDTO>> GetAllAsync();
        Task<TipoServicioDTO?> GetByIdAsync(int id);
        Task<bool> CrearAsync(TipoServicioDTO dto);
        Task<bool> ActualizarAsync(TipoServicioDTO dto);
        Task<bool> EliminarAsync(int id);
    }
}