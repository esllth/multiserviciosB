using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : BaseController
    {
        public IActionResult Index()
        {
            return View("~/Views/Administrador/Index.cshtml");
        }

        public IActionResult IndexAdmin()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
