using Microsoft.AspNetCore.Mvc;
using MultiservicioB.Models;
using System.Diagnostics;

namespace MultiservicioB.Controllers
{
    public class HomeController : Controller
    {
        // HOME PUBLICO (revista o landing)
        public IActionResult Index()
        {
            // Si ya esta autenticado, lo mandamos al dashboard
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Dashboard");
            }

            return View(); // Layout publico
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
                return View("~/Views/Administrador/IndexAdmin.cshtml");

            if (User.IsInRole("Empleado"))
                return View("~/Views/Empleados/IndexEmpleado.cshtml");

            return View("~/Views/Cliente/IndexCliente.cshtml");
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