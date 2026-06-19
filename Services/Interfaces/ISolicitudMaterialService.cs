using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar solicitudes de materiales (RM-007)
    /// </summary>
    public interface ISolicitudMaterialService
    {
        Task<IEnumerable<SolicitudMaterialDTO>> GetAllAsync();
        Task<IEnumerable<SolicitudMaterialDTO>> GetByOrdenIdAsync(int ordenId);
        Task<IEnumerable<SolicitudMaterialDTO>> GetByEstadoAsync(string estado);
        Task<IEnumerable<SolicitudMaterialDTO>> GetPendientesAsync();
        Task<SolicitudMaterialDTO?> GetByIdAsync(int id);
        Task<SolicitudMaterialDTO> CreateAsync(SolicitudMaterialDTO solicitudDto);
        Task<bool> AprobarSolicitudAsync(int id, string respuestaAdmin);
        Task<bool> RechazarSolicitudAsync(int id, string respuestaAdmin);
        Task<bool> MarcarComoEntregadaAsync(int id);
    }
}
