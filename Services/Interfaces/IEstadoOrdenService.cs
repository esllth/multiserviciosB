using MultiservicioB.DTOs;

namespace MultiservicioB.Services.Interfaces
{
    public interface IEstadoOrdenService
    {
        Task<IEnumerable<EstadoOrdenDTO>> GetAllAsync();
        Task<EstadoOrdenDTO?> GetByIdAsync(int id);
        Task<bool> CrearAsync(EstadoOrdenDTO dto);
        Task<bool> ActualizarAsync(EstadoOrdenDTO dto);
        Task<bool> EliminarAsync(int id);
    }
}