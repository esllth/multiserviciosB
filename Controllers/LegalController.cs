using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
    public class LegalController : Controller
    {
        public IActionResult Terminos()
        {
            return View();
        }
    }
}
