using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Cliente,Administrador")]
    public class TecnicosController : BaseController
    {
        private readonly IOrdenServicioService _ordenService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TecnicosController(
            IOrdenServicioService ordenService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _ordenService = ordenService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index(int? estadoOrdenId, string? cliente)
        {
            var query = _context.OrdenesServicio
                .AsNoTracking()
                .AsQueryable();

            if (User.IsInRole("Cliente"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user?.Email == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var email = user.Email.Trim().ToLower();
                var clienteId = await _context.Clientes
                    .Where(c => c.Correo != null && c.Correo.ToLower() == email && c.Estado == "Activo")
                    .Select(c => (int?)c.IdCliente)
                    .FirstOrDefaultAsync();

                if (!clienteId.HasValue)
                {
                    return RedirectToAction("CompletarPerfil", "Cliente");
                }

                query = query.Where(o => o.ClienteId == clienteId.Value);
            }
            else if (!User.IsInRole("Administrador"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToPage("/Account/Login", new { area = "Identity" });
                }

                var empleadoId = await _context.Empleados
                    .Where(e => e.UserId == user.Id)
                    .Select(e => (int?)e.IdEmpleado)
                    .FirstOrDefaultAsync();

                if (!empleadoId.HasValue)
                {
                    TempData["ErrorMessage"] = "Tu cuenta de empleado no está vinculada correctamente. Contacta al administrador.";
                    query = query.Where(_ => false);
                }
                else
                {
                    query = query.Where(o => o.EmpleadoId == empleadoId.Value);
                }
            }

            if (estadoOrdenId.HasValue)
            {
                query = query.Where(o => o.EstadoOrdenId == estadoOrdenId.Value);
            }

            if (!string.IsNullOrWhiteSpace(cliente))
            {
                var termino = cliente.Trim();
                query = query.Where(o =>
                    o.Cliente != null &&
                    (o.Cliente.Nombre.Contains(termino) ||
                     (o.Cliente.Apellidos != null && o.Cliente.Apellidos.Contains(termino)) ||
                     o.Cliente.Identificacion.Contains(termino)));
            }

            var ordenes = await query
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => new OrdenServicioDTO
                {
                    IdOrden = o.IdOrden,
                    CotizacionId = o.CotizacionId,
                    ClienteId = o.ClienteId,
                    NombreCliente = o.Cliente != null
                        ? o.Cliente.Nombre + (o.Cliente.Apellidos != null ? " " + o.Cliente.Apellidos : "")
                        : null,
                    EmpleadoId = o.EmpleadoId,
                    NombreTecnico = o.Empleado != null ? o.Empleado.NombreEmpleado + " " + o.Empleado.ApellidosEmpleado : null,
                    FechaCreacion = o.FechaCreacion,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    EstadoOrdenId = o.EstadoOrdenId,
                    NombreEstado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : null,
                    DescripcionServicio = o.Cotizacion != null ? o.Cotizacion.Descripcion : null
                })
                .ToListAsync();

            ViewBag.EstadosOrden = new SelectList(
                await _context.EstadosOrden.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
                "Id",
                "Nombre",
                estadoOrdenId);
            ViewBag.Cliente = cliente;

            return View(ordenes);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var ordenPermitida = await AplicarAlcanceUsuario(_context.OrdenesServicio.AsNoTracking())
                .AnyAsync(o => o.IdOrden == id);

            if (!ordenPermitida)
            {
                return NotFound();
            }

            var orden = await _ordenService.GetByIdAsync(id);
            if (orden == null)
            {
                return NotFound();
            }
            return View(orden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> IniciarOrden(int id)
        {
            if (!await PuedeGestionarOrdenAsync(id))
            {
                return NotFound();
            }

            var result = await _ordenService.IniciarOrdenAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Orden iniciada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> FinalizarOrden(int id)
        {
            if (!await PuedeGestionarOrdenAsync(id))
            {
                return NotFound();
            }

            var result = await _ordenService.FinalizarOrdenAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Orden finalizada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Empleado,Administrador")]
        public IActionResult Tecnicos()
        {
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Administrar(int id)
        {
            var orden = await _context.OrdenesServicio.AsNoTracking().FirstOrDefaultAsync(o => o.IdOrden == id);
            if (orden == null)
            {
                return NotFound();
            }

            await CargarAdministracionAsync(orden.EmpleadoId, orden.EstadoOrdenId);
            return View(new AdministrarOrdenViewModel
            {
                IdOrden = orden.IdOrden,
                EmpleadoId = orden.EmpleadoId,
                EstadoOrdenId = orden.EstadoOrdenId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Administrar(int id, AdministrarOrdenViewModel model)
        {
            if (id != model.IdOrden)
            {
                return BadRequest();
            }

            if (model.EmpleadoId.HasValue &&
                !await _context.Empleados.AnyAsync(e => e.IdEmpleado == model.EmpleadoId.Value && e.EstadoEmpleado && e.EstadoAcceso == "Aprobado"))
            {
                ModelState.AddModelError(nameof(model.EmpleadoId), "Seleccione un técnico activo con acceso aprobado.");
            }

            var estadoSeleccionado = await _context.EstadosOrden
                .Where(e => e.Id == model.EstadoOrdenId)
                .Select(e => e.Nombre)
                .FirstOrDefaultAsync();
            if (estadoSeleccionado == null)
            {
                ModelState.AddModelError(nameof(model.EstadoOrdenId), "Seleccione un estado válido.");
            }
            else if (!model.EmpleadoId.HasValue && estadoSeleccionado != "Pendiente")
            {
                ModelState.AddModelError(nameof(model.EmpleadoId), "Asigne un técnico antes de cambiar la orden a un estado operativo.");
            }

            if (!ModelState.IsValid)
            {
                await CargarAdministracionAsync(model.EmpleadoId, model.EstadoOrdenId);
                return View(model);
            }

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            orden.EmpleadoId = model.EmpleadoId;
            orden.EstadoOrdenId = model.EstadoOrdenId;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Orden actualizada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        private IQueryable<OrdenServicio> AplicarAlcanceUsuario(IQueryable<OrdenServicio> query)
        {
            if (User.IsInRole("Administrador"))
            {
                return query;
            }

            var email = User.Identity?.Name?.Trim().ToLower();
            if (User.IsInRole("Cliente"))
            {
                return query.Where(o =>
                    o.Cliente != null &&
                    o.Cliente.Correo != null &&
                    o.Cliente.Correo.ToLower() == email &&
                    o.Cliente.Estado == "Activo");
            }

            return query.Where(o => o.Empleado != null && o.Empleado.User != null && o.Empleado.User.Email != null && o.Empleado.User.Email.ToLower() == email);
        }

        private async Task<bool> PuedeGestionarOrdenAsync(int id)
        {
            if (User.IsInRole("Administrador"))
            {
                return await _context.OrdenesServicio.AnyAsync(o => o.IdOrden == id);
            }

            var user = await _userManager.GetUserAsync(User);
            return user != null && await _context.OrdenesServicio
                .AnyAsync(o => o.IdOrden == id && o.Empleado != null && o.Empleado.UserId == user.Id);
        }

        private async Task CargarAdministracionAsync(int? empleadoId, int estadoOrdenId)
        {
            ViewBag.Empleados = new SelectList(
                await _context.Empleados.AsNoTracking()
                    .Where(e => e.EstadoEmpleado && e.EstadoAcceso == "Aprobado")
                    .OrderBy(e => e.NombreEmpleado)
                    .ThenBy(e => e.ApellidosEmpleado)
                    .Select(e => new { e.IdEmpleado, Nombre = e.NombreEmpleado + " " + e.ApellidosEmpleado })
                    .ToListAsync(),
                "IdEmpleado",
                "Nombre",
                empleadoId);

            ViewBag.EstadosOrden = new SelectList(
                await _context.EstadosOrden.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
                "Id",
                "Nombre",
                estadoOrdenId);
        }
    }
}
