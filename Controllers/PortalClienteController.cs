using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class PortalClienteController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEncuestaService _encuestaService;

        public PortalClienteController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IEncuestaService encuestaService)
        {
            _context = context;
            _userManager = userManager;
            _encuestaService = encuestaService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var eventos = await ObtenerEventosClienteAsync(DateTime.Today, DateTime.MaxValue);
            var correo = (await _userManager.GetUserAsync(User))?.Email;
            return View(new CalendarioClienteViewModel
            {
                Mes = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                Eventos = eventos,
                TieneEncuestasPendientes = !string.IsNullOrWhiteSpace(correo) &&
                    await _encuestaService.TieneEncuestasPendientesClienteAsync(correo)
            });
        }

        public async Task<IActionResult> Index(int? anio, int? mes)
        {
            var fecha = DateTime.Today;
            var mesCalendario = new DateTime(anio ?? fecha.Year, mes ?? fecha.Month, 1);
            var inicio = mesCalendario.AddMonths(-1);
            var fin = mesCalendario.AddMonths(2);
            var eventos = await ObtenerEventosClienteAsync(inicio, fin);

            return View(new CalendarioClienteViewModel
            {
                Mes = mesCalendario,
                Eventos = eventos
            });
        }

        private async Task<List<EventoCalendarioClienteViewModel>> ObtenerEventosClienteAsync(DateTime inicio, DateTime fin)
        {
            var usuario = await _userManager.GetUserAsync(User);
            var correo = usuario?.Email?.Trim().ToLower();
            if (correo == null)
            {
                return new List<EventoCalendarioClienteViewModel>();
            }

            return await _context.OrdenesServicio
                .AsNoTracking()
                .Where(o => o.Cliente != null && o.Cliente.Correo != null &&
                    o.Cliente.Correo.ToLower() == correo && o.Cliente.Estado == "Activo" &&
                    o.CompromisoConfirmado && o.FechaCompromiso >= inicio && o.FechaCompromiso < fin &&
                    o.EstadoOrden != null && o.EstadoOrden.Nombre != "Completada" && o.EstadoOrden.Nombre != "Cancelada")
                .OrderBy(o => o.FechaCompromiso)
                .Select(o => new EventoCalendarioClienteViewModel
                {
                    IdOrden = o.IdOrden,
                    FechaCompromiso = o.FechaCompromiso!.Value,
                    TipoServicio = o.Cotizacion != null && o.Cotizacion.TipoServicio != null ? o.Cotizacion.TipoServicio.Nombre : "Servicio",
                    EstadoOrden = o.EstadoOrden!.Nombre
                })
                .ToListAsync();
        }
    }
}
