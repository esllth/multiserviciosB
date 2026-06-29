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
using System.Text;
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

            var proyectoDto = new ProyectoFabricacionDTO
            {
                ClienteId = cliente.IdCliente,
                NombreProyecto = model.NombreProyecto.Trim(),
                Descripcion = LimitarTexto(ConstruirDescripcionChat(model), 1000),
                Estado = "Pendiente",
                FechaInicioEstimada = model.FechaDeseada,
                CostoEstimado = model.PresupuestoAproximado,
                ObservacionesCliente = LimitarTexto(ConstruirObservacionesChat(model), 1000)
            };

            await _proyectoService.CreateAsync(proyectoDto);
            TempData["SuccessMessage"] = "Solicitud de fabricacion enviada exitosamente. El administrador revisara la informacion inicial.";
            return RedirectToAction(nameof(Index));
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
                "Solicitud creada desde el chat de cotizacion inteligente.",
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

        private static string ConstruirObservacionesChat(ChatFabricacionViewModel model)
        {
            var resumen = new StringBuilder();
            resumen.AppendLine("Informacion inicial recopilada por asistente:");
            resumen.AppendLine($"- Proyecto: {model.NombreProyecto.Trim()}");
            resumen.AppendLine($"- Trabajo: {model.TipoTrabajo.Trim()}");
            AgregarLineaSiTieneValor(resumen, "Medidas", model.Medidas);
            AgregarLineaSiTieneValor(resumen, "Material", model.MaterialPreferido);
            AgregarLineaSiTieneValor(resumen, "Acabado/color", model.AcabadoColor);
            AgregarLineaSiTieneValor(resumen, "Ubicacion", model.UbicacionInstalacion);

            if (model.FechaDeseada.HasValue)
            {
                resumen.AppendLine($"- Fecha deseada: {model.FechaDeseada.Value:dd/MM/yyyy}");
            }

            if (model.PresupuestoAproximado.HasValue)
            {
                resumen.AppendLine($"- Presupuesto aproximado: CRC {model.PresupuestoAproximado.Value:N2}");
            }

            AgregarLineaSiTieneValor(resumen, "Detalles", model.DetallesAdicionales);
            return resumen.ToString();
        }

        private static void AgregarSiTieneValor(ICollection<string> partes, string etiqueta, string? valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                partes.Add($"{etiqueta}: {valor.Trim()}");
            }
        }

        private static void AgregarLineaSiTieneValor(StringBuilder resumen, string etiqueta, string? valor)
        {
            if (!string.IsNullOrWhiteSpace(valor))
            {
                resumen.AppendLine($"- {etiqueta}: {valor.Trim()}");
            }
        }

        private static string LimitarTexto(string texto, int maximo)
        {
            return texto.Length <= maximo ? texto : texto.Substring(0, maximo);
        }
    }
}
