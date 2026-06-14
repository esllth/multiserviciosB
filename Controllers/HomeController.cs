using Microsoft.AspNetCore.Mvc;
using MultiservicioB.Models;
using System.Diagnostics;

namespace MultiservicioB.Controllers
{
    public class HomeController : BaseController
    {
        // HOME PUBLICO (revista o landing)
        public IActionResult Index()
        {
            // Si ya esta autenticado, lo mandamos al dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }

            return RedirectToAction("Index", "Revista");
        }

        // DASHBOARD SEGUN ROL
        public IActionResult Dashboard()
        {
            // Si NO esta logueado el login
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return Redirect("/Identity/Account/Login");
            }

            // Redirección por rol
            if (User.IsInRole("Administrador"))
                return View("~/Views/Administrador/Index.cshtml");

            if (User.IsInRole("Empleado"))
                return View("~/Views/Empleados/Dashboard.cshtml");

            if (User.IsInRole("Cliente"))
                return RedirectToAction("Index", "PortalCliente");

            return RedirectToPage(
                "/Account/AccessDenied",
                new { area = "Identity" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
