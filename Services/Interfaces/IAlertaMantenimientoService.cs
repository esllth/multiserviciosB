using MultiservicioB.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar alertas de mantenimiento (RE-009, RE-010)
    /// </summary>
    public interface IAlertaMantenimientoService
    {
        Task<IEnumerable<AlertaMantenimientoDTO>> GetAllAsync();
        Task<IEnumerable<AlertaMantenimientoDTO>> GetByEquipoIdAsync(int equipoId);
        Task<IEnumerable<AlertaMantenimientoDTO>> GetPendientesAsync();
        Task<IEnumerable<AlertaMantenimientoDTO>> GetProximasAsync(int dias);
        Task<AlertaMantenimientoDTO?> GetByIdAsync(int id);
        Task<AlertaMantenimientoDTO> CreateAsync(AlertaMantenimientoDTO alertaDto);
        Task<bool> UpdateAsync(AlertaMantenimientoDTO alertaDto);
        Task<bool> DeleteAsync(int id);
        Task<bool> MarcarComoNotificadaAsync(int id);
        Task<bool> MarcarComoRealizadaAsync(int id);
        Task GenerarAlertasAutomaticasAsync();
        Task<bool> EnviarNotificacionMantenimientoAsync(int alertaId);
    }
}
