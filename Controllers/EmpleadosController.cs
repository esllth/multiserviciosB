using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class EmpleadosController : Controller
{
    [Authorize(Roles = "Administrador")]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize]
    public IActionResult Perfil()
    {
        return View();
    }
}