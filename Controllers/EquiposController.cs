using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class EquiposController : Controller
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult Equipos()
        {
            return View();
        }
    }
}