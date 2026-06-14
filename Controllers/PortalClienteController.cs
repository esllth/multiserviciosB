using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class PortalClienteController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public PortalClienteController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Cliente"))
            {
                var user = await _userManager.GetUserAsync(User);
                var email = user?.Email?.Trim().ToLowerInvariant();
                var tienePerfil = email != null && await _context.Clientes.AnyAsync(c =>
                    c.Correo != null &&
                    c.Correo.ToLower() == email &&
                    c.Estado == "Activo");

                if (!tienePerfil)
                {
                    return RedirectToAction("CompletarPerfil", "Cliente");
                }
            }

            return View();
        }
    }
}
