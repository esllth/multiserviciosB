using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class EncuestasController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncuestaService _encuestaService;
        private readonly UserManager<IdentityUser> _userManager;

        public EncuestasController(ApplicationDbContext context, IEncuestaService encuestaService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _encuestaService = encuestaService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Cliente,Administrador")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Cliente"))
            {
                var correo = (await _userManager.GetUserAsync(User))?.Email;
                var encuestasCliente = string.IsNullOrWhiteSpace(correo)
                    ? new List<EncuestaClienteViewModel>()
                    : (await _encuestaService.ObtenerEncuestasClienteAsync(correo)).ToList();

                return View(new EncuestasIndexViewModel
                {
                    EsCliente = true,
                    EncuestasCliente = encuestasCliente
                });
            }

            var resultados = await _context.Encuestas
                .AsNoTracking()
                .Where(e => e.CalificacionServicio != null && e.CalificacionTecnico != null)
                .OrderByDescending(e => e.Fecha)
                .Select(e => new EncuestaResultadoViewModel
                {
                    OrdenId = e.OrdenId,
                    Cliente = e.Cliente != null
                        ? e.Cliente.Nombre + (e.Cliente.Apellidos != null ? " " + e.Cliente.Apellidos : "")
                        : "Cliente no disponible",
                    Tecnico = e.Orden != null && e.Orden.Empleado != null
                        ? e.Orden.Empleado.NombreEmpleado + " " + e.Orden.Empleado.ApellidosEmpleado
                        : "Sin técnico asignado",
                    CalificacionServicio = e.CalificacionServicio ?? 0,
                    CalificacionTecnico = e.CalificacionTecnico ?? 0,
                    Comentarios = e.Comentarios,
                    Fecha = e.Fecha
                })
                .ToListAsync();

            return View(new EncuestasIndexViewModel
            {
                TotalRespuestas = resultados.Count,
                PromedioServicio = resultados.Count == 0 ? 0 : resultados.Average(e => e.CalificacionServicio),
                PromedioTecnico = resultados.Count == 0 ? 0 : resultados.Average(e => e.CalificacionTecnico),
                Resultados = resultados
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Guardar(ResponderEncuestaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Seleccione ambas calificaciones antes de guardar.";
                return RedirectToAction(nameof(Index));
            }

            var correoUsuario = (await _userManager.GetUserAsync(User))?.Email;
            if (string.IsNullOrWhiteSpace(correoUsuario) ||
                !await _encuestaService.GuardarRespuestaClienteAsync(correoUsuario, model))
            {
                TempData["ErrorMessage"] = "La encuesta no está disponible o ya fue respondida.";
                return RedirectToAction(nameof(Index));
            }

            TempData["SuccessMessage"] = "Gracias por compartir su experiencia.";
            return RedirectToAction(nameof(Index));
        }
    }
}
