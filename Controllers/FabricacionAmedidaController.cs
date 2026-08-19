using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador,Cliente")]
    public class FabricacionAmedidaController : BaseController
    {
        private readonly IProyectoFabricacionService _proyectoService;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public FabricacionAmedidaController(
            IProyectoFabricacionService proyectoService,
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _proyectoService = proyectoService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var proyectos = User.IsInRole("Cliente")
                ? await ObtenerProyectosClienteActualAsync()
                : await _proyectoService.GetAllAsync();

            return View(proyectos);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ChatCotizacion()
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            return View(new ChatFabricacionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ChatCotizacion(ChatFabricacionViewModel model)
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return RedirectToAction("CompletarPerfil", "Cliente");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var tipoFabricacion = await _context.TiposServicio.FirstOrDefaultAsync(t =>
                t.Nombre == "Fabricación a Medida" && t.Estado == "Activo");
            var estadoPendiente = await _context.EstadosCotizacion.FirstOrDefaultAsync(e =>
                e.Nombre == "Pendiente");

            if (tipoFabricacion == null || estadoPendiente == null)
            {
                ModelState.AddModelError(string.Empty,
                    "No fue posible preparar la cotización porque falta la configuración de Fabricación a medida. Contacte al administrador.");
                return View(model);
            }

            var cotizacion = new Cotizacion
            {
                ClienteId = cliente.IdCliente,
                TipoServicioId = tipoFabricacion.Id,
                EstadoCotizacionId = estadoPendiente.Id,
                Descripcion = LimitarTexto(ConstruirDescripcionChat(model), 1000),
                FechaSolicitud = DateTime.UtcNow,
                FechaVisitaSolicitada = model.FechaDeseada,
                AprobadaPorCliente = false
            };

            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cotización #{cotizacion.IdCotizacion} de Fabricación a medida creada correctamente.";
            return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacion.IdCotizacion });
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Crear(ProyectoFabricacionDTO proyectoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(proyectoDto);
            }

            await _proyectoService.CreateAsync(proyectoDto);
            TempData["SuccessMessage"] = "Proyecto de fabricacion creado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar(ProyectoFabricacionDTO proyectoDto)
        {
            if (!ModelState.IsValid)
            {
                return View(proyectoDto);
            }

            var result = await _proyectoService.UpdateAsync(proyectoDto);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Proyecto de fabricacion actualizado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var proyecto = await _proyectoService.GetByIdAsync(id);
            if (proyecto == null)
            {
                return NotFound();
            }
            return View(proyecto);
        }

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var result = await _proyectoService.DeleteAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Proyecto de fabricacion eliminado exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador,Cliente")]
        public IActionResult FabricacionAmedida()
        {
            return RedirectToAction(nameof(Index));
        }

        private async Task<IEnumerable<ProyectoFabricacionDTO>> ObtenerProyectosClienteActualAsync()
        {
            var cliente = await ObtenerClienteActualAsync();
            return cliente == null
                ? Enumerable.Empty<ProyectoFabricacionDTO>()
                : await _proyectoService.GetByClienteAsync(cliente.IdCliente);
        }

        private async Task<Cliente?> ObtenerClienteActualAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user?.Email == null)
            {
                return null;
            }

            var email = user.Email.Trim().ToLowerInvariant();
            return await _context.Clientes.FirstOrDefaultAsync(c =>
                c.Correo != null &&
                c.Correo.ToLower() == email &&
                c.Estado == "Activo");
        }

        private static string ConstruirDescripcionChat(ChatFabricacionViewModel model)
        {
            var partes = new List<string>
            {
                $"Proyecto: {model.NombreProyecto.Trim()}",
                $"Trabajo requerido: {model.TipoTrabajo.Trim()}"
            };

            AgregarSiTieneValor(partes, "Medidas aproximadas", model.Medidas);
            AgregarSiTieneValor(partes, "Material preferido", model.MaterialPreferido);
            AgregarSiTieneValor(partes, "Acabado o color", model.AcabadoColor);
            AgregarSiTieneValor(partes, "Lugar de instalacion", model.UbicacionInstalacion);

            if (model.FechaDeseada.HasValue)
            {
                partes.Add($"Fecha deseada: {model.FechaDeseada.Value:dd/MM/yyyy}");
            }

            if (model.PresupuestoAproximado.HasValue)
            {
                partes.Add($"Presupuesto aproximado indicado por el cliente: CRC {model.PresupuestoAproximado.Value:N2}");
            }

            AgregarSiTieneValor(partes, "Detalles adicionales", model.DetallesAdicionales);
            return string.Join(Environment.NewLine, partes);
        }

        private static void AgregarSiTieneValor(ICollection<string> partes, string etiqueta, string? valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                partes.Add($"{etiqueta}: {valor.Trim()}");
            }
        }

        private static string LimitarTexto(string texto, int maximo)
        {
            return texto.Length <= maximo ? texto : texto.Substring(0, maximo);
        }
    }
}
