using MultiservicioB.DTOs;

namespace MultiservicioB.Services.Interfaces
{
    public interface IRolService
    {
        Task<IEnumerable<UsuarioRolDTO>> GetUsuariosConRolesAsync();
        Task<UsuarioRolDTO?> GetUsuarioByIdAsync(string id);
        Task<IEnumerable<string>> GetRolesDisponiblesAsync();
        Task<bool> AsignarRolAsync(string userId, string nuevoRol);
        Task<bool> QuitarRolAsync(string userId, string rol);
        Task<(bool Exito, string Mensaje)> CambiarPermisoEmpleadoAsync(string userId, string permiso, bool asignar);
        Task<(bool Exito, string Mensaje)> CambiarRolLaboralAsync(string userId, string rolLaboral);
    }
}
