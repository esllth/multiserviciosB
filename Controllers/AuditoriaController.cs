using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AuditoriaController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string? buscar, DateTime? desde, DateTime? hasta)
        {
            var consulta = _context.Auditorias.AsNoTracking().Include(a => a.Usuario).AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var termino = buscar.Trim();
                consulta = consulta.Where(a =>
                    (a.Accion != null && a.Accion.Contains(termino)) ||
                    (a.Detalle != null && a.Detalle.Contains(termino)) ||
                    (a.Usuario != null && a.Usuario.Email != null && a.Usuario.Email.Contains(termino)));
            }
            if (desde.HasValue) consulta = consulta.Where(a => a.Fecha >= desde.Value.Date);
            if (hasta.HasValue) consulta = consulta.Where(a => a.Fecha < hasta.Value.Date.AddDays(1));

            ViewBag.Buscar = buscar;
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            return View(await consulta.OrderByDescending(a => a.Fecha).Take(500).ToListAsync());
        }
    }
}
