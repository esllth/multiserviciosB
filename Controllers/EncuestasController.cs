using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class EncuestasController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EncuestasController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var resultados = await _context.Encuestas
                .AsNoTracking()
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

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Responder(int ordenId, int? calificacionServicio = null)
        {
            var orden = await ObtenerOrdenCompletadaDelClienteAsync(ordenId);
            if (orden == null) return NotFound();

            if (await _context.Encuestas.AnyAsync(e => e.OrdenId == ordenId))
            {
                TempData["SuccessMessage"] = "La encuesta de esta orden ya fue respondida. Gracias por su opinión.";
                return RedirectToAction("Detalle", "Tecnicos", new { id = ordenId });
            }

            return View(new ResponderEncuestaViewModel
            {
                OrdenId = ordenId,
                CalificacionServicio = calificacionServicio is >= 1 and <= 5
                    ? calificacionServicio
                    : null
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Responder(ResponderEncuestaViewModel model)
        {
            var orden = await ObtenerOrdenCompletadaDelClienteAsync(model.OrdenId);
            if (orden == null) return NotFound();

            if (await _context.Encuestas.AnyAsync(e => e.OrdenId == model.OrdenId))
            {
                TempData["SuccessMessage"] = "La encuesta de esta orden ya fue respondida.";
                return RedirectToAction("Detalle", "Tecnicos", new { id = model.OrdenId });
            }

            if (!ModelState.IsValid) return View(model);

            _context.Encuestas.Add(new Encuesta
            {
                OrdenId = orden.IdOrden,
                ClienteId = orden.ClienteId,
                CalificacionServicio = model.CalificacionServicio,
                CalificacionTecnico = model.CalificacionTecnico,
                Comentarios = model.Comentarios?.Trim(),
                Fecha = DateTime.Today
            });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Gracias por compartir su experiencia.";
            return RedirectToAction("Detalle", "Tecnicos", new { id = model.OrdenId });
        }

        private async Task<OrdenServicio?> ObtenerOrdenCompletadaDelClienteAsync(int ordenId)
        {
            var user = await _userManager.GetUserAsync(User);
            var email = user?.Email?.Trim().ToLowerInvariant();
            if (email == null) return null;

            return await _context.OrdenesServicio
                .AsNoTracking()
                .Include(o => o.Cliente)
                .Include(o => o.EstadoOrden)
                .FirstOrDefaultAsync(o => o.IdOrden == ordenId &&
                    o.Cliente != null && o.Cliente.Correo != null &&
                    o.Cliente.Correo.ToLower() == email &&
                    o.EstadoOrden != null && o.EstadoOrden.Nombre == "Completada");
        }
    }
}
