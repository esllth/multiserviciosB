using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Administrador")]
    public class OperacionesController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
