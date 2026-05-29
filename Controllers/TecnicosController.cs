using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Administrador")]
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

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var empleado = _context.Empleados.FirstOrDefault(e => e.UserId == user.Id);

            if (empleado != null)
            {
                var ordenes = await _ordenService.GetByTecnicoAsync(empleado.IdEmpleado);
                return View(ordenes);
            }

            var todasOrdenes = await _ordenService.GetAllAsync();
            return View(todasOrdenes);
        }

        public async Task<IActionResult> Detalle(int id)
        {
            var orden = await _ordenService.GetByIdAsync(id);
            if (orden == null)
            {
                return NotFound();
            }
            return View(orden);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarOrden(int id)
        {
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
        public async Task<IActionResult> FinalizarOrden(int id)
        {
            var result = await _ordenService.FinalizarOrdenAsync(id);
            if (!result)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Orden finalizada exitosamente";
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
        public async Task<IActionResult> Crear(OrdenServicioDTO ordenDto)
        {
            if (!ModelState.IsValid)
            {
                return View(ordenDto);
            }

            await _ordenService.CreateAsync(ordenDto);
            TempData["SuccessMessage"] = "Orden de servicio creada exitosamente";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Empleado,Administrador")]
        public IActionResult Tecnicos()
        {
            return RedirectToAction(nameof(Index));
        }
    }
}
