using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Services
{
    public class RolService : IRolService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IEnumerable<UsuarioRolDTO>> GetUsuariosConRolesAsync()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var resultado = new List<UsuarioRolDTO>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                resultado.Add(new UsuarioRolDTO
                {
                    Id = usuario.Id,
                    Email = usuario.Email,
                    UserName = usuario.UserName,
                    RolActual = roles.FirstOrDefault()
                });
            }

            return resultado;
        }

        public async Task<UsuarioRolDTO?> GetUsuarioByIdAsync(string id)
        {
            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return null;

            var roles = await _userManager.GetRolesAsync(usuario);
            var rolesDisponibles = await GetRolesDisponiblesAsync();

            return new UsuarioRolDTO
            {
                Id = usuario.Id,
                Email = usuario.Email,
                UserName = usuario.UserName,
                RolActual = roles.FirstOrDefault(),
                RolesDisponibles = rolesDisponibles.ToList()
            };
        }

        public async Task<IEnumerable<string>> GetRolesDisponiblesAsync()
        {
            return await _roleManager.Roles
                .Select(r => r.Name)
                .ToListAsync();
        }

        public async Task<bool> AsignarRolAsync(string userId, string nuevoRol)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null ||
                string.IsNullOrWhiteSpace(nuevoRol) ||
                !await _roleManager.RoleExistsAsync(nuevoRol))
            {
                return false;
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            if (rolesActuales.Count == 1 && rolesActuales.Contains(nuevoRol))
            {
                return true;
            }

            var rolNuevoAgregado = !rolesActuales.Contains(nuevoRol);
            if (rolNuevoAgregado)
            {
                var agregar = await _userManager.AddToRoleAsync(usuario, nuevoRol);
                if (!agregar.Succeeded)
                {
                    return false;
                }
            }

            var rolesAEliminar = rolesActuales.Where(r => r != nuevoRol).ToList();
            if (rolesAEliminar.Count > 0)
            {
                var eliminar = await _userManager.RemoveFromRolesAsync(usuario, rolesAEliminar);
                if (!eliminar.Succeeded)
                {
                    if (rolNuevoAgregado)
                    {
                        await _userManager.RemoveFromRoleAsync(usuario, nuevoRol);
                    }
                    return false;
                }
            }

            await _userManager.UpdateSecurityStampAsync(usuario);
            return true;
        }

        public async Task<bool> QuitarRolAsync(string userId, string rol)
        {
            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return false;

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            if (!rolesActuales.Contains(rol) || rolesActuales.Count <= 1)
            {
                return false;
            }

            var resultado = await _userManager.RemoveFromRoleAsync(usuario, rol);
            if (!resultado.Succeeded)
            {
                return false;
            }

            await _userManager.UpdateSecurityStampAsync(usuario);
            return true;
        }
    }
}
