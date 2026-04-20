using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class MaterialesController : Controller
    {
        [Authorize(Roles = "Administrador")]
        public IActionResult Materiales()
        {
            return View();
        }
    }
}