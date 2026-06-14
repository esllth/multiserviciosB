using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador,Empleado")]
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

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [Authorize(Roles = "Administrador")]
        public IActionResult Materiales()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
