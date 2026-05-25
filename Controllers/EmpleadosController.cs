using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class EmpleadosController : BaseController
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult IndexEmpleado()
        {
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult Perfil()
        {
            return View();
        }
    }
}
