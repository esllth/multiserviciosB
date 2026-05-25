using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class PortalClienteController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
