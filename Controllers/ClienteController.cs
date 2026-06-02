using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    public class ClienteController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ClienteController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var clientes = await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Direccion)
                    .ThenInclude(d => d!.UbicacionDTA)
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellidos)
                .ToListAsync();

            return View(clientes);
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult IndexCliente()
        {
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear()
        {
            await CargarListasDTAAsync();
            return View(new ClienteFormViewModel { Estado = "Activo" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear(ClienteFormViewModel model)
        {
            var ubicacion = await ValidarUbicacionAsync(model);
            if (!ModelState.IsValid || ubicacion == null)
            {
                await CargarListasDTAAsync(model.ProvinciaId, model.CantonId, model.UbicacionDTAId);
                return View(model);
            }

            var cliente = new Cliente
            {
                Identificacion = model.Identificacion.Trim(),
                Nombre = model.Nombre.Trim(),
                Apellidos = model.Apellidos?.Trim(),
                Correo = model.Correo?.Trim(),
                Telefono = model.Telefono?.Trim(),
                Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Activo" : model.Estado.Trim(),
                Direccion = new Direccion
                {
                    UbicacionDTAId = ubicacion.Id,
                    OtrasSenas = model.OtrasSenas?.Trim()
                }
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cliente creado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id)
        {
            var cliente = await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Direccion)
                    .ThenInclude(d => d!.UbicacionDTA)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            var ubicacion = cliente.Direccion?.UbicacionDTA;
            var model = new ClienteFormViewModel
            {
                IdCliente = cliente.IdCliente,
                Identificacion = cliente.Identificacion,
                Nombre = cliente.Nombre,
                Apellidos = cliente.Apellidos,
                Correo = cliente.Correo,
                Telefono = cliente.Telefono,
                Estado = cliente.Estado,
                ProvinciaId = ubicacion?.IdProvincia,
                CantonId = ubicacion?.IdCanton,
                UbicacionDTAId = ubicacion?.Id,
                OtrasSenas = cliente.Direccion?.OtrasSenas
            };

            await CargarListasDTAAsync(model.ProvinciaId, model.CantonId, model.UbicacionDTAId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id, ClienteFormViewModel model)
        {
            if (id != model.IdCliente)
            {
                return BadRequest();
            }

            var ubicacion = await ValidarUbicacionAsync(model);
            if (!ModelState.IsValid || ubicacion == null)
            {
                await CargarListasDTAAsync(model.ProvinciaId, model.CantonId, model.UbicacionDTAId);
                return View(model);
            }

            var cliente = await _context.Clientes
                .Include(c => c.Direccion)
                .FirstOrDefaultAsync(c => c.IdCliente == id);

            if (cliente == null)
            {
                return NotFound();
            }

            cliente.Identificacion = model.Identificacion.Trim();
            cliente.Nombre = model.Nombre.Trim();
            cliente.Apellidos = model.Apellidos?.Trim();
            cliente.Correo = model.Correo?.Trim();
            cliente.Telefono = model.Telefono?.Trim();
            cliente.Estado = string.IsNullOrWhiteSpace(model.Estado) ? "Activo" : model.Estado.Trim();

            if (cliente.Direccion == null)
            {
                cliente.Direccion = new Direccion();
            }

            cliente.Direccion.UbicacionDTAId = ubicacion.Id;
            cliente.Direccion.OtrasSenas = model.OtrasSenas?.Trim();

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cliente actualizado exitosamente.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerCantones(int provinciaId)
        {
            var cantones = await _context.UbicacionDTA
                .AsNoTracking()
                .Where(u => u.IdProvincia == provinciaId)
                .Select(u => new { id = u.IdCanton, nombre = u.Canton })
                .Distinct()
                .OrderBy(c => c.nombre)
                .ToListAsync();

            return Json(cantones);
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> ObtenerDistritos(int provinciaId, int cantonId)
        {
            var distritos = await _context.UbicacionDTA
                .AsNoTracking()
                .Where(u => u.IdProvincia == provinciaId && u.IdCanton == cantonId)
                .Select(u => new { id = u.Id, nombre = u.Distrito, codigoDTA = u.CodigoDTA })
                .OrderBy(d => d.nombre)
                .ToListAsync();

            return Json(distritos);
        }

        [Authorize(Roles = "Cliente,Administrador")]
        public IActionResult Perfil()
        {
            return View();
        }

        private async Task<UbicacionDTA?> ValidarUbicacionAsync(ClienteFormViewModel model)
        {
            if (!model.ProvinciaId.HasValue || !model.CantonId.HasValue || !model.UbicacionDTAId.HasValue)
            {
                return null;
            }

            var ubicacion = await _context.UbicacionDTA
                .FirstOrDefaultAsync(u =>
                    u.Id == model.UbicacionDTAId.Value &&
                    u.IdProvincia == model.ProvinciaId.Value &&
                    u.IdCanton == model.CantonId.Value);

            if (ubicacion == null)
            {
                ModelState.AddModelError(nameof(model.UbicacionDTAId), "La ubicación DTA seleccionada no es válida.");
            }

            return ubicacion;
        }

        private async Task CargarListasDTAAsync(int? provinciaId = null, int? cantonId = null, int? ubicacionDTAId = null)
        {
            var provincias = await _context.UbicacionDTA
                .AsNoTracking()
                .Select(u => new { u.IdProvincia, u.Provincia })
                .Distinct()
                .OrderBy(p => p.Provincia)
                .ToListAsync();

            var cantones = provinciaId.HasValue
                ? await _context.UbicacionDTA
                    .AsNoTracking()
                    .Where(u => u.IdProvincia == provinciaId.Value)
                    .Select(u => new { u.IdCanton, u.Canton })
                    .Distinct()
                    .OrderBy(c => c.Canton)
                    .ToListAsync()
                : [];

            var distritos = provinciaId.HasValue && cantonId.HasValue
                ? await _context.UbicacionDTA
                    .AsNoTracking()
                    .Where(u => u.IdProvincia == provinciaId.Value && u.IdCanton == cantonId.Value)
                    .OrderBy(u => u.Distrito)
                    .ToListAsync()
                : [];

            ViewBag.Provincias = new SelectList(provincias, "IdProvincia", "Provincia", provinciaId);
            ViewBag.Cantones = new SelectList(cantones, "IdCanton", "Canton", cantonId);
            ViewBag.Distritos = new SelectList(distritos, "Id", "Distrito", ubicacionDTAId);
        }
    }
}
