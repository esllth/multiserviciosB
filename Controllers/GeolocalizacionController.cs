using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class GeolocalizacionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}