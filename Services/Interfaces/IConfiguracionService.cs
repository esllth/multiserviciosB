using MultiservicioB.DTOs;

namespace MultiservicioB.Services.Interfaces
{
    public interface IConfiguracionService
    {
        // Horarios
        Task<IEnumerable<HorarioDTO>> GetHorariosAsync();
        Task<HorarioDTO?> GetHorarioByIdAsync(int id);
        Task<bool> CrearHorarioAsync(HorarioDTO dto);
        Task<bool> ActualizarHorarioAsync(HorarioDTO dto);
        Task<bool> EliminarHorarioAsync(int id);

        // Zonas
        Task<IEnumerable<ZonaDTO>> GetZonasAsync();
        Task<ZonaDTO?> GetZonaByIdAsync(int id);
        Task<bool> CrearZonaAsync(ZonaDTO dto);
        Task<bool> ActualizarZonaAsync(ZonaDTO dto);
        Task<bool> EliminarZonaAsync(int id);

        // Configuracion General
        Task<IEnumerable<ConfiguracionSistemaDTO>> GetConfiguracionesAsync();
        Task<ConfiguracionSistemaDTO?> GetConfiguracionByClaveAsync(string clave);
        Task<bool> ActualizarConfiguracionAsync(ConfiguracionSistemaDTO dto);
        Task<RevistaNosotrosDTO> GetRevistaNosotrosAsync();
        Task GuardarRevistaNosotrosAsync(RevistaNosotrosDTO dto);
    }
}
