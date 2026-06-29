using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdminController : BaseController
    {
        private const string TituloTrabajoCompletado = "Tecnico completo el trabajo";
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.NotificacionesTrabajoCompletado = await ObtenerAvisosTrabajoCompletadoAsync();
            ViewBag.CompromisosCalendario = await ObtenerCompromisosCalendarioAsync();
            return View("~/Views/Administrador/Index.cshtml");
        }

        public IActionResult IndexAdmin()
        {
            return RedirectToAction(nameof(Index));
        }

        private async Task<List<NotificacionTrabajoCompletadoViewModel>> ObtenerAvisosTrabajoCompletadoAsync()
        {
            return await _context.Notificaciones
                .AsNoTracking()
                .Where(n => n.Leida != true && n.OrdenId.HasValue && n.Titulo == TituloTrabajoCompletado)
                .OrderByDescending(n => n.Fecha)
                .Take(8)
                .Select(n => new NotificacionTrabajoCompletadoViewModel
                {
                    IdNotificacion = n.IdNotificacion,
                    IdOrden = n.OrdenId!.Value,
                    Mensaje = n.Mensaje,
                    Fecha = n.Fecha,
                    Cliente = n.Orden != null && n.Orden.Cliente != null
                        ? n.Orden.Cliente.Nombre + (n.Orden.Cliente.Apellidos != null ? " " + n.Orden.Cliente.Apellidos : "")
                        : null,
                    Tecnico = n.Orden != null && n.Orden.Empleado != null
                        ? n.Orden.Empleado.NombreEmpleado + " " + n.Orden.Empleado.ApellidosEmpleado
                        : null,
                    EstadoOrden = n.Orden != null && n.Orden.EstadoOrden != null ? n.Orden.EstadoOrden.Nombre : null
                })
                .ToListAsync();
        }

        private async Task<List<CompromisoCalendarioViewModel>> ObtenerCompromisosCalendarioAsync()
        {
            return await _context.OrdenesServicio
                .AsNoTracking()
                .Where(o => o.FechaCompromiso.HasValue)
                .OrderBy(o => o.FechaCompromiso)
                .Take(12)
                .Select(o => new CompromisoCalendarioViewModel
                {
                    IdOrden = o.IdOrden,
                    FechaCompromiso = o.FechaCompromiso!.Value,
                    Cliente = o.Cliente != null
                        ? o.Cliente.Nombre + (o.Cliente.Apellidos != null ? " " + o.Cliente.Apellidos : "")
                        : "Cliente no disponible",
                    TipoServicio = o.Cotizacion != null && o.Cotizacion.TipoServicio != null
                        ? o.Cotizacion.TipoServicio.Nombre
                        : "Servicio no disponible",
                    Tecnico = o.Empleado != null
                        ? o.Empleado.NombreEmpleado + " " + o.Empleado.ApellidosEmpleado
                        : null,
                    Confirmado = o.CompromisoConfirmado
                })
                .ToListAsync();
        }
    }
}
