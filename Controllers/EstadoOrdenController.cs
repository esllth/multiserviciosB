using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EstadoOrdenController : BaseController
    {
        private readonly IEstadoOrdenService _estadoOrdenService;

        public EstadoOrdenController(IEstadoOrdenService estadoOrdenService)
        {
            _estadoOrdenService = estadoOrdenService;
        }

        public async Task<IActionResult> Index()
        {
            var estados = await _estadoOrdenService.GetAllAsync();
            return View(estados);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(EstadoOrdenDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _estadoOrdenService.CrearAsync(dto);
            if (!resultado)
            {
                TempData["ErrorMessage"] = $"El estado '{dto.Nombre}' ya existe en el sistema.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = $"Estado '{dto.Nombre}' creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var estado = await _estadoOrdenService.GetByIdAsync(id);
            if (estado == null) return NotFound();
            return View(estado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(EstadoOrdenDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _estadoOrdenService.ActualizarAsync(dto);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = $"Estado '{dto.Nombre}' actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _estadoOrdenService.EliminarAsync(id);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Estado eliminado exitosamente.";
            return RedirectToAction(nameof(Index));
        }
    }
}

