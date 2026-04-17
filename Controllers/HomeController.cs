using Microsoft.AspNetCore.Mvc;
using MultiservicioB.Models;
using System.Diagnostics;

namespace MultiservicioB.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
