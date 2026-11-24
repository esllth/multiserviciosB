using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CotizacionAprobacionController : Controller
    {
        private readonly ICotizacionAprobacionService _service;
        private readonly UserManager<IdentityUser> _userManager;

        public CotizacionAprobacionController(
            ICotizacionAprobacionService service,
            UserManager<IdentityUser> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Aprobar(int cotizacionId, bool confirmarDocumentos)
        {
            if (!confirmarDocumentos)
            {
                TempData["ErrorMessage"] = "Debe confirmar que revisó los documentos antes de aprobar.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return Challenge();

            var resultado = await _service.AprobarAsync(cotizacionId, usuario.Id);
            if (!resultado.Exito)
            {
                TempData["ErrorMessage"] = resultado.Error;
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            TempData["SuccessMessage"] = $"Cotización aprobada y orden #{resultado.OrdenId} creada automáticamente.";
            return RedirectToAction("Detalle", "Tecnicos", new { id = resultado.OrdenId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Rechazar(int cotizacionId, string motivo)
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return Challenge();

            var resultado = await _service.RechazarAsync(cotizacionId, usuario.Id, motivo);
            if (!resultado.Exito)
            {
                TempData["ErrorMessage"] = resultado.Error;
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            TempData["SuccessMessage"] = "Cotización rechazada correctamente.";
            return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
        }
    }
}
