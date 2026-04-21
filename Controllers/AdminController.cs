using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : Controller
    {
        public IActionResult IndexAdmin()
        {
            return View();
        }
    }
}