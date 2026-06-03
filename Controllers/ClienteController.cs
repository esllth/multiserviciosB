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
        public IActionResult Crear()
        {
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
                IdCliente      = cliente.IdCliente,
                Identificacion = cliente.Identificacion,
                Nombre         = cliente.Nombre,
                Apellidos      = cliente.Apellidos,
                Correo         = cliente.Correo,
                Telefono       = cliente.Telefono,
                Estado         = cliente.Estado,
                ProvinciaId    = ubicacion?.IdProvincia,
                CantonId       = ubicacion?.IdCanton,
                // Usamos IdDistrito para que el JS del API externo pueda pre-seleccionarlo
                UbicacionDTAId  = ubicacion?.IdDistrito,
                NombreProvincia = ubicacion?.Provincia,
                NombreCanton    = ubicacion?.Canton,
                NombreDistrito  = ubicacion?.Distrito,
                CodigoDTA       = ubicacion?.CodigoDTA,
                OtrasSenas      = cliente.Direccion?.OtrasSenas
            };

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
                ModelState.AddModelError(nameof(model.UbicacionDTAId), "Seleccione provincia, cantón y distrito.");
                return null;
            }

            if (string.IsNullOrWhiteSpace(model.NombreProvincia) ||
                string.IsNullOrWhiteSpace(model.NombreCanton)    ||
                string.IsNullOrWhiteSpace(model.NombreDistrito))
            {
                ModelState.AddModelError(nameof(model.UbicacionDTAId), "La ubicación seleccionada no es válida.");
                return null;
            }

            int idDistrito = model.UbicacionDTAId.Value;
            string codigoDTA = model.CodigoDTA ?? GenerarCodigoDTA(idDistrito);

            // Buscar registro existente por IdDistrito
            var ubicacion = await _context.UbicacionDTA
                .FirstOrDefaultAsync(u => u.IdDistrito == idDistrito);

            if (ubicacion == null)
            {
                // Crear el registro si no existe (datos del API externo)
                ubicacion = new UbicacionDTA
                {
                    IdProvincia = model.ProvinciaId.Value,
                    Provincia   = model.NombreProvincia.Trim(),
                    IdCanton    = model.CantonId.Value,
                    Canton      = model.NombreCanton.Trim(),
                    IdDistrito  = idDistrito,
                    Distrito    = model.NombreDistrito.Trim(),
                    CodigoDTA   = codigoDTA
                };
                _context.UbicacionDTA.Add(ubicacion);
                await _context.SaveChangesAsync();
            }

            return ubicacion;
        }

        /// <summary>
        /// Deriva el código DTA del idDistrito.
        /// El JS ya convierte los IDs de provincias a INEC antes de enviar,
        /// por lo que idDistrito ya codifica el ID INEC en su primer dígito.
        /// Formato: PCCDD → "P-CC-DD"  (ej: 30101 → "3-01-01" = Cartago)
        /// </summary>
        private static string GenerarCodigoDTA(int idDistrito)
        {
            int p  = idDistrito / 10000;
            int cc = (idDistrito % 10000) / 100;
            int dd = idDistrito % 100;
            return $"{p}-{cc:D2}-{dd:D2}";
        }
    }
}
