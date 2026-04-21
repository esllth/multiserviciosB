using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class ClienteController : Controller
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult IndexCliente()
        {
            return View();
        }

        [Authorize(Roles = "Cliente,Administrador")]
        public IActionResult Perfil()
        {
            return View();
        }
    }
}