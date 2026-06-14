using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador")]
    public class CotizacionesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CotizacionesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? estadoCotizacionId, string? cliente)
        {
            var query = _context.Cotizaciones.AsNoTracking().AsQueryable();

            if (!User.IsInRole("Administrador"))
            {
                var clienteActual = await ObtenerClienteActualAsync();
                if (clienteActual == null)
                {
                    return RedirectToAction("CompletarPerfil", "Cliente");
                }

                query = query.Where(c => c.ClienteId == clienteActual.IdCliente);
            }
            else if (!string.IsNullOrWhiteSpace(cliente))
            {
                var termino = cliente.Trim();
                query = query.Where(c =>
                    c.Cliente != null &&
                    (c.Cliente.Nombre.Contains(termino) ||
                     (c.Cliente.Apellidos != null && c.Cliente.Apellidos.Contains(termino)) ||
                     c.Cliente.Identificacion.Contains(termino)));
            }

            if (estadoCotizacionId.HasValue)
            {
                query = query.Where(c => c.EstadoCotizacionId == estadoCotizacionId.Value);
            }

            var cotizaciones = await query
                .OrderByDescending(c => c.FechaSolicitud)
                .Select(c => new CotizacionListItemViewModel
                {
                    IdCotizacion = c.IdCotizacion,
                    Cliente = c.Cliente != null
                        ? c.Cliente.Nombre + (c.Cliente.Apellidos != null ? " " + c.Cliente.Apellidos : "")
                        : "",
                    TipoServicio = c.TipoServicio != null ? c.TipoServicio.Nombre : "",
                    Estado = c.EstadoCotizacion != null ? c.EstadoCotizacion.Nombre : "",
                    Descripcion = c.Descripcion,
                    MontoPresupuesto = c.MontoPresupuesto,
                    FechaSolicitud = c.FechaSolicitud,
                    AprobadaPorCliente = c.AprobadaPorCliente
                })
                .ToListAsync();

            ViewBag.EstadosCotizacion = new SelectList(
                await _context.EstadosCotizacion.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
                "Id",
                "Nombre",
                estadoCotizacionId);
            ViewBag.Cliente = cliente;

            return View(cotizaciones);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var cotizacion = await ConsultaPermitidaAsync()
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.TipoServicio)
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            return cotizacion == null ? NotFound() : View(cotizacion);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Solicitar()
        {
            await CargarTiposServicioAsync();
            return View(new SolicitarCotizacionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Solicitar(SolicitarCotizacionViewModel model)
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            var tipoValido = model.TipoServicioId.HasValue &&
                await _context.TiposServicio.AnyAsync(t => t.Id == model.TipoServicioId.Value && t.Estado == "Activo");

            if (!tipoValido)
            {
                ModelState.AddModelError(nameof(model.TipoServicioId), "Seleccione un tipo de servicio activo.");
            }

            if (!ModelState.IsValid)
            {
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            var estadoPendiente = await ObtenerEstadoAsync("Pendiente");
            _context.Cotizaciones.Add(new Cotizacion
            {
                ClienteId = cliente.IdCliente,
                TipoServicioId = model.TipoServicioId!.Value,
                EstadoCotizacionId = estadoPendiente.Id,
                Descripcion = model.Descripcion.Trim(),
                FechaSolicitud = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Solicitud de cotización registrada.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Editar(int id)
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            var cotizacion = await _context.Cotizaciones
                .AsNoTracking()
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id && c.ClienteId == cliente.IdCliente);
            if (cotizacion == null)
            {
                return NotFound();
            }

            if (cotizacion.EstadoCotizacion?.Nombre != "Pendiente")
            {
                TempData["ErrorMessage"] = "Solo se pueden modificar cotizaciones pendientes.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            await CargarTiposServicioAsync(cotizacion.TipoServicioId);
            return View(new SolicitarCotizacionViewModel
            {
                TipoServicioId = cotizacion.TipoServicioId,
                Descripcion = cotizacion.Descripcion ?? ""
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Editar(int id, SolicitarCotizacionViewModel model)
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id && c.ClienteId == cliente.IdCliente);
            if (cotizacion == null)
            {
                return NotFound();
            }

            if (cotizacion.EstadoCotizacion?.Nombre != "Pendiente")
            {
                TempData["ErrorMessage"] = "La cotización ya no está pendiente y no puede modificarse.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            var tipoValido = model.TipoServicioId.HasValue &&
                await _context.TiposServicio.AnyAsync(t =>
                    t.Id == model.TipoServicioId.Value &&
                    t.Estado == "Activo");
            if (!tipoValido)
            {
                ModelState.AddModelError(nameof(model.TipoServicioId), "Seleccione un tipo de servicio activo.");
            }

            if (!ModelState.IsValid)
            {
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            cotizacion.TipoServicioId = model.TipoServicioId!.Value;
            cotizacion.Descripcion = model.Descripcion.Trim();
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cotización actualizada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Evaluar(int id)
        {
            var cotizacion = await _context.Cotizaciones.AsNoTracking().FirstOrDefaultAsync(c => c.IdCotizacion == id);
            return cotizacion == null
                ? NotFound()
                : View(new EvaluarCotizacionViewModel { IdCotizacion = id, MontoPresupuesto = cotizacion.MontoPresupuesto });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Evaluar(int id, EvaluarCotizacionViewModel model)
        {
            if (id != model.IdCotizacion)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cotizacion = await _context.Cotizaciones.FirstOrDefaultAsync(c => c.IdCotizacion == id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            var estadoActual = await _context.EstadosCotizacion
                .Where(e => e.Id == cotizacion.EstadoCotizacionId)
                .Select(e => e.Nombre)
                .SingleAsync();
            if (estadoActual == "Aprobada" || estadoActual == "Rechazada")
            {
                return BadRequest("No puede modificar una cotización que ya fue respondida por el cliente.");
            }

            cotizacion.MontoPresupuesto = model.MontoPresupuesto;
            cotizacion.EstadoCotizacionId = (await ObtenerEstadoAsync("Evaluada")).Id;
            cotizacion.AprobadaPorCliente = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cotización evaluada y enviada al cliente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Responder(int id, bool aprobar)
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id && c.ClienteId == cliente.IdCliente);

            if (cotizacion == null)
            {
                return NotFound();
            }

            if (cotizacion.EstadoCotizacion?.Nombre != "Evaluada")
            {
                return BadRequest("Solo puede responder cotizaciones evaluadas.");
            }

            cotizacion.AprobadaPorCliente = aprobar;
            cotizacion.EstadoCotizacionId = (await ObtenerEstadoAsync(aprobar ? "Aprobada" : "Rechazada")).Id;

            if (aprobar && !await _context.OrdenesServicio.AnyAsync(o => o.CotizacionId == cotizacion.IdCotizacion))
            {
                var estadoPendiente = await _context.EstadosOrden.SingleAsync(e => e.Nombre == "Pendiente");
                _context.OrdenesServicio.Add(new OrdenServicio
                {
                    CotizacionId = cotizacion.IdCotizacion,
                    ClienteId = cotizacion.ClienteId,
                    EmpleadoId = null,
                    EstadoOrdenId = estadoPendiente.Id,
                    FechaCreacion = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = aprobar ? "Cotización aprobada." : "Cotización rechazada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        private IQueryable<Cotizacion> ConsultaPermitidaAsync()
        {
            if (User.IsInRole("Administrador"))
            {
                return _context.Cotizaciones;
            }

            var email = User.Identity?.Name?.Trim().ToLower();
            return _context.Cotizaciones.Where(c =>
                c.Cliente != null &&
                c.Cliente.Correo != null &&
                c.Cliente.Correo.ToLower() == email &&
                c.Cliente.Estado == "Activo");
        }

        private async Task<Cliente?> ObtenerClienteActualAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email == null)
            {
                return null;
            }

            var email = user.Email.Trim().ToLower();
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Correo != null && c.Correo.ToLower() == email && c.Estado == "Activo");
        }

        private async Task<EstadoCotizacion> ObtenerEstadoAsync(string nombre)
        {
            return await _context.EstadosCotizacion.SingleAsync(e => e.Nombre == nombre);
        }

        private async Task CargarTiposServicioAsync(int? seleccionado = null)
        {
            ViewBag.TiposServicio = new SelectList(
                await _context.TiposServicio.AsNoTracking().Where(t => t.Estado == "Activo").OrderBy(t => t.Nombre).ToListAsync(),
                "Id",
                "Nombre",
                seleccionado);
        }
    }
}
