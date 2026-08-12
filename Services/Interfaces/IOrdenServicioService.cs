using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    public interface IOrdenServicioService
    {
        Task<IEnumerable<OrdenServicioDTO>> GetAllAsync();
        Task<OrdenServicioDTO?> GetByIdAsync(int id);
        Task<IEnumerable<OrdenServicioDTO>> GetByTecnicoAsync(int empleadoId);
        Task<IEnumerable<OrdenServicioDTO>> GetByClienteAsync(int clienteId);
        Task<OrdenServicioDTO> CreateAsync(OrdenServicioDTO ordenDto);
        Task<bool> UpdateAsync(OrdenServicioDTO ordenDto);
        Task<bool> DeleteAsync(int id);

        // Nuevos métodos para historias de usuario RT
        Task<bool> ConfirmarLlegadaSitioAsync(int id, decimal latitud, decimal longitud);
        Task<bool> IniciarOrdenAsync(int id);
        Task<bool> FinalizarOrdenAsync(int id, string comentariosFinales);
        Task<bool> AceptarFinalizacionClienteAsync(int id);
        Task<bool> ActualizarObservacionesTecnicasAsync(int id, string observaciones);
        Task<bool> ValidarPuedeFinalizarAsync(int id);
        Task<int> CalcularTiempoEfectivoAsync(int id);
        Task<string?> ObtenerErrorStockMaterialesAsync(int id);
    }
}

