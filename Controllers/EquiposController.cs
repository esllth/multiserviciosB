using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EquiposController : BaseController
    {
        private readonly IEquipoService _equipoService;

        public EquiposController(IEquipoService equipoService)
        {
            _equipoService = equipoService;
        }

        public async Task<IActionResult> Index()
        {
            var equipos = await _equipoService.GetAllAsync();
            return View(equipos);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(EquipoDTO equipoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(equipoDto);
            }

            await _equipoService.CreateAsync(equipoDto);
            TempData["SuccessMessage"] = "Equipo creado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var equipo = await _equipoService.GetByIdAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            return View(equipo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EquipoDTO equipoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(equipoDto);
            }

            var result = await _equipoService.UpdateAsync(equipoDto);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Equipo actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Eliminar(int id)
        {
            var equipo = await _equipoService.GetByIdAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            return View(equipo);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var result = await _equipoService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Equipo eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Equipos()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
