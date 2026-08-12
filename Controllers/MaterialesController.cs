using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador,Empleado,Secretaria")]
    public class MaterialesController : BaseController
    {
        private readonly IMaterialService _materialService;

        public MaterialesController(IMaterialService materialService)
        {
            _materialService = materialService;
        }

        public async Task<IActionResult> Index()
        {
            var materiales = await _materialService.GetAllAsync();
            return View(materiales);
        }

        [Authorize(Roles = "Administrador,Secretaria")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<IActionResult> Crear(MaterialDTO materialDto)
        {
            if (!ModelState.IsValid)
            {
                return View(materialDto);
            }

            await _materialService.CreateAsync(materialDto);
            TempData["SuccessMessage"] = "Material creado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(MaterialDTO materialDto)
        {
            if (!ModelState.IsValid)
            {
                return View(materialDto);
            }

            var result = await _materialService.UpdateAsync(materialDto);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Material actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null)
            {
                return NotFound();
            }
            return View(material);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var result = await _materialService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Material eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> BajoStock()
        {
            var materiales = await _materialService.GetBajoStockAsync();
            return View(materiales);
        }

        public async Task<IActionResult> Historial(int id)
        {
            var material = await _materialService.GetByIdAsync(id);
            if (material == null) return NotFound();

            return View(new MaterialHistorialViewModel
            {
                Material = material,
                Movimientos = (await _materialService.GetHistorialConsumoAsync(id)).ToList()
            });
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Materiales()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
