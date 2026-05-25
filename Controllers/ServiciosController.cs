using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ServiciosController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
