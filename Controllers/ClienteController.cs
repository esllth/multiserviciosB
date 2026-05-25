using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class ClienteController : BaseController
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult IndexCliente()
        {
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Cliente,Administrador")]
        public IActionResult Perfil()
        {
            return View();
        }
    }
}
