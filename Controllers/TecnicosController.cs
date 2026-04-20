using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class TecnicosController : Controller
    {
        [Authorize(Roles = "Empleado,Administrador")]
        public IActionResult Tecnicos()
        {
            return View();
        }
    }
}