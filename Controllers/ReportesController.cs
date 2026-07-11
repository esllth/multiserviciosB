using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    // Solo Gerente y Administrador ven el panel.
    [Authorize(Roles = "Gerente,Administrador")]
    public class ReportesController : BaseController
    {
        private readonly ApplicationDbContext _context;

        // Nombres  de los estados 
        private const string EstadoPendiente = "Pendiente";
        private const string EstadoEnProgreso = "En Progreso";
        private const string EstadoCompletada = "Completada";
        private const string EstadoCancelada = "Cancelada";

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

      
        public async Task<IActionResult> Index(DateTime? inicio, DateTime? fin)
        {
            var modelo = await ConstruirIndicadoresAsync(inicio, fin);
            return View(modelo);
        }

       
        [HttpGet]
        public async Task<IActionResult> GetIndicadores(DateTime? inicio, DateTime? fin)
        {
            var modelo = await ConstruirIndicadoresAsync(inicio, fin);
            return Json(modelo);
        }

        private async Task<ReporteIndicadoresViewModel> ConstruirIndicadoresAsync(
            DateTime? inicio, DateTime? fin)
        {
            
            var ordenes = _context.OrdenesServicio.AsNoTracking().AsQueryable();

            if (inicio.HasValue)
            {
                var desde = inicio.Value.Date;
                ordenes = ordenes.Where(o => o.FechaCreacion >= desde);
            }

            if (fin.HasValue)
            {
        
                var hasta = fin.Value.Date.AddDays(1).AddTicks(-1);
                ordenes = ordenes.Where(o => o.FechaCreacion <= hasta);
            }

           
            var estados = await _context.EstadosOrden
                .AsNoTracking()
                .Select(e => new { e.Id, e.Nombre })
                .ToListAsync();

            int? IdDe(string nombre) => estados
                .FirstOrDefault(e => e.Nombre == nombre)?.Id;

            var idPendiente = IdDe(EstadoPendiente);
            var idEnProgreso = IdDe(EstadoEnProgreso);
            var idCompletada = IdDe(EstadoCompletada);
            var idCancelada = IdDe(EstadoCancelada);

            var pendientes = idPendiente is null ? 0
                : await ordenes.CountAsync(o => o.EstadoOrdenId == idPendiente);
            var enProgreso = idEnProgreso is null ? 0
                : await ordenes.CountAsync(o => o.EstadoOrdenId == idEnProgreso);
            var completadas = idCompletada is null ? 0
                : await ordenes.CountAsync(o => o.EstadoOrdenId == idCompletada);
            var canceladas = idCancelada is null ? 0
                : await ordenes.CountAsync(o => o.EstadoOrdenId == idCancelada);

            var total = await ordenes.CountAsync();

            
            var porTecnico = await ordenes
                .GroupBy(o => o.EmpleadoId)
                .Select(g => new
                {
                    g.Key,
                    Cantidad = g.Count(),
                    Nombre = g.Key == null
                        ? "Sin asignar"
                        : _context.Empleados
                            .Where(e => e.IdEmpleado == g.Key)
                            .Select(e => e.NombreEmpleado + " " + e.ApellidosEmpleado)
                            .FirstOrDefault()
                })
                .ToListAsync();

            
            var porTipo = await ordenes
                .Join(_context.Cotizaciones.AsNoTracking(),
                    o => o.CotizacionId,
                    c => c.IdCotizacion,
                    (o, c) => c.TipoServicioId)
                .Join(_context.TiposServicio.AsNoTracking(),
                    tipoId => tipoId,
                    t => t.Id,
                    (tipoId, t) => t.Nombre)
                .GroupBy(nombre => nombre)
                .Select(g => new ConteoPorCategoria
                {
                    Nombre = g.Key,
                    Cantidad = g.Count()
                })
                .ToListAsync();

            return new ReporteIndicadoresViewModel
            {
                Pendientes = pendientes,
                EnProgreso = enProgreso,
                Completadas = completadas,
                Canceladas = canceladas,
                Total = total,
                FechaInicio = inicio,
                FechaFin = fin,
                PorTecnico = porTecnico
                    .Select(x => new ConteoPorCategoria
                    {
                        Nombre = x.Nombre ?? "Sin asignar",
                        Cantidad = x.Cantidad
                    })
                    .OrderByDescending(x => x.Cantidad)
                    .ToList(),
                PorTipoServicio = porTipo
                    .OrderByDescending(x => x.Cantidad)
                    .ToList()
            };
        }
    }
}