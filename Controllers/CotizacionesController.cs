using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.Services;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Cliente,Administrador,Secretaria")]
    public class CotizacionesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public CotizacionesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        public async Task<IActionResult> Index(int? estadoCotizacionId, string? cliente)
        {
            var query = _context.Cotizaciones.AsNoTracking().AsQueryable();

            if (!EsPersonalAdministrativo())
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
                    RequiereAdelanto = c.RequiereAdelanto,
                    PorcentajeAdelanto = c.PorcentajeAdelanto,
                    FechaSolicitud = c.FechaSolicitud,
                    FechaVisitaSolicitada = c.FechaVisitaSolicitada,
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

        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<IActionResult> Registrar()
        {
            await CargarTiposServicioAsync();
            return View(new RegistrarCotizacionAdministrativaViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Secretaria")]
        public async Task<IActionResult> Registrar(RegistrarCotizacionAdministrativaViewModel model)
        {
            var cliente = await BuscarClienteActivoAsync(model.IdentificadorCliente);
            if (cliente == null && !string.IsNullOrWhiteSpace(model.IdentificadorCliente))
            {
                ModelState.AddModelError(nameof(model.IdentificadorCliente), "No se encontró un cliente activo con esa cédula, correo o teléfono.");
            }

            var tipoValido = model.TipoServicioId.HasValue &&
                await _context.TiposServicio.AnyAsync(t => t.Id == model.TipoServicioId.Value && t.Estado == "Activo");
            if (!tipoValido)
            {
                ModelState.AddModelError(nameof(model.TipoServicioId), "Seleccione un tipo de servicio activo.");
            }

            if (!ModelState.IsValid || cliente == null)
            {
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            var estadoPendiente = await ObtenerEstadoAsync("Pendiente");
            var cotizacion = new Cotizacion
            {
                ClienteId = cliente.IdCliente,
                TipoServicioId = model.TipoServicioId!.Value,
                EstadoCotizacionId = estadoPendiente.Id,
                Descripcion = model.Descripcion.Trim(),
                FechaSolicitud = DateTime.UtcNow,
                FechaVisitaSolicitada = model.FechaVisitaSolicitada,
                AprobadaPorCliente = false
            };

            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cotización #{cotizacion.IdCotizacion} registrada para {cliente.Nombre} {cliente.Apellidos}.";
            return RedirectToAction(nameof(Detalle), new { id = cotizacion.IdCotizacion });
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var cotizacion = await ConsultaPermitidaAsync()
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.TipoServicio)
                .Include(c => c.EstadoCotizacion)
                .Include(c => c.Fotos)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion != null)
            {
                ViewBag.OrdenServicio = await _context.OrdenesServicio
                    .AsNoTracking()
                    .Include(o => o.Empleado)
                    .FirstOrDefaultAsync(o => o.CotizacionId == id);
            }

            return cotizacion == null ? NotFound() : View(cotizacion);
        }

        public async Task<IActionResult> DescargarPdf(int id)
        {
            var cotizacion = await ConsultaPermitidaAsync()
                .AsNoTracking()
                .Include(c => c.Cliente)
                .Include(c => c.TipoServicio)
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == id);

            if (cotizacion == null)
            {
                return NotFound();
            }

            var pdf = CotizacionPdfService.Crear(cotizacion);
            return File(pdf, "application/pdf", $"cotizacion-{cotizacion.IdCotizacion}.pdf");
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Solicitar()
        {
            await CargarTiposServicioAsync();
            return View(new SolicitarCotizacionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(30_000_000)]
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

            if (await ServicioRequiereVisitaAsync(model.TipoServicioId) && !model.FechaVisitaSolicitada.HasValue)
            {
                ModelState.AddModelError(nameof(model.FechaVisitaSolicitada), "Seleccione la fecha requerida para la visita.");
            }

            var codigoDtaCliente = cliente.Direccion?.UbicacionDTA?.CodigoDTA;
            if (string.IsNullOrWhiteSpace(codigoDtaCliente))
            {
                ModelState.AddModelError(string.Empty, "Complete la dirección DTA de su perfil antes de solicitar un servicio.");
            }
            else if (!await _context.Zonas.AnyAsync(z => z.Activo && z.CodigoDTA == codigoDtaCliente))
            {
                ModelState.AddModelError(string.Empty,
                    $"Actualmente no brindamos cobertura en {cliente.Direccion!.UbicacionDTA!.Distrito}, {cliente.Direccion.UbicacionDTA.Canton}. Consulte las zonas disponibles en la revista.");
            }

            await ValidarFotosAsync(model.FotosReferencia);
            if (!ModelState.IsValid)
            {
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            var estadoPendiente = await ObtenerEstadoAsync("Pendiente");
            var cotizacion = new Cotizacion
            {
                ClienteId = cliente.IdCliente,
                TipoServicioId = model.TipoServicioId!.Value,
                EstadoCotizacionId = estadoPendiente.Id,
                Descripcion = model.Descripcion.Trim(),
                FechaVisitaSolicitada = model.FechaVisitaSolicitada,
                FechaSolicitud = DateTime.UtcNow
            };
            _context.Cotizaciones.Add(cotizacion);
            await _context.SaveChangesAsync();
            await GuardarFotosAsync(cotizacion, model.FotosReferencia);

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
                Descripcion = cotizacion.Descripcion ?? "",
                FechaVisitaSolicitada = cotizacion.FechaVisitaSolicitada
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(30_000_000)]
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

            if (await ServicioRequiereVisitaAsync(model.TipoServicioId) && !model.FechaVisitaSolicitada.HasValue)
            {
                ModelState.AddModelError(nameof(model.FechaVisitaSolicitada), "Seleccione la fecha requerida para la visita.");
            }

            var fotosExistentes = await _context.FotosCotizacion.CountAsync(f => f.CotizacionId == id);
            await ValidarFotosAsync(model.FotosReferencia, fotosExistentes);
            if (!ModelState.IsValid)
            {
                await CargarTiposServicioAsync(model.TipoServicioId);
                return View(model);
            }

            cotizacion.TipoServicioId = model.TipoServicioId!.Value;
            cotizacion.Descripcion = model.Descripcion.Trim();
            cotizacion.FechaVisitaSolicitada = model.FechaVisitaSolicitada;
            await _context.SaveChangesAsync();
            await GuardarFotosAsync(cotizacion, model.FotosReferencia);

            TempData["SuccessMessage"] = "Cotización actualizada correctamente.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AgendarCita(int id)
        {
            var cotizacion = await ObtenerCotizacionEvaluadaDelClienteAsync(id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            return View(new AgendarCitaViewModel
            {
                IdCotizacion = id,
                FechaCompromiso = cotizacion.FechaVisitaSolicitada,
                UsarDireccionPerfil = cotizacion.UsarDireccionPerfil || string.IsNullOrWhiteSpace(cotizacion.EnlaceWaze),
                EnlaceWaze = cotizacion.EnlaceWaze,
                FormaPagoAceptada = cotizacion.FormaPagoAceptada ?? "",
                DireccionPerfilResumen = FormatearDireccion(cotizacion.Cliente?.Direccion)
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AgendarCita(int id, AgendarCitaViewModel model)
        {
            if (id != model.IdCotizacion)
            {
                return BadRequest();
            }

            var cotizacion = await ObtenerCotizacionEvaluadaDelClienteAsync(id);
            if (cotizacion == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                model.DireccionPerfilResumen = FormatearDireccion(cotizacion.Cliente?.Direccion);
                return View(model);
            }

            if (EsMedioDia(model.FechaCompromiso))
            {
                ModelState.AddModelError(nameof(model.FechaCompromiso), "El medio dia no esta disponible por horario de almuerzo. Seleccione otra hora.");
                model.DireccionPerfilResumen = FormatearDireccion(cotizacion.Cliente?.Direccion);
                return View(model);
            }

            if (!model.UsarDireccionPerfil)
            {
                if (string.IsNullOrWhiteSpace(model.EnlaceWaze))
                {
                    ModelState.AddModelError(nameof(model.EnlaceWaze), "Ingrese el enlace de Waze para la ubicacion de instalacion.");
                }
                else if (!EsEnlaceWazeValido(model.EnlaceWaze))
                {
                    ModelState.AddModelError(nameof(model.EnlaceWaze), "Ingrese un enlace valido de Waze.");
                }
            }
            else if (string.IsNullOrWhiteSpace(FormatearDireccion(cotizacion.Cliente?.Direccion)))
            {
                ModelState.AddModelError(nameof(model.UsarDireccionPerfil), "Actualice la direccion de su perfil antes de agendar la cita.");
                model.DireccionPerfilResumen = null;
                return View(model);
            }

            if (model.FormaPagoAceptada != "Completo" && model.FormaPagoAceptada != "AdelantoSaldo")
            {
                ModelState.AddModelError(nameof(model.FormaPagoAceptada), "Seleccione una forma de pago valida.");
            }

            if (!ModelState.IsValid)
            {
                model.DireccionPerfilResumen = FormatearDireccion(cotizacion.Cliente?.Direccion);
                return View(model);
            }

            cotizacion.AprobadaPorCliente = true;
            cotizacion.EstadoCotizacionId = (await ObtenerEstadoAsync("Aprobada")).Id;
            cotizacion.FechaVisitaSolicitada = model.FechaCompromiso;
            cotizacion.UsarDireccionPerfil = model.UsarDireccionPerfil;
            cotizacion.EnlaceWaze = model.UsarDireccionPerfil ? null : model.EnlaceWaze?.Trim();
            cotizacion.FormaPagoAceptada = model.FormaPagoAceptada;

            var orden = await _context.OrdenesServicio.FirstOrDefaultAsync(o => o.CotizacionId == cotizacion.IdCotizacion);
            if (orden == null)
            {
                var estadoPendiente = await _context.EstadosOrden.SingleAsync(e => e.Nombre == "Pendiente");
                _context.OrdenesServicio.Add(new OrdenServicio
                {
                    CotizacionId = cotizacion.IdCotizacion,
                    ClienteId = cotizacion.ClienteId,
                    EmpleadoId = null,
                    EstadoOrdenId = estadoPendiente.Id,
                    FechaCreacion = DateTime.UtcNow,
                    FechaCompromiso = model.FechaCompromiso,
                    CompromisoConfirmado = false,
                    UsarDireccionPerfil = model.UsarDireccionPerfil,
                    EnlaceWaze = model.UsarDireccionPerfil ? null : model.EnlaceWaze?.Trim()
                });
            }
            else
            {
                orden.FechaCompromiso = model.FechaCompromiso;
                orden.CompromisoConfirmado = false;
                orden.UsarDireccionPerfil = model.UsarDireccionPerfil;
                orden.EnlaceWaze = model.UsarDireccionPerfil ? null : model.EnlaceWaze?.Trim();
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Cita solicitada. El administrador confirmará el compromiso.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        public async Task<IActionResult> Foto(int id)
        {
            var foto = await _context.FotosCotizacion
                .AsNoTracking()
                .Include(f => f.Cotizacion)
                .ThenInclude(c => c!.Cliente)
                .FirstOrDefaultAsync(f => f.IdFotoCotizacion == id);

            if (foto?.Cotizacion == null)
            {
                return NotFound();
            }

            var email = User.Identity?.Name?.Trim().ToLowerInvariant();
            var puedeVer = User.IsInRole("Administrador") ||
                (User.IsInRole("Cliente") &&
                 foto.Cotizacion.Cliente?.Correo?.ToLower() == email);
            if (!puedeVer)
            {
                return NotFound();
            }

            var ruta = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, foto.Ruta));
            var raiz = Path.GetFullPath(Path.Combine(_environment.ContentRootPath, "App_Data", "cotizaciones"));
            if (!ruta.StartsWith(raiz, StringComparison.OrdinalIgnoreCase) || !System.IO.File.Exists(ruta))
            {
                return NotFound();
            }

            return PhysicalFile(ruta, foto.TipoContenido);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Evaluar(int id)
        {
            var cotizacion = await _context.Cotizaciones.AsNoTracking().FirstOrDefaultAsync(c => c.IdCotizacion == id);
            return cotizacion == null
                ? NotFound()
                : View(new EvaluarCotizacionViewModel
                {
                    IdCotizacion = id,
                    MontoPresupuesto = cotizacion.MontoPresupuesto,
                    RequiereAdelanto = cotizacion.RequiereAdelanto,
                    PorcentajeAdelanto = cotizacion.PorcentajeAdelanto
                });
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

            if (model.RequiereAdelanto && model.PorcentajeAdelanto is not (20 or 30 or 50))
            {
                ModelState.AddModelError(nameof(model.PorcentajeAdelanto), "Seleccione un adelanto de 20%, 30% o 50%.");
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
            cotizacion.RequiereAdelanto = model.RequiereAdelanto;
            cotizacion.PorcentajeAdelanto = model.RequiereAdelanto ? model.PorcentajeAdelanto : null;
            cotizacion.EstadoCotizacionId = (await ObtenerEstadoAsync("Evaluada")).Id;
            cotizacion.AprobadaPorCliente = false;
            cotizacion.FormaPagoAceptada = null;
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
                    FechaCreacion = DateTime.UtcNow,
                    FechaCompromiso = cotizacion.FechaVisitaSolicitada,
                    CompromisoConfirmado = false,
                    UsarDireccionPerfil = cotizacion.UsarDireccionPerfil,
                    EnlaceWaze = cotizacion.EnlaceWaze
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = aprobar ? "Cotización aprobada." : "Cotización rechazada.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        private IQueryable<Cotizacion> ConsultaPermitidaAsync()
        {
            if (EsPersonalAdministrativo())
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
            return await _context.Clientes
                .Include(c => c.Direccion)
                .ThenInclude(d => d!.UbicacionDTA)
                .FirstOrDefaultAsync(c => c.Correo != null && c.Correo.ToLower() == email && c.Estado == "Activo");
        }

        private async Task<Cliente?> BuscarClienteActivoAsync(string? identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador)) return null;

            var termino = identificador.Trim();
            var minuscula = termino.ToLower();
            var normalizado = termino.Replace("-", "").Replace(" ", "");
            return await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c =>
                c.Estado == "Activo" &&
                (c.Identificacion.Replace("-", "").Replace(" ", "") == normalizado ||
                 (c.Correo != null && c.Correo.ToLower() == minuscula) ||
                 (c.Telefono != null && c.Telefono.Replace("-", "").Replace(" ", "") == normalizado)));
        }

        private bool EsPersonalAdministrativo() =>
            User.IsInRole("Administrador") || User.IsInRole("Secretaria");

        private async Task<EstadoCotizacion> ObtenerEstadoAsync(string nombre)
        {
            return await _context.EstadosCotizacion.SingleAsync(e => e.Nombre == nombre);
        }

        private async Task CargarTiposServicioAsync(int? seleccionado = null)
        {
            ViewBag.TiposServicio = await _context.TiposServicio
                .AsNoTracking()
                .Where(t => t.Estado == "Activo")
                .OrderBy(t => t.Nombre)
                .ToListAsync();
            ViewBag.TipoServicioSeleccionado = seleccionado;
        }

        private async Task<bool> ServicioRequiereVisitaAsync(int? tipoServicioId)
        {
            return tipoServicioId.HasValue &&
                await _context.TiposServicio.AnyAsync(t => t.Id == tipoServicioId.Value && t.RequiereVisita);
        }

        private async Task<Cotizacion?> ObtenerCotizacionEvaluadaDelClienteAsync(int id)
        {
            var cliente = await ObtenerClienteActualAsync();
            if (cliente == null)
            {
                return null;
            }

            return await _context.Cotizaciones
                .Include(c => c.EstadoCotizacion)
                .Include(c => c.Cliente)
                    .ThenInclude(c => c!.Direccion)
                    .ThenInclude(d => d!.UbicacionDTA)
                .FirstOrDefaultAsync(c =>
                    c.IdCotizacion == id &&
                    c.ClienteId == cliente.IdCliente &&
                    c.EstadoCotizacion != null &&
                    c.EstadoCotizacion.Nombre == "Evaluada");
        }

        private static string? FormatearDireccion(Direccion? direccion)
        {
            if (direccion?.UbicacionDTA == null)
            {
                return null;
            }

            var ubicacion = direccion.UbicacionDTA;
            var partes = new List<string?>
            {
                direccion.OtrasSenas,
                ubicacion.Distrito,
                ubicacion.Canton,
                ubicacion.Provincia,
                "Costa Rica",
                $"DTA {ubicacion.CodigoDTA}"
            };

            return string.Join(", ", partes.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static bool EsMedioDia(DateTime? fecha)
        {
            return fecha.HasValue && fecha.Value.TimeOfDay == TimeSpan.FromHours(12);
        }

        private static bool EsEnlaceWazeValido(string enlace)
        {
            return Uri.TryCreate(enlace.Trim(), UriKind.Absolute, out var uri) &&
                   (uri.Host.Contains("waze.com", StringComparison.OrdinalIgnoreCase) ||
                    uri.Host.Contains("waze.to", StringComparison.OrdinalIgnoreCase));
        }

        private async Task ValidarFotosAsync(IEnumerable<IFormFile> fotos, int existentes = 0)
        {
            var archivos = fotos.Where(f => f.Length > 0).ToList();
            if (existentes + archivos.Count > 2)
            {
                ModelState.AddModelError(
                    nameof(SolicitarCotizacionViewModel.FotosReferencia),
                    "Puede adjuntar un máximo de 2 fotografías por cotización.");
            }

            foreach (var foto in archivos)
            {
                if (foto.Length > 5_000_000)
                {
                    ModelState.AddModelError(
                        nameof(SolicitarCotizacionViewModel.FotosReferencia),
                        $"La fotografía {Path.GetFileName(foto.FileName)} supera el límite de 5 MB.");
                    continue;
                }

                if (!await EsImagenPermitidaAsync(foto))
                {
                    ModelState.AddModelError(
                        nameof(SolicitarCotizacionViewModel.FotosReferencia),
                        $"El archivo {Path.GetFileName(foto.FileName)} no es una imagen JPEG, PNG o WebP válida.");
                }
            }
        }

        private async Task GuardarFotosAsync(Cotizacion cotizacion, IEnumerable<IFormFile> fotos)
        {
            var archivos = fotos.Where(f => f.Length > 0).ToList();
            if (archivos.Count == 0)
            {
                return;
            }

            var carpetaRelativa = Path.Combine("App_Data", "cotizaciones");
            var carpetaFisica = Path.Combine(_environment.ContentRootPath, carpetaRelativa);
            Directory.CreateDirectory(carpetaFisica);

            foreach (var foto in archivos)
            {
                var extension = foto.ContentType.ToLowerInvariant() switch
                {
                    "image/png" => ".png",
                    "image/webp" => ".webp",
                    _ => ".jpg"
                };
                var nombreInterno = $"{Guid.NewGuid():N}{extension}";
                var rutaRelativa = Path.Combine(carpetaRelativa, nombreInterno);
                await using var destino = System.IO.File.Create(
                    Path.Combine(_environment.ContentRootPath, rutaRelativa));
                await foto.CopyToAsync(destino);

                cotizacion.Fotos.Add(new FotoCotizacion
                {
                    Ruta = rutaRelativa,
                    NombreOriginal = Path.GetFileName(foto.FileName),
                    TipoContenido = foto.ContentType.ToLowerInvariant(),
                    FechaCarga = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
        }

        private static async Task<bool> EsImagenPermitidaAsync(IFormFile foto)
        {
            var tiposPermitidos = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!tiposPermitidos.Contains(foto.ContentType.ToLowerInvariant()))
            {
                return false;
            }

            var encabezado = new byte[12];
            await using var stream = foto.OpenReadStream();
            var leidos = await stream.ReadAsync(encabezado.AsMemory(0, encabezado.Length));
            if (leidos < 3)
            {
                return false;
            }

            var jpeg = encabezado[0] == 0xFF && encabezado[1] == 0xD8 && encabezado[2] == 0xFF;
            var png = leidos >= 8 &&
                encabezado[0] == 0x89 && encabezado[1] == 0x50 &&
                encabezado[2] == 0x4E && encabezado[3] == 0x47 &&
                encabezado[4] == 0x0D && encabezado[5] == 0x0A &&
                encabezado[6] == 0x1A && encabezado[7] == 0x0A;
            var webp = leidos >= 12 &&
                encabezado[0] == 0x52 && encabezado[1] == 0x49 &&
                encabezado[2] == 0x46 && encabezado[3] == 0x46 &&
                encabezado[8] == 0x57 && encabezado[9] == 0x45 &&
                encabezado[10] == 0x42 && encabezado[11] == 0x50;

            return jpeg || png || webp;
        }
    }
}
