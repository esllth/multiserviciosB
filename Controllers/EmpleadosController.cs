using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class EmpleadosController : Controller
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult IndexEmpleado()
        {
            return View();
        }

        [Authorize]
        public IActionResult Perfil()
        {
            return View();
        }
    }
}