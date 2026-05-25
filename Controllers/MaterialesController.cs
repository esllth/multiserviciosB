using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class MaterialesController : BaseController
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Materiales()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
