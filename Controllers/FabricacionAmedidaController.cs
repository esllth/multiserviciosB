using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class FabricacionAmedidaController : Controller
    {
        [Authorize(Roles = "Administrador,Cliente")]
        public IActionResult FabricacionAmedida()
        {
            return View();
        }
    }
}