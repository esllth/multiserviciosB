using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Cliente,Administrador")]
    public class TecnicosController : BaseController
    {
        private const string TituloTrabajoCompletado = "Tecnico completo el trabajo";
        private const string TituloEncuestaEnviada = "Encuesta de satisfacción enviada";
        private const long MaximoBytesFoto = 5_000_000;
        private readonly IOrdenServicioService _ordenService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IEmailSender _emailSender;

        public TecnicosController(
            IOrdenServicioService ordenService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            IEmailSender emailSender)
        {
            _ordenService = ordenService;
            _userManager = userManager;
            _context = context;
            _environment = environment;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index(int? estadoOrdenId, string? cliente, int? numeroOrden)
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

            if (numeroOrden.HasValue)
            {
                query = query.Where(o => o.IdOrden == numeroOrden.Value);
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
                    FechaCompromiso = o.FechaCompromiso,
                    CompromisoConfirmado = o.CompromisoConfirmado,
                    UsarDireccionPerfil = o.UsarDireccionPerfil,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    EstadoOrdenId = o.EstadoOrdenId,
                    NombreEstado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : null,
                    DescripcionServicio = o.Cotizacion != null ? o.Cotizacion.Descripcion : null,
                    AvisoTrabajoCompletadoEnviado = _context.Notificaciones.Any(n =>
                        n.OrdenId == o.IdOrden &&
                        n.Leida != true &&
                        n.Titulo == TituloTrabajoCompletado),
                    FechaAvisoTrabajoCompletado = _context.Notificaciones
                        .Where(n =>
                            n.OrdenId == o.IdOrden &&
                            n.Leida != true &&
                            n.Titulo == TituloTrabajoCompletado)
                        .OrderByDescending(n => n.Fecha)
                        .Select(n => n.Fecha)
                        .FirstOrDefault()
                })
                .ToListAsync();

            ViewBag.EstadosOrden = new SelectList(
                await _context.EstadosOrden.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
                "Id",
                "Nombre",
                estadoOrdenId);
            ViewBag.Cliente = cliente;
            ViewBag.NumeroOrden = numeroOrden;

            return View(ordenes);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CrearOrden()
        {
            await Task.CompletedTask;
            TempData["ErrorMessage"] = "Las órdenes de servicio solo se generan desde una cotización evaluada y aprobada.";
            return RedirectToAction("Index", "Cotizaciones");
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> BuscarClientes(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino) || termino.Trim().Length < 3)
                return Json(Array.Empty<object>());

            var texto = termino.Trim();
            var minuscula = texto.ToLower();
            var normalizado = texto.Replace("-", "").Replace(" ", "");
            var clientes = await _context.Clientes.AsNoTracking()
                .Where(c => c.Estado == "Activo" &&
                    (c.Identificacion.Replace("-", "").Replace(" ", "").Contains(normalizado) ||
                     (c.Correo != null && c.Correo.ToLower().Contains(minuscula)) ||
                     (c.Telefono != null && c.Telefono.Replace("-", "").Replace(" ", "").Contains(normalizado))))
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellidos)
                .Take(8)
                .Select(c => new
                {
                    id = c.IdCliente,
                    nombre = c.Nombre + (c.Apellidos != null ? " " + c.Apellidos : ""),
                    identificacion = c.Identificacion,
                    correo = c.Correo,
                    telefono = c.Telefono
                })
                .ToListAsync();

            return Json(clientes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CrearOrden(CrearOrdenAdministrativaViewModel model)
        {
            await Task.CompletedTask;
            TempData["ErrorMessage"] = "No se permite crear órdenes directamente. Registre y evalúe una cotización.";
            return RedirectToAction("Index", "Cotizaciones");
#pragma warning disable CS0162
            Cliente? cliente = model.ClienteId.HasValue
                ? await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.IdCliente == model.ClienteId.Value && c.Estado == "Activo")
                : null;
            if (cliente == null && !string.IsNullOrWhiteSpace(model.IdentificadorCliente))
            {
                var termino = model.IdentificadorCliente.Trim();
                var terminoMinuscula = termino.ToLower();
                var terminoNormalizado = termino.Replace("-", "").Replace(" ", "");

                var coincidencias = await _context.Clientes
                    .AsNoTracking()
                    .Where(c => c.Estado == "Activo" &&
                        (c.Identificacion.Replace("-", "").Replace(" ", "") == terminoNormalizado ||
                         (c.Correo != null && c.Correo.ToLower() == terminoMinuscula) ||
                         (c.Telefono != null && c.Telefono.Replace("-", "").Replace(" ", "") == terminoNormalizado)))
                    .Take(2)
                    .ToListAsync();

                if (coincidencias.Count == 1)
                {
                    cliente = coincidencias[0];
                }
                else if (coincidencias.Count > 1)
                {
                    ModelState.AddModelError(nameof(model.IdentificadorCliente), "Hay más de un cliente con ese dato. Utilice el correo electrónico para identificarlo.");
                }
                else
                {
                    ModelState.AddModelError(nameof(model.IdentificadorCliente), "No se encontró un cliente activo con esa cédula, correo o teléfono.");
                }
            }

            if (cliente == null)
            {
                ModelState.AddModelError(nameof(model.IdentificadorCliente), "Busque y seleccione un cliente de la lista de resultados.");
            }

            var tipoServicioValido = model.TipoServicioId.HasValue &&
                await _context.TiposServicio.AnyAsync(t => t.Id == model.TipoServicioId.Value && t.Estado == "Activo");
            if (!tipoServicioValido)
            {
                ModelState.AddModelError(nameof(model.TipoServicioId), "Seleccione un tipo de servicio activo.");
            }

            if (!ModelState.IsValid || cliente == null)
            {
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            var estadoCotizacion = await _context.EstadosCotizacion.SingleOrDefaultAsync(e => e.Nombre == "Aprobada");
            var estadoOrden = await _context.EstadosOrden.SingleOrDefaultAsync(e => e.Nombre == "Pendiente");
            if (estadoCotizacion == null || estadoOrden == null)
            {
                ModelState.AddModelError("", "No están configurados los estados Aprobada y Pendiente requeridos para crear la orden.");
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            var cotizacion = new Cotizacion
            {
                ClienteId = cliente.IdCliente,
                TipoServicioId = model.TipoServicioId!.Value,
                EstadoCotizacionId = estadoCotizacion.Id,
                Descripcion = model.Descripcion.Trim(),
                FechaSolicitud = DateTime.UtcNow,
                AprobadaPorCliente = false
            };

            var orden = new OrdenServicio
            {
                Cotizacion = cotizacion,
                ClienteId = cliente.IdCliente,
                EstadoOrdenId = estadoOrden.Id,
                FechaCreacion = DateTime.UtcNow,
                RequiereFotosObligatorias = model.RequiereFotosObligatorias
            };

            _context.OrdenesServicio.Add(orden);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Orden #{orden.IdOrden} creada para {cliente.Nombre} {cliente.Apellidos}.";
            return RedirectToAction(nameof(Administrar), new { id = orden.IdOrden });
#pragma warning restore CS0162
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

            orden.AvisoTrabajoCompletadoEnviado = await _context.Notificaciones.AnyAsync(n =>
                n.OrdenId == id &&
                n.Leida != true &&
                n.Titulo == TituloTrabajoCompletado);
            orden.FechaAvisoTrabajoCompletado = await _context.Notificaciones
                .Where(n =>
                    n.OrdenId == id &&
                    n.Leida != true &&
                    n.Titulo == TituloTrabajoCompletado)
                .OrderByDescending(n => n.Fecha)
                .Select(n => n.Fecha)
                .FirstOrDefaultAsync();

            ViewBag.Documentos = await _context.DocumentosOrdenServicio
                .AsNoTracking()
                .Where(d => d.OrdenId == id)
                .OrderByDescending(d => d.FechaCarga)
                .ToListAsync();

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
        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> ReportarTrabajoCompletado(int id)
        {
            if (!await PuedeGestionarOrdenAsync(id))
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var orden = await _context.OrdenesServicio
                .Include(o => o.EstadoOrden)
                .Include(o => o.Empleado)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (user == null || orden == null || orden.Empleado?.UserId != user.Id)
            {
                return NotFound();
            }

            var estado = orden.EstadoOrden?.Nombre;
            if (estado != "En Progreso" && estado != "EnProgreso")
            {
                TempData["ErrorMessage"] = "Solo se puede avisar trabajo completado cuando la orden esta en progreso.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            if (orden.RequiereFotosObligatorias)
            {
                var tieneFotoInicial = await _context.FotosOrdenServicio.AnyAsync(f => f.OrdenId == id && f.TipoFoto == "Inicial");
                var tieneFotoFinal  = await _context.FotosOrdenServicio.AnyAsync(f => f.OrdenId == id && f.TipoFoto == "Final");
                if (!tieneFotoInicial || !tieneFotoFinal)
                {
                    TempData["ErrorMessage"] = "Esta orden requiere evidencia fotográfica inicial y final antes de poder reportar el trabajo como completado.";
                    return RedirectToAction(nameof(Detalle), new { id });
                }
            }

            var avisoPendiente = await _context.Notificaciones.AnyAsync(n =>
                n.OrdenId == id &&
                n.Leida != true &&
                n.Titulo == TituloTrabajoCompletado);

            if (!avisoPendiente)
            {
                var nombreTecnico = orden.Empleado != null
                    ? $"{orden.Empleado.NombreEmpleado} {orden.Empleado.ApellidosEmpleado}"
                    : "El tecnico asignado";

                _context.Notificaciones.Add(new Notificacion
                {
                    OrdenId = orden.IdOrden,
                    Titulo = TituloTrabajoCompletado,
                    Mensaje = $"{nombreTecnico} reportó que la orden #{orden.IdOrden} está lista para cierre administrativo.",
                    Fecha = DateTime.Now,
                    Leida = false
                });

                await _context.SaveChangesAsync();

                try
                {
                    await EnviarCorreoTrabajoCompletadoAsync(
                        "admin@multiserviciosb.com",
                        orden.IdOrden,
                        nombreTecnico,
                        "El técnico reportó que el trabajo está listo para revisión y cierre administrativo.");
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "El aviso interno fue registrado, pero no se pudo enviar el correo. Verifique la configuración SMTP.";
                    return RedirectToAction(nameof(Detalle), new { id });
                }
            }

            TempData["SuccessMessage"] = "Aviso interno y correo enviados al administrador.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Empleado,Administrador")]
        [RequestSizeLimit(30_000_000)]
        public async Task<IActionResult> AdjuntarEvidencia(
            int id,
            IFormFile? archivoInicial,
            IFormFile? archivoFinal,
            string? descripcionInicial,
            string? descripcionFinal)
        {
            if (!await PuedeGestionarOrdenAsync(id))
            {
                return NotFound();
            }

            if (archivoInicial == null || archivoInicial.Length == 0 ||
                archivoFinal == null || archivoFinal.Length == 0)
            {
                TempData["ErrorMessage"] = "Seleccione una foto de antes y una foto de después.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            var evidencias = new[]
            {
                (Archivo: archivoInicial, Tipo: "Inicial", Descripcion: descripcionInicial),
                (Archivo: archivoFinal, Tipo: "Final", Descripcion: descripcionFinal)
            };

            foreach (var evidencia in evidencias)
            {
                var error = await ValidarFotoAsync(evidencia.Archivo);
                if (error != null)
                {
                    TempData["ErrorMessage"] = error;
                    return RedirectToAction(nameof(Detalle), new { id });
                }
            }

            await AsegurarTablaFotoOrdenAsync();
            Directory.CreateDirectory(ObtenerCarpetaEvidencias(id));
            var rutasGuardadas = new List<string>();

            try
            {
                foreach (var evidencia in evidencias)
                {
                    var archivo = evidencia.Archivo;
                    var extension = ObtenerExtensionPermitida(archivo.ContentType);
                    var nombreArchivo = $"{Guid.NewGuid():N}{extension}";
                    var rutaFisica = Path.Combine(ObtenerCarpetaEvidencias(id), nombreArchivo);

                    await using (var destino = System.IO.File.Create(rutaFisica))
                    {
                        await archivo.CopyToAsync(destino);
                    }
                    rutasGuardadas.Add(rutaFisica);

                    _context.FotosOrdenServicio.Add(new FotoOrdenServicio
                    {
                        OrdenId = id,
                        Ruta = $"/images/OrdenesServicio/{id}/{nombreArchivo}",
                        NombreOriginal = LimitarTexto(Path.GetFileName(archivo.FileName), 150),
                        TipoContenido = LimitarTexto(archivo.ContentType, 50),
                        TipoFoto = evidencia.Tipo,
                        FechaCarga = DateTime.Now,
                        Descripcion = string.IsNullOrWhiteSpace(evidencia.Descripcion)
                            ? null
                            : LimitarTexto(evidencia.Descripcion.Trim(), 500)
                    });
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                LimpiarArchivosGuardados(rutasGuardadas);
                TempData["ErrorMessage"] = $"No se pudo guardar la evidencia en la base de datos. Detalle: {ex.InnerException?.Message ?? ex.Message}";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            TempData["SuccessMessage"] = "Las fotos de antes y después se adjuntaron correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [Authorize(Roles = "Empleado,Cliente,Administrador")]
        public async Task<IActionResult> Evidencia(int id)
        {
            var foto = await _context.FotosOrdenServicio
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.IdFotoOrden == id);

            if (foto == null)
            {
                return NotFound();
            }

            var ordenPermitida = await AplicarAlcanceUsuario(_context.OrdenesServicio.AsNoTracking())
                .AnyAsync(o => o.IdOrden == foto.OrdenId);

            if (!ordenPermitida)
            {
                return NotFound();
            }

            var rutaRelativa = foto.Ruta.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var rutaFisica = Path.Combine(_environment.WebRootPath, rutaRelativa);

            return System.IO.File.Exists(rutaFisica)
                ? PhysicalFile(rutaFisica, foto.TipoContenido)
                : NotFound();
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

            var errorStock = await _ordenService.ObtenerErrorStockMaterialesAsync(id);
            if (errorStock != null)
            {
                TempData["ErrorMessage"] = errorStock;
                return RedirectToAction(nameof(Detalle), new { id });
            }

            var result = await _ordenService.FinalizarOrdenAsync(id, "Orden finalizada por administrador");
            if (!result)
            {
                TempData["ErrorMessage"] = "No se pudo finalizar la orden. Verifique que cumple con todos los requisitos.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            await MarcarAvisosTrabajoCompletadoAsync(id);

            try
            {
                var enviado = await EnviarEncuestaSiCorrespondeAsync(id);
                TempData["SuccessMessage"] = enviado
                    ? "Orden finalizada y encuesta enviada al correo del cliente."
                    : "Orden finalizada. No se envió una encuesta nueva porque no hay correo registrado o ya había sido enviada.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "La orden fue finalizada, pero la encuesta no pudo enviarse. Verifique la configuración SMTP.";
                return RedirectToAction(nameof(Detalle), new { id });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> CerrarOrdenDesdeAviso(int id, bool volverDashboard = false)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.EstadoOrden)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null)
            {
                return NotFound();
            }

            var tieneAvisoPendiente = await _context.Notificaciones.AnyAsync(n =>
                n.OrdenId == id &&
                n.Leida != true &&
                n.Titulo == TituloTrabajoCompletado);

            if (!tieneAvisoPendiente)
            {
                TempData["ErrorMessage"] = "La orden no tiene un aviso pendiente de trabajo completado.";
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            var estadoActual = orden.EstadoOrden?.Nombre;
            if (estadoActual != "En Progreso" && estadoActual != "EnProgreso")
            {
                TempData["ErrorMessage"] = "Solo se pueden cerrar desde aviso las ordenes en progreso.";
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            if (!await _ordenService.ValidarPuedeFinalizarAsync(id))
            {
                TempData["ErrorMessage"] = "No se puede cerrar la orden: requiere evidencia fotografica inicial y final.";
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            var errorStock = await _ordenService.ObtenerErrorStockMaterialesAsync(id);
            if (errorStock != null)
            {
                TempData["ErrorMessage"] = errorStock;
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            var finalizada = await _ordenService.FinalizarOrdenAsync(id, "Orden cerrada por administrador desde aviso de trabajo completado.");
            if (!finalizada)
            {
                TempData["ErrorMessage"] = "No se pudo cerrar la orden. Verifique sus evidencias y materiales.";
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            await MarcarAvisosTrabajoCompletadoAsync(id, guardarCambios: false);
            await _context.SaveChangesAsync();

            try
            {
                var encuestaEnviada = await EnviarEncuestaSiCorrespondeAsync(id);
                TempData["SuccessMessage"] = encuestaEnviada
                    ? "Orden cerrada y encuesta de satisfacción enviada al cliente."
                    : "Orden cerrada. No se envió una encuesta nueva porque no hay correo registrado o ya había sido enviada.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "La orden fue cerrada, pero la encuesta no pudo enviarse. Verifique la configuración SMTP.";
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            return volverDashboard
                ? RedirectToAction("Dashboard", "Home")
                : RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> RechazarOrden(int id, string? motivoRechazo)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.EstadoOrden)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null) return NotFound();

            var estadoActual = orden.EstadoOrden?.Nombre;
            if (estadoActual != "En Progreso" && estadoActual != "EnProgreso" && estadoActual != "Pendiente")
            {
                TempData["ErrorMessage"] = "Solo se pueden rechazar órdenes en estado Pendiente o En Progreso.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            var estadoCancelada = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Cancelada");
            if (estadoCancelada == null)
            {
                TempData["ErrorMessage"] = "No se encontró el estado Cancelada.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            orden.EstadoOrdenId = estadoCancelada.Id;
            orden.FechaFin = DateTime.Now;
            orden.ComentariosFinales = string.IsNullOrWhiteSpace(motivoRechazo)
                ? "Orden rechazada por el administrador."
                : $"Rechazada: {motivoRechazo.Trim()}";

            await MarcarAvisosTrabajoCompletadoAsync(id, guardarCambios: false);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Orden rechazada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
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

            await CargarAdministracionAsync(id, orden.EmpleadoId, orden.EstadoOrdenId);
            return View(new AdministrarOrdenViewModel
            {
                IdOrden = orden.IdOrden,
                EmpleadoId = orden.EmpleadoId,
                EstadoOrdenId = orden.EstadoOrdenId,
                FechaCompromiso = orden.FechaCompromiso,
                FechaCalendario = orden.FechaCompromiso?.Date,
                HoraCalendario = orden.FechaCompromiso?.TimeOfDay,
                CompromisoConfirmado = orden.CompromisoConfirmado
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

            if (model.FechaCalendario.HasValue && model.HoraCalendario.HasValue)
            {
                model.FechaCompromiso = model.FechaCalendario.Value.Date.Add(model.HoraCalendario.Value);
            }
            else if (model.FechaCalendario.HasValue || model.HoraCalendario.HasValue)
            {
                ModelState.AddModelError(nameof(model.FechaCalendario), "Seleccione tanto la fecha como la hora del compromiso.");
                model.FechaCompromiso = null;
            }
            else
            {
                model.FechaCompromiso = null;
            }

            if (model.EmpleadoId.HasValue &&
                !await _context.Empleados.AnyAsync(e =>
                    e.IdEmpleado == model.EmpleadoId.Value &&
                    e.EstadoEmpleado &&
                    (e.EstadoAcceso == EstadosEmpleado.Activo || e.EstadoAcceso == "Aprobado")))
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

            if (model.CompromisoConfirmado)
            {
                if (!model.EmpleadoId.HasValue)
                {
                    ModelState.AddModelError(nameof(model.EmpleadoId), "Asigne un técnico antes de confirmar el compromiso.");
                }

                if (!model.FechaCompromiso.HasValue)
                {
                    ModelState.AddModelError(nameof(model.FechaCompromiso), "Seleccione la fecha y hora antes de confirmar el compromiso.");
                }
            }

            if (model.FechaCompromiso.HasValue && model.FechaCompromiso.Value.TimeOfDay == TimeSpan.FromHours(12))
            {
                ModelState.AddModelError(nameof(model.FechaCompromiso), "El medio dia no esta disponible por horario de almuerzo. Seleccione otra hora.");
            }

            if (!ModelState.IsValid)
            {
                await CargarAdministracionAsync(id, model.EmpleadoId, model.EstadoOrdenId);
                return View(model);
            }

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null)
            {
                return NotFound();
            }

            orden.EmpleadoId = model.EmpleadoId;
            orden.EstadoOrdenId = model.EstadoOrdenId;
            orden.FechaCompromiso = model.FechaCompromiso;
            orden.CompromisoConfirmado = model.CompromisoConfirmado;
            await _context.SaveChangesAsync();

            var quedoCompletada = await _context.EstadosOrden
                .AnyAsync(e => e.Id == model.EstadoOrdenId && e.Nombre == "Completada");
            if (quedoCompletada)
            {
                try
                {
                    var encuestaEnviada = await EnviarEncuestaSiCorrespondeAsync(id);
                    TempData["SuccessMessage"] = encuestaEnviada
                        ? "Orden actualizada y encuesta enviada al cliente."
                        : "Orden actualizada. La encuesta ya había sido enviada o el cliente no tiene correo.";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "La orden quedó completada, pero la encuesta no pudo enviarse. Verifique la configuración SMTP.";
                }
            }
            else
            {
                TempData["SuccessMessage"] = "Orden actualizada.";
            }
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> AsignarMaterial(int id, int materialId, int cantidad)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.EstadoOrden)
                .FirstOrDefaultAsync(o => o.IdOrden == id);
            var material = await _context.Materiales.FirstOrDefaultAsync(m => m.IdMaterial == materialId && m.Estado == "Activo");
            if (orden == null || material == null) return NotFound();

            if (orden.EstadoOrden?.Nombre != "En Progreso")
            {
                TempData["ErrorMessage"] = "Los materiales solo pueden asignarse mientras la orden está En Progreso.";
                return RedirectToAction(nameof(Administrar), new { id });
            }

            if (!orden.EmpleadoId.HasValue)
            {
                TempData["ErrorMessage"] = "Primero asigne un técnico a la orden y guarde los cambios.";
                return RedirectToAction(nameof(Administrar), new { id });
            }

            if (cantidad <= 0)
            {
                TempData["ErrorMessage"] = "La cantidad debe ser mayor que cero.";
                return RedirectToAction(nameof(Administrar), new { id });
            }

            var consumo = await _context.ConsumosMaterial
                .FirstOrDefaultAsync(c => c.OrdenId == id && c.MaterialId == materialId);
            var reservadaEnOtrasOrdenes = await _context.ConsumosMaterial
                .Where(c => c.MaterialId == materialId && c.OrdenId != id &&
                    c.Orden != null && c.Orden.EstadoOrden != null &&
                    c.Orden.EstadoOrden.Nombre != "Completada" && c.Orden.EstadoOrden.Nombre != "Cancelada")
                .SumAsync(c => c.CantidadUsada ?? 0);

            if ((material.StockActual ?? 0) < reservadaEnOtrasOrdenes + cantidad)
            {
                TempData["ErrorMessage"] = $"Stock no disponible. Hay {material.StockActual ?? 0} unidades y {reservadaEnOtrasOrdenes:N0} están asignadas a otras órdenes.";
                return RedirectToAction(nameof(Administrar), new { id });
            }

            if (consumo == null)
            {
                _context.ConsumosMaterial.Add(new ConsumoMaterial
                {
                    OrdenId = id,
                    MaterialId = materialId,
                    CantidadUsada = cantidad,
                    FechaRegistro = DateTime.Now
                });
            }
            else
            {
                consumo.CantidadUsada = cantidad;
                consumo.FechaRegistro = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Material asignado a la orden.";
            return RedirectToAction(nameof(Administrar), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> QuitarMaterial(int id, int consumoId)
        {
            var consumo = await _context.ConsumosMaterial
                .Include(c => c.Orden).ThenInclude(o => o!.EstadoOrden)
                .FirstOrDefaultAsync(c => c.IdConsumo == consumoId && c.OrdenId == id);
            if (consumo == null) return NotFound();

            if (consumo.Orden?.EstadoOrden?.Nombre != "En Progreso")
            {
                TempData["ErrorMessage"] = "Los materiales solo pueden modificarse mientras la orden está En Progreso.";
                return RedirectToAction(nameof(Administrar), new { id });
            }

            _context.ConsumosMaterial.Remove(consumo);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Material retirado de la orden.";
            return RedirectToAction(nameof(Administrar), new { id });
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

        private async Task CargarAdministracionAsync(int ordenId, int? empleadoId, int estadoOrdenId)
        {
            ViewBag.Empleados = new SelectList(
                await _context.Empleados.AsNoTracking()
                    .Where(e => e.EstadoEmpleado && (e.EstadoAcceso == EstadosEmpleado.Activo || e.EstadoAcceso == "Aprobado"))
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

            ViewBag.Materiales = new SelectList(
                await _context.Materiales.AsNoTracking()
                    .Where(m => m.Estado == "Activo")
                    .OrderBy(m => m.Categoria).ThenBy(m => m.Nombre)
                    .Select(m => new { m.IdMaterial, Nombre = (m.Categoria ?? "Sin categoría") + " · " + m.Nombre + " (stock: " + (m.StockActual ?? 0) + ")" })
                    .ToListAsync(),
                "IdMaterial",
                "Nombre");

            var catalogoMateriales = await _context.Materiales
                .AsNoTracking()
                .Where(m => m.Estado == "Activo")
                .OrderBy(m => m.Categoria)
                .ThenBy(m => m.Nombre)
                .ToListAsync();

            var reservadoEnOtrasOrdenes = await _context.ConsumosMaterial
                .AsNoTracking()
                .Where(c => c.OrdenId != ordenId && c.Orden != null && c.Orden.EstadoOrden != null &&
                    c.Orden.EstadoOrden.Nombre != "Completada" && c.Orden.EstadoOrden.Nombre != "Cancelada")
                .GroupBy(c => c.MaterialId)
                .Select(g => new { MaterialId = g.Key, Cantidad = g.Sum(c => c.CantidadUsada ?? 0) })
                .ToDictionaryAsync(x => x.MaterialId, x => x.Cantidad);

            foreach (var material in catalogoMateriales)
            {
                reservadoEnOtrasOrdenes.TryGetValue(material.IdMaterial, out var reservado);
                material.StockActual = Math.Max(0, (material.StockActual ?? 0) - (int)reservado);
            }
            ViewBag.CatalogoMateriales = catalogoMateriales.Where(m => m.StockActual > 0).ToList();

            ViewBag.PuedeAsignarMateriales = await _context.EstadosOrden
                .AnyAsync(e => e.Id == estadoOrdenId && e.Nombre == "En Progreso");

            ViewBag.MaterialesAsignados = await _context.ConsumosMaterial
                .AsNoTracking()
                .Include(c => c.Material)
                .Where(c => c.OrdenId == ordenId)
                .ToListAsync();
        }

        private async Task EnviarCorreoTrabajoCompletadoAsync(
            string destinatario,
            int ordenId,
            string tecnico,
            string mensaje)
        {
            await _emailSender.SendEmailAsync(
                destinatario,
                $"Trabajo completado - Orden #{ordenId}",
                $"""
                <div style="font-family:Arial,sans-serif;max-width:620px;margin:auto;color:#1f2937">
                    <h2 style="color:#166534">Trabajo completado</h2>
                    <p>La orden de servicio <strong>#{ordenId}</strong> fue reportada como completada.</p>
                    <p><strong>Técnico:</strong> {WebUtility.HtmlEncode(tecnico)}</p>
                    <p>{WebUtility.HtmlEncode(mensaje)}</p>
                    <hr style="border:0;border-top:1px solid #e5e7eb;margin:24px 0">
                    <p style="color:#64748b;font-size:13px">Multiservicios Bolívar</p>
                </div>
                """);
        }

        private async Task<bool> EnviarEncuestaSiCorrespondeAsync(int ordenId)
        {
            if (await _context.Notificaciones.AnyAsync(n =>
                n.OrdenId == ordenId && n.Titulo == TituloEncuestaEnviada))
            {
                return false;
            }

            var orden = await _context.OrdenesServicio
                .AsNoTracking()
                .Include(o => o.Cliente)
                .Include(o => o.Empleado)
                .FirstOrDefaultAsync(o => o.IdOrden == ordenId);
            var destinatario = orden?.Cliente?.Correo;
            if (orden == null || string.IsNullOrWhiteSpace(destinatario)) return false;

            var tecnico = orden.Empleado == null
                ? "Nuestro equipo técnico"
                : $"{orden.Empleado.NombreEmpleado} {orden.Empleado.ApellidosEmpleado}";
            var tecnicoSeguro = WebUtility.HtmlEncode(tecnico);
            var enlacesEstrellas = Enumerable.Range(1, 5)
                .Select(calificacion => Url.Action(
                    "Responder",
                    "Encuestas",
                    new { ordenId, calificacionServicio = calificacion },
                    Request.Scheme))
                .ToArray();
            var estrellasHtml = string.Join("", enlacesEstrellas.Select((enlace, indice) =>
                $"<a href=\"{WebUtility.HtmlEncode(enlace)}\" title=\"{indice + 1} estrellas\" style=\"color:#f59e0b;font-size:34px;text-decoration:none;margin-right:5px\">&#9733;</a>"));
            await _emailSender.SendEmailAsync(
                destinatario,
                $"Cuéntenos sobre su servicio - Orden #{ordenId}",
                $"""
                <div style="font-family:Arial,sans-serif;max-width:620px;margin:auto;color:#1f2937">
                    <h2 style="color:#166534">Trabajo completado</h2>
                    <p>La orden de servicio <strong>#{ordenId}</strong> fue completada.</p>
                    <p><strong>Técnico:</strong> {tecnicoSeguro}</p>
                    <p>¿Cómo califica el servicio? Seleccione una estrella:</p>
                    <div style="margin:20px 0">{estrellasHtml}</div>
                    <p style="color:#64748b;font-size:13px">1 estrella = malo &nbsp;·&nbsp; 5 estrellas = excelente</p>
                    <p>Al seleccionar una estrella se abrirá la encuesta con su calificación marcada. Revise la atención del técnico y pulse <strong>Enviar encuesta</strong>.</p>
                    <hr style="border:0;border-top:1px solid #e5e7eb;margin:24px 0">
                    <p style="color:#64748b;font-size:13px">Multiservicios Bolívar</p>
                </div>
                """);

            _context.Notificaciones.Add(new Notificacion
            {
                OrdenId = ordenId,
                ClienteId = orden.ClienteId,
                Titulo = TituloEncuestaEnviada,
                Mensaje = "Encuesta de satisfacción enviada al correo del cliente.",
                Fecha = DateTime.Now,
                Leida = true
            });
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task CargarTiposServicioAsync(int? tipoServicioId = null)
        {
            ViewBag.TiposServicio = new SelectList(
                await _context.TiposServicio
                    .AsNoTracking()
                    .Where(t => t.Estado == "Activo")
                    .OrderBy(t => t.Nombre)
                    .ToListAsync(),
                "Id",
                "Nombre",
                tipoServicioId);
        }

        private async Task MarcarAvisosTrabajoCompletadoAsync(int ordenId, bool guardarCambios = true)
        {
            var avisos = await _context.Notificaciones
                .Where(n => n.OrdenId == ordenId && n.Leida != true && n.Titulo == TituloTrabajoCompletado)
                .ToListAsync();

            if (avisos.Count == 0)
            {
                return;
            }

            foreach (var aviso in avisos)
            {
                aviso.Leida = true;
            }

            if (guardarCambios)
            {
                await _context.SaveChangesAsync();
            }
        }

        private string ObtenerCarpetaEvidencias(int ordenId)
        {
            return Path.Combine(_environment.WebRootPath, "images", "OrdenesServicio", ordenId.ToString());
        }

        private async Task AsegurarTablaFotoOrdenAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'dbo.FotoOrden', N'U') IS NULL
                BEGIN
                    IF OBJECT_ID(N'dbo.FotosOrdenServicio', N'U') IS NOT NULL
                    BEGIN
                        EXEC sp_rename N'dbo.FotosOrdenServicio', N'FotoOrden';
                    END
                    ELSE
                    BEGIN
                        CREATE TABLE [dbo].[FotoOrden] (
                            [IdFotoOrden]    INT            IDENTITY (1, 1) NOT NULL,
                            [OrdenId]        INT            NOT NULL,
                            [Ruta]           NVARCHAR (260) NOT NULL,
                            [NombreOriginal] NVARCHAR (150) NOT NULL,
                            [TipoContenido]  NVARCHAR (50)  NOT NULL,
                            [TipoFoto]       NVARCHAR (20)  NOT NULL,
                            [FechaCarga]     DATETIME       CONSTRAINT [DF_FotoOrden_FechaCarga] DEFAULT (GETDATE()) NOT NULL,
                            [Descripcion]    NVARCHAR (500) NULL,
                            CONSTRAINT [PK_FotoOrden] PRIMARY KEY CLUSTERED ([IdFotoOrden] ASC),
                            CONSTRAINT [CK_FotoOrden_TipoFoto] CHECK ([TipoFoto] = N'Final' OR [TipoFoto] = N'Inicial')
                        );
                    END
                END

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_FotoOrden_OrdenId'
                      AND object_id = OBJECT_ID(N'dbo.FotoOrden')
                )
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_FotoOrden_OrdenId]
                        ON [dbo].[FotoOrden]([OrdenId] ASC);
                END
                """);
        }

        private static async Task<string?> ValidarFotoAsync(IFormFile archivo)
        {
            if (archivo.Length > MaximoBytesFoto)
            {
                return "Cada fotografia debe pesar 5 MB o menos.";
            }

            if (ObtenerExtensionPermitida(archivo.ContentType) == null)
            {
                return "Solo se permiten fotografias JPEG, PNG o WebP.";
            }

            if (!await EsImagenPermitidaAsync(archivo))
            {
                return "Uno de los archivos seleccionados no parece ser una imagen valida.";
            }

            return null;
        }

        private static string? ObtenerExtensionPermitida(string tipoContenido)
        {
            return tipoContenido.ToLowerInvariant() switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => null
            };
        }

        private static async Task<bool> EsImagenPermitidaAsync(IFormFile foto)
        {
            var encabezado = new byte[12];
            await using var stream = foto.OpenReadStream();
            var leidos = await stream.ReadAsync(encabezado.AsMemory());

            return foto.ContentType.ToLowerInvariant() switch
            {
                "image/jpeg" => leidos >= 3 && encabezado[0] == 0xFF && encabezado[1] == 0xD8 && encabezado[2] == 0xFF,
                "image/png" => leidos >= 8 && encabezado[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                "image/webp" => leidos >= 12
                    && encabezado[..4].SequenceEqual("RIFF"u8)
                    && encabezado[8..12].SequenceEqual("WEBP"u8),
                _ => false
            };
        }

        private static string LimitarTexto(string texto, int maximo)
        {
            return texto.Length <= maximo ? texto : texto.Substring(0, maximo);
        }

        private static void LimpiarArchivosGuardados(IEnumerable<string> rutas)
        {
            foreach (var ruta in rutas)
            {
                if (System.IO.File.Exists(ruta))
                {
                    System.IO.File.Delete(ruta);
                }
            }
        }
    }
}
