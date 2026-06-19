using Microsoft.AspNetCore.Http;
using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar fotos de órdenes de servicio (RT-004)
    /// </summary>
    public interface IFotoOrdenService
    {
        Task<IEnumerable<FotoOrdenServicioDTO>> GetByOrdenIdAsync(int ordenId);
        Task<IEnumerable<FotoOrdenServicioDTO>> GetByOrdenIdAndTipoAsync(int ordenId, string tipoFoto);
        Task<FotoOrdenServicioDTO?> GetByIdAsync(int id);
        Task<FotoOrdenServicioDTO> CreateAsync(FotoOrdenServicioDTO fotoDto, IFormFile archivo);
        Task<bool> DeleteAsync(int id);
        Task<bool> ValidarFotosObligatoriasAsync(int ordenId);
        Task<bool> TieneFotosInicioAsync(int ordenId);
        Task<bool> TieneFotosFinAsync(int ordenId);
    }
}
