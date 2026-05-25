using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class FabricacionAmedidaController : BaseController
    {
        [Authorize(Roles = "Administrador,Cliente")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador,Cliente")]
        public IActionResult FabricacionAmedida()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
