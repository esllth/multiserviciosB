using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class RolesController : BaseController
    {
        private readonly IRolService _rolService;

        public RolesController(IRolService rolService)
        {
            _rolService = rolService;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _rolService.GetUsuariosConRolesAsync();
            return View(usuarios);
        }

        public async Task<IActionResult> Asignar(string id)
        {
            var usuario = await _rolService.GetUsuarioByIdAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(UsuarioRolDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id) || string.IsNullOrWhiteSpace(dto.RolActual))
            {
                TempData["ErrorMessage"] = "Seleccione un rol válido.";
                return RedirectToAction(nameof(Index));
            }

            var resultado = await _rolService.AsignarRolAsync(dto.Id, dto.RolActual);
            if (!resultado)
            {
                TempData["ErrorMessage"] = "No se pudo asignar el rol. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Rol asignado correctamente al usuario {dto.Email}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarRol(string userId, string rol)
        {
            var resultado = await _rolService.QuitarRolAsync(userId, rol);
            if (!resultado)
            {
                TempData["ErrorMessage"] = "No se pudo quitar el rol. Intente nuevamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Rol removido correctamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}
