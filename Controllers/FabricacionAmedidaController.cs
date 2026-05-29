using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador,Cliente")]
    public class FabricacionAmedidaController : BaseController
    {
        private readonly IProyectoFabricacionService _proyectoService;

        public FabricacionAmedidaController(IProyectoFabricacionService proyectoService)
        {
            _proyectoService = proyectoService;
        }

        public async Task<IActionResult> Index()
        {
            var proyectos = await _proyectoService.GetAllAsync();
            return View(proyectos);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear(ProyectoFabricacionDTO proyectoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(proyectoDto);
            }

            await _proyectoService.CreateAsync(proyectoDto);
            TempData["SuccessMessage"] = "Proyecto de fabricación creado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(ProyectoFabricacionDTO proyectoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(proyectoDto);
            }

            var result = await _proyectoService.UpdateAsync(proyectoDto);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Proyecto de fabricación actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var result = await _proyectoService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Proyecto de fabricación eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Cliente")]
        public IActionResult FabricacionAmedida()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
