using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.DTOs;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Services
{
    public class RolService : IRolService
    {
        private const string CorreoAdministradorPrincipal = "admin@multiserviciosb.com";
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RolService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IEnumerable<UsuarioRolDTO>> GetUsuariosConRolesAsync()
        {
            var usuarios = await _userManager.Users.ToListAsync();
            var resultado = new List<UsuarioRolDTO>();

            foreach (var usuario in usuarios)
            {
                await SincronizarTipoBaseAsync(usuario);
                var roles = await _userManager.GetRolesAsync(usuario);
                var esEmpleado = EsCorreoCorporativo(usuario.Email);
                resultado.Add(new UsuarioRolDTO
                {
                    Id = usuario.Id,
                    Email = usuario.Email ?? string.Empty,
                    UserName = usuario.UserName ?? string.Empty,
                    RolActual = esEmpleado ? "Empleado" : "Cliente",
                    EsEmpleado = esEmpleado,
                    EsAdministrador = roles.Contains("Administrador"),
                    EsSecretaria = roles.Contains("Secretaria"),
                    EsAdministradorPrincipal = EsAdministradorPrincipal(usuario)
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
                Email = usuario.Email ?? string.Empty,
                UserName = usuario.UserName ?? string.Empty,
                RolActual = roles.FirstOrDefault(),
                RolesDisponibles = rolesDisponibles.ToList()
            };
        }

        public async Task<IEnumerable<string>> GetRolesDisponiblesAsync()
        {
            return await _roleManager.Roles
                .Select(r => r.Name)
                .Where(nombre => nombre != null)
                .Select(nombre => nombre!)
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

        public async Task<(bool Exito, string Mensaje)> CambiarPermisoEmpleadoAsync(string userId, string permiso, bool asignar)
        {
            if (permiso != "Administrador")
                return (false, "El rol de Secretaría se define al crear el empleado y no puede modificarse desde esta pantalla.");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null)
                return (false, "El usuario no existe.");

            if (EsAdministradorPrincipal(usuario))
                return asignar
                    ? (true, "La cuenta del administrador principal ya está protegida.")
                    : (false, "No se puede retirar el rol al administrador principal del sistema.");

            await SincronizarTipoBaseAsync(usuario);
            if (!EsCorreoCorporativo(usuario.Email))
                return (false, "Solo los empleados con dominio @multiserviciosb.com pueden recibir este permiso.");

            var tienePermiso = await _userManager.IsInRoleAsync(usuario, permiso);
            if (tienePermiso == asignar)
                return (true, "El permiso ya estaba actualizado.");

            if (permiso == "Administrador" && !asignar)
            {
                var administradores = await _userManager.GetUsersInRoleAsync("Administrador");
                if (administradores.Count <= 1)
                    return (false, "No se puede retirar el permiso al último administrador del sistema.");
            }

            var resultado = asignar
                ? await _userManager.AddToRoleAsync(usuario, permiso)
                : await _userManager.RemoveFromRoleAsync(usuario, permiso);

            if (!resultado.Succeeded)
                return (false, string.Join(" ", resultado.Errors.Select(e => e.Description)));

            await _userManager.UpdateSecurityStampAsync(usuario);
            return (true, asignar ? $"Permiso {permiso} asignado." : $"Permiso {permiso} retirado.");
        }

        public async Task<(bool Exito, string Mensaje)> CambiarRolLaboralAsync(string userId, string rolLaboral)
        {
            if (rolLaboral is not ("Tecnico" or "Secretaria"))
                return (false, "Seleccione un rol laboral válido.");

            var usuario = await _userManager.FindByIdAsync(userId);
            if (usuario == null) return (false, "El usuario no existe.");
            if (EsAdministradorPrincipal(usuario))
                return (false, "El rol del administrador principal está protegido.");

            await SincronizarTipoBaseAsync(usuario);
            if (!EsCorreoCorporativo(usuario.Email))
                return (false, "Solo el personal con dominio @multiserviciosb.com puede tener un rol laboral interno.");

            var esSecretaria = await _userManager.IsInRoleAsync(usuario, "Secretaria");
            IdentityResult resultado;
            if (rolLaboral == "Secretaria" && !esSecretaria)
                resultado = await _userManager.AddToRoleAsync(usuario, "Secretaria");
            else if (rolLaboral == "Tecnico" && esSecretaria)
                resultado = await _userManager.RemoveFromRoleAsync(usuario, "Secretaria");
            else
                return (true, $"El usuario ya tiene el rol de {(rolLaboral == "Tecnico" ? "Técnico" : "Secretaría")}.");

            if (!resultado.Succeeded)
                return (false, string.Join(" ", resultado.Errors.Select(e => e.Description)));

            await _userManager.UpdateSecurityStampAsync(usuario);
            return (true, $"Rol actualizado a {(rolLaboral == "Tecnico" ? "Técnico" : "Secretaría")}.");
        }

        private async Task SincronizarTipoBaseAsync(IdentityUser usuario)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            if (EsAdministradorPrincipal(usuario))
            {
                if (!roles.Contains("Administrador"))
                    await _userManager.AddToRoleAsync(usuario, "Administrador");
                if (roles.Contains("Secretaria"))
                    await _userManager.RemoveFromRoleAsync(usuario, "Secretaria");
                roles = await _userManager.GetRolesAsync(usuario);
            }
            var esEmpleado = EsCorreoCorporativo(usuario.Email);
            var rolBase = esEmpleado ? "Empleado" : "Cliente";
            var rolIncompatible = esEmpleado ? "Cliente" : "Empleado";

            if (!roles.Contains(rolBase))
                await _userManager.AddToRoleAsync(usuario, rolBase);
            if (roles.Contains(rolIncompatible))
                await _userManager.RemoveFromRoleAsync(usuario, rolIncompatible);

            if (!esEmpleado)
            {
                foreach (var permiso in new[] { "Administrador", "Secretaria" })
                    if (roles.Contains(permiso)) await _userManager.RemoveFromRoleAsync(usuario, permiso);

                var perfilExterno = await _context.Empleados.FirstOrDefaultAsync(e => e.UserId == usuario.Id);
                if (perfilExterno != null)
                {
                    EstadosEmpleado.Aplicar(perfilExterno, EstadosEmpleado.Inactivo);
                    perfilExterno.TieneUsuario = false;
                    perfilExterno.UserId = null;
                    await _context.SaveChangesAsync();
                }
                return;
            }

            var email = usuario.Email!.Trim().ToLowerInvariant();
            var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.UserId == usuario.Id || e.CorreoElectronicoEmpleado.ToLower() == email);
            if (empleado == null)
            {
                var nombre = email.Split('@')[0].Replace('.', ' ').Replace('_', ' ').Trim();
                empleado = new Empleado
                {
                    IdentificacionEmpleado = $"USR-{usuario.Id[..Math.Min(8, usuario.Id.Length)].ToUpperInvariant()}",
                    NombreEmpleado = string.IsNullOrWhiteSpace(nombre) ? "Empleado" : nombre,
                    ApellidosEmpleado = "Pendiente de completar",
                    CorreoElectronicoEmpleado = email,
                    TelefonoEmpleado = "Pendiente",
                    SalarioBase = 0,
                    FechaInicioEmpleado = DateTime.UtcNow,
                    TieneUsuario = true,
                    UserId = usuario.Id
                };
                EstadosEmpleado.Aplicar(empleado, EstadosEmpleado.Activo);
                _context.Empleados.Add(empleado);
            }
            else
            {
                empleado.UserId = usuario.Id;
                empleado.TieneUsuario = true;
                EstadosEmpleado.Aplicar(empleado, EstadosEmpleado.Activo);
            }

            if (EsAdministradorPrincipal(usuario))
            {
                empleado.NombreEmpleado = "Administrador";
                empleado.ApellidosEmpleado = "de órdenes de servicio";
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.Correo != null && c.Correo.ToLower() == email);
            if (cliente != null) cliente.Estado = "Inactivo";
            await _context.SaveChangesAsync();
        }

        private static bool EsCorreoCorporativo(string? email) =>
            email?.Trim().EndsWith("@multiserviciosb.com", StringComparison.OrdinalIgnoreCase) == true;

        private static bool EsAdministradorPrincipal(IdentityUser usuario) =>
            string.Equals(usuario.Email?.Trim(), CorreoAdministradorPrincipal, StringComparison.OrdinalIgnoreCase);
    }
}
