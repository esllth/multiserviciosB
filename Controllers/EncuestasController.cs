using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class EncuestasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}