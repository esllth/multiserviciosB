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
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public EmpleadosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Empleados
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Empleados.AsNoTracking().ToListAsync();
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
        public async Task<IActionResult> Create([Bind("IdentificacionEmpleado,NombreEmpleado,ApellidosEmpleado,CorreoElectronicoEmpleado,TelefonoEmpleado,SalarioBase,FechaInicioEmpleado")] Empleado empleado)
        {
            if (!ModelState.IsValid)
                return View(empleado);

            string email = empleado.CorreoElectronicoEmpleado.Trim().ToLower();

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

            // El usuario Identity se crea cuando el empleado configura su contraseña.
            empleado.CorreoElectronicoEmpleado = email;
            empleado.TieneUsuario = false;
            empleado.UserId = null;
            EstadosEmpleado.Aplicar(empleado, EstadosEmpleado.Pendiente);

            _context.Add(empleado);
            await _context.SaveChangesAsync();

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
            string estadoGestion)
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
            empleado.IdentificacionEmpleado = formulario.IdentificacionEmpleado;
            empleado.NombreEmpleado = formulario.NombreEmpleado;
            empleado.ApellidosEmpleado = formulario.ApellidosEmpleado;
            empleado.TelefonoEmpleado = formulario.TelefonoEmpleado;
            empleado.SalarioBase = formulario.SalarioBase;
            empleado.FechaInicioEmpleado = formulario.FechaInicioEmpleado;
            empleado.FechaFinalizacionEmpleado = formulario.FechaFinalizacionEmpleado;
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
            return empleado == null ? NotFound() : View(empleado);
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

        private bool EmpleadoExists(int id)
        {
            return _context.Empleados.Any(e => e.IdEmpleado == id);
        }
    }
}
