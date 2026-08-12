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
            await Task.CompletedTask;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(UsuarioRolDTO dto)
        {
            await Task.CompletedTask;
            TempData["ErrorMessage"] = "El tipo de usuario se asigna automáticamente. Utilice los permisos disponibles en la tabla.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarRol(string userId, string rol)
        {
            await Task.CompletedTask;
            TempData["ErrorMessage"] = "El tipo de usuario no se puede retirar manualmente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarPermiso(string userId, string permiso, bool asignar)
        {
            var resultado = await _rolService.CambiarPermisoEmpleadoAsync(userId, permiso, asignar);
            TempData[resultado.Exito ? "SuccessMessage" : "ErrorMessage"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarRolLaboral(string userId, string rolLaboral)
        {
            var resultado = await _rolService.CambiarRolLaboralAsync(userId, rolLaboral);
            TempData[resultado.Exito ? "SuccessMessage" : "ErrorMessage"] = resultado.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}
