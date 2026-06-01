using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ServiciosController : BaseController
    {
        private readonly ITipoServicioService _tipoServicioService;

        public ServiciosController(ITipoServicioService tipoServicioService)
        {
            _tipoServicioService = tipoServicioService;
        }

        public async Task<IActionResult> Index()
        {
            var servicios = await _tipoServicioService.GetAllAsync();
            return View(servicios);
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(TipoServicioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _tipoServicioService.CrearAsync(dto);
            TempData["SuccessMessage"] = "Tipo de servicio creado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Editar(int id)
        {
            var servicio = await _tipoServicioService.GetByIdAsync(id);
            if (servicio == null) return NotFound();
            return View(servicio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(TipoServicioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _tipoServicioService.ActualizarAsync(dto);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Tipo de servicio actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _tipoServicioService.EliminarAsync(id);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Tipo de servicio eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }
    }
}