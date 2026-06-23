using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar eventos de órdenes de servicio (RT-001, RT-002, RT-003, RT-006, RT-007, RT-008)
    /// </summary>
    public interface IEventoOrdenService
    {
        Task<IEnumerable<EventoOrdenServicioDTO>> GetByOrdenIdAsync(int ordenId);
        Task<EventoOrdenServicioDTO?> GetByIdAsync(int id);
        Task<EventoOrdenServicioDTO> CreateAsync(EventoOrdenServicioDTO eventoDto);
        Task<bool> RegistrarLlegadaSitioAsync(int ordenId, decimal latitud, decimal longitud, string usuarioId);
        Task<bool> RegistrarInicioServicioAsync(int ordenId, string usuarioId);
        Task<bool> RegistrarObservacionTecnicaAsync(int ordenId, string descripcion, string usuarioId);
        Task<bool> RegistrarFinalizacionServicioAsync(int ordenId, string comentarioFinal, string usuarioId);
        Task<bool> RegistrarAceptacionClienteAsync(int ordenId, string usuarioId);
    }
}
