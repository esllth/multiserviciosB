using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class TecnicosController : BaseController
    {
        [Authorize(Roles = "Empleado,Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Empleado,Administrador")]
        public IActionResult Tecnicos()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
