using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.Data;
using MultiservicioB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace MultiservicioB.Controllers
{
    [Authorize]
    public class EmpleadosController : Controller
    {
        private const string CorreoAdministradorPrincipal = "admin@multiserviciosb.com";
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;

        public EmpleadosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Empleados
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Empleados.AsNoTracking().ToListAsync();
            var rolesPorEmpleado = new Dictionary<int, string>();
            foreach (var empleado in lista)
            {
                if (EsAdministradorPrincipal(empleado))
                {
                    rolesPorEmpleado[empleado.IdEmpleado] = "Administrador principal";
                    continue;
                }

                IdentityUser? usuario = null;
                if (!string.IsNullOrWhiteSpace(empleado.UserId))
                    usuario = await _userManager.FindByIdAsync(empleado.UserId);
                usuario ??= await _userManager.FindByEmailAsync(empleado.CorreoElectronicoEmpleado);

                var roles = usuario == null
                    ? Array.Empty<string>()
                    : (await _userManager.GetRolesAsync(usuario)).ToArray();
                rolesPorEmpleado[empleado.IdEmpleado] = roles.Contains("Administrador")
                    ? "Administrador"
                    : roles.Contains("Secretaria") ? "Secretaría" : "Técnico";
            }
            ViewBag.RolesPorEmpleado = rolesPorEmpleado;
            return View(lista);
        }

        [Authorize(Roles = "Empleado")]
        public async Task<IActionResult> MiPerfil()
        {
            var userId = _userManager.GetUserId(User);
            var email = User.Identity?.Name?.Trim().ToLower();
            var empleado = await _context.Empleados.AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.UserId == userId ||
                    (email != null && e.CorreoElectronicoEmpleado.ToLower() == email));

            if (empleado == null)
            {
                return NotFound();
            }

            if (!EstadosEmpleado.PuedeAcceder(empleado))
            {
                TempData["ErrorMessage"] = "Su perfil de empleado no se encuentra activo.";
                return RedirectToAction("Dashboard", "Home");
            }

            return View(empleado);
        }

        // GET: Empleados/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Empleados/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Create([Bind("IdentificacionEmpleado,NombreEmpleado,ApellidosEmpleado,CorreoElectronicoEmpleado,TelefonoEmpleado,SalarioBase,FechaInicioEmpleado,RolInicial")] Empleado empleado, IFormFile? fotoPerfil)
        {
            ValidarFotoPerfil(fotoPerfil);
            if (!ModelState.IsValid)
                return View(empleado);

            string email = empleado.CorreoElectronicoEmpleado.Trim().ToLower();

            if (empleado.RolInicial is not ("Empleado" or "Secretaria"))
            {
                ModelState.AddModelError("RolInicial", "Seleccione un rol válido para el personal.");
                return View(empleado);
            }

            if (email == "admin@multiserviciosb.com" && empleado.RolInicial == "Secretaria")
            {
                ModelState.AddModelError("RolInicial", "El administrador principal no puede tener el rol de Secretaría.");
                return View(empleado);
            }

            // Validar dominio corporativo según requerimiento de HU
            if (!email.EndsWith("@multiserviciosb.com"))
            {
                ModelState.AddModelError("CorreoElectronicoEmpleado", "El correo de un empleado debe pertenecer al dominio @multiserviciosb.com");
                return View(empleado);
            }

            // Validar duplicados en Identity de forma preventiva
            var usuarioExistente = await _userManager.FindByEmailAsync(email);
            if (usuarioExistente != null)
            {
                ModelState.AddModelError("CorreoElectronicoEmpleado", "Este correo electrónico ya se encuentra registrado en el sistema de seguridad.");
                return View(empleado);
            }

            // Validar duplicados en la tabla de negocio
            bool empleadoExistente = await _context.Empleados.AnyAsync(e => e.CorreoElectronicoEmpleado.Trim().ToLower() == email);
            if (empleadoExistente)
            {
                ModelState.AddModelError("CorreoElectronicoEmpleado", "Este empleado ya se encuentra registrado en el módulo de personal.");
                return View(empleado);
            }

            await using var transaccion = await _context.Database.BeginTransactionAsync();
            var usuario = new IdentityUser { UserName = email, Email = email };
            var creacionUsuario = await _userManager.CreateAsync(usuario);
            if (!creacionUsuario.Succeeded)
            {
                ModelState.AddModelError(string.Empty, string.Join(" ", creacionUsuario.Errors.Select(e => e.Description)));
                return View(empleado);
            }

            var asignacionEmpleado = await _userManager.AddToRoleAsync(usuario, "Empleado");
            var asignacionRol = empleado.RolInicial == "Secretaria"
                ? await _userManager.AddToRoleAsync(usuario, "Secretaria")
                : IdentityResult.Success;
            if (!asignacionEmpleado.Succeeded || !asignacionRol.Succeeded)
            {
                await transaccion.RollbackAsync();
                ModelState.AddModelError(string.Empty, "No fue posible asignar el rol seleccionado al empleado.");
                return View(empleado);
            }

            empleado.CorreoElectronicoEmpleado = email;
            empleado.FotoPerfil = await GuardarFotoPerfilAsync(fotoPerfil);
            empleado.TieneUsuario = false;
            empleado.UserId = usuario.Id;
            EstadosEmpleado.Aplicar(empleado, EstadosEmpleado.Pendiente);

            _context.Add(empleado);
            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int id)
        {
            var empleado = await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(e => e.IdEmpleado == id);
            return empleado == null ? NotFound() : View(empleado);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            return empleado == null ? NotFound() : View(empleado);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("IdEmpleado,IdentificacionEmpleado,NombreEmpleado,ApellidosEmpleado,TelefonoEmpleado,SalarioBase,FechaInicioEmpleado,FechaFinalizacionEmpleado")] Empleado formulario,
            string estadoGestion,
            IFormFile? fotoPerfil)
        {
            if (id != formulario.IdEmpleado)
            {
                return BadRequest();
            }

            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            ValidarFotoPerfil(fotoPerfil);

            ModelState.Remove(nameof(Empleado.CorreoElectronicoEmpleado));
            ModelState.Remove(nameof(Empleado.EstadoAcceso));
            if (!ModelState.IsValid)
            {
                formulario.CorreoElectronicoEmpleado = empleado.CorreoElectronicoEmpleado;
                formulario.TieneUsuario = empleado.TieneUsuario;
                formulario.EstadoAcceso = empleado.EstadoAcceso;
                ViewData["EstadoGestion"] = estadoGestion;
                return View(formulario);
            }

            var estadoAnterior = EstadosEmpleado.Obtener(empleado);
            if (EsAdministradorPrincipal(empleado) && estadoGestion != EstadosEmpleado.Activo)
            {
                ModelState.AddModelError(string.Empty, "El administrador principal no puede ser desactivado ni quedar pendiente.");
                formulario.CorreoElectronicoEmpleado = empleado.CorreoElectronicoEmpleado;
                formulario.TieneUsuario = empleado.TieneUsuario;
                formulario.EstadoAcceso = empleado.EstadoAcceso;
                ViewData["EstadoGestion"] = EstadosEmpleado.Activo;
                return View(formulario);
            }

            empleado.IdentificacionEmpleado = formulario.IdentificacionEmpleado;
            empleado.NombreEmpleado = formulario.NombreEmpleado;
            empleado.ApellidosEmpleado = formulario.ApellidosEmpleado;
            empleado.TelefonoEmpleado = formulario.TelefonoEmpleado;
            empleado.SalarioBase = formulario.SalarioBase;
            empleado.FechaInicioEmpleado = formulario.FechaInicioEmpleado;
            empleado.FechaFinalizacionEmpleado = formulario.FechaFinalizacionEmpleado;
            if (fotoPerfil is { Length: > 0 })
            {
                var fotoAnterior = empleado.FotoPerfil;
                empleado.FotoPerfil = await GuardarFotoPerfilAsync(fotoPerfil);
                EliminarFotoPerfil(fotoAnterior);
            }
            EstadosEmpleado.Aplicar(empleado, estadoGestion);
            await _context.SaveChangesAsync();

            if (estadoAnterior != EstadosEmpleado.Obtener(empleado) &&
                !string.IsNullOrWhiteSpace(empleado.UserId))
            {
                var usuario = await _userManager.FindByIdAsync(empleado.UserId);
                if (usuario != null)
                {
                    await _userManager.UpdateSecurityStampAsync(usuario);
                }
            }

            TempData["SuccessMessage"] = "Empleado actualizado.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var empleado = await _context.Empleados.AsNoTracking().FirstOrDefaultAsync(e => e.IdEmpleado == id);
            if (empleado == null) return NotFound();
            if (EsAdministradorPrincipal(empleado))
            {
                TempData["ErrorMessage"] = "El administrador principal está protegido y no puede darse de baja.";
                return RedirectToAction(nameof(Index));
            }
            return View(empleado);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null)
            {
                return NotFound();
            }

            if (EsAdministradorPrincipal(empleado))
            {
                TempData["ErrorMessage"] = "El administrador principal está protegido y no puede darse de baja.";
                return RedirectToAction(nameof(Index));
            }

            EstadosEmpleado.Aplicar(empleado, EstadosEmpleado.Inactivo);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(empleado.UserId))
            {
                var usuario = await _userManager.FindByIdAsync(empleado.UserId);
                if (usuario != null)
                {
                    await _userManager.UpdateSecurityStampAsync(usuario);
                }
            }

            TempData["SuccessMessage"] = "Empleado dado de baja.";
            return RedirectToAction(nameof(Index));
        }

        private static bool EsAdministradorPrincipal(Empleado empleado) =>
            string.Equals(empleado.CorreoElectronicoEmpleado?.Trim(), CorreoAdministradorPrincipal, StringComparison.OrdinalIgnoreCase);

        private void ValidarFotoPerfil(IFormFile? foto)
        {
            if (foto == null || foto.Length == 0) return;
            if (foto.Length > 5_000_000) ModelState.AddModelError("fotoPerfil", "La fotografía no puede superar los 5 MB.");
            if (foto.ContentType.ToLowerInvariant() is not ("image/jpeg" or "image/png" or "image/webp"))
                ModelState.AddModelError("fotoPerfil", "Seleccione una imagen JPEG, PNG o WebP.");
        }

        private async Task<string?> GuardarFotoPerfilAsync(IFormFile? foto)
        {
            if (foto == null || foto.Length == 0) return null;
            var extension = foto.ContentType.ToLowerInvariant() switch { "image/png" => ".png", "image/webp" => ".webp", _ => ".jpg" };
            var carpeta = Path.Combine(_environment.WebRootPath, "images", "empleados");
            Directory.CreateDirectory(carpeta);
            var nombre = $"{Guid.NewGuid():N}{extension}";
            await using var destino = System.IO.File.Create(Path.Combine(carpeta, nombre));
            await foto.CopyToAsync(destino);
            return $"/images/empleados/{nombre}";
        }

        private void EliminarFotoPerfil(string? ruta)
        {
            if (string.IsNullOrWhiteSpace(ruta) || !ruta.StartsWith("/images/empleados/", StringComparison.OrdinalIgnoreCase)) return;
            var destino = Path.Combine(_environment.WebRootPath, "images", "empleados", Path.GetFileName(ruta));
            if (System.IO.File.Exists(destino)) System.IO.File.Delete(destino);
        }

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.IdEmpleado == id);
        }
    }
}
