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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Cliente,Administrador")]
    public class TecnicosController : BaseController
    {
        private const string TituloTrabajoCompletado = "Tecnico completo el trabajo";
        private const long MaximoBytesFoto = 5_000_000;
        private readonly IOrdenServicioService _ordenService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public TecnicosController(
            IOrdenServicioService ordenService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment environment)
        {
            _ordenService = ordenService;
            _userManager = userManager;
            _context = context;
            _environment = environment;
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
            }

            TempData["SuccessMessage"] = "Aviso enviado al administrador.";
            return RedirectToAction(nameof(Detalle), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Empleado,Administrador")]
        [RequestSizeLimit(30_000_000)]
        public async Task<IActionResult> AdjuntarEvidencia(int id, string tipoFoto, string? descripcion, List<IFormFile> archivos)
        {
            if (!await PuedeGestionarOrdenAsync(id))
            {
                return NotFound();
            }

            if (tipoFoto != "Inicial" && tipoFoto != "Final")
            {
                TempData["ErrorMessage"] = "Seleccione si la evidencia corresponde al estado inicial o final.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            if (archivos == null || archivos.Count == 0 || archivos.All(a => a.Length == 0))
            {
                TempData["ErrorMessage"] = "Seleccione al menos una fotografia para adjuntar.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            foreach (var archivo in archivos.Where(a => a.Length > 0))
            {
                var error = await ValidarFotoAsync(archivo);
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
                foreach (var archivo in archivos.Where(a => a.Length > 0))
                {
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
                        TipoFoto = tipoFoto,
                        FechaCarga = DateTime.Now,
                        Descripcion = string.IsNullOrWhiteSpace(descripcion)
                            ? null
                            : LimitarTexto(descripcion.Trim(), 500)
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

            TempData["SuccessMessage"] = tipoFoto == "Inicial"
                ? "Evidencia inicial adjuntada correctamente."
                : "Evidencia final adjuntada correctamente.";
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

            var result = await _ordenService.FinalizarOrdenAsync(id, "Orden finalizada por administrador");
            if (!result)
            {
                TempData["ErrorMessage"] = "No se pudo finalizar la orden. Verifique que cumple con todos los requisitos.";
                return RedirectToAction(nameof(Detalle), new { id });
            }

            await MarcarAvisosTrabajoCompletadoAsync(id);

            TempData["SuccessMessage"] = "Orden finalizada exitosamente";
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

            var estadoCompletada = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Completada");
            if (estadoCompletada == null)
            {
                TempData["ErrorMessage"] = "No se encontro el estado Completada.";
                return volverDashboard
                    ? RedirectToAction("Dashboard", "Home")
                    : RedirectToAction(nameof(Detalle), new { id });
            }

            orden.FechaFin = DateTime.Now;
            orden.EstadoOrdenId = estadoCompletada.Id;
            orden.ComentariosFinales = "Orden cerrada por administrador desde aviso de trabajo completado.";

            await MarcarAvisosTrabajoCompletadoAsync(id, guardarCambios: false);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Orden cerrada desde el aviso pendiente.";
            return volverDashboard
                ? RedirectToAction("Dashboard", "Home")
                : RedirectToAction(nameof(Detalle), new { id });
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
                EstadoOrdenId = orden.EstadoOrdenId,
                FechaCompromiso = orden.FechaCompromiso,
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
            orden.FechaCompromiso = model.FechaCompromiso;
            orden.CompromisoConfirmado = model.CompromisoConfirmado;
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
