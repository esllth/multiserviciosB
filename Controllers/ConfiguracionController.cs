using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ConfiguracionController : BaseController
    {
        private readonly IConfiguracionService _configuracionService;
        private readonly ApplicationDbContext _context;

        public ConfiguracionController(IConfiguracionService configuracionService, ApplicationDbContext context)
        {
            _configuracionService = configuracionService;
            _context = context;
        }

        //  INDEX 

        public IActionResult Index()
        {
            return View();
        }

        //  HORARIOS 

        public async Task<IActionResult> Horarios()
        {
            var horarios = await _configuracionService.GetHorariosAsync();
            return View(horarios);
        }

        public IActionResult CrearHorario()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearHorario(HorarioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _configuracionService.CrearHorarioAsync(dto);
            TempData["SuccessMessage"] = "Horario creado exitosamente";
            return RedirectToAction(nameof(Horarios));
        }

        public async Task<IActionResult> EditarHorario(int id)
        {
            var horario = await _configuracionService.GetHorarioByIdAsync(id);
            if (horario == null) return NotFound();
            return View(horario);
        }

        public async Task<IActionResult> ConfirmarEliminarHorario(int id)
        {
            var horario = await _configuracionService.GetHorarioByIdAsync(id);
            if (horario == null) return NotFound();
            return View(horario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarHorario(HorarioDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _configuracionService.ActualizarHorarioAsync(dto);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Horario actualizado exitosamente";
            return RedirectToAction(nameof(Horarios));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarHorario(int id)
        {
            var resultado = await _configuracionService.EliminarHorarioAsync(id);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Horario eliminado exitosamente";
            return RedirectToAction(nameof(Horarios));
        }

        //  ZONAS 

        public async Task<IActionResult> Zonas()
        {
            var zonas = await _configuracionService.GetZonasAsync();
            return View(zonas);
        }

        public IActionResult CrearZona()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearZona(ZonaDTO dto)
        {
            if (await _context.Zonas.AnyAsync(z => z.CodigoDTA == dto.CodigoDTA))
                ModelState.AddModelError(nameof(dto.CodigoDTA), "Este distrito ya está registrado como zona de cobertura.");
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _configuracionService.CrearZonaAsync(dto);
            TempData["SuccessMessage"] = "Zona creada exitosamente";
            return RedirectToAction(nameof(Zonas));
        }

        public async Task<IActionResult> EditarZona(int id)
        {
            var zona = await _configuracionService.GetZonaByIdAsync(id);
            if (zona == null) return NotFound();
            return View(zona);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarZona(ZonaDTO dto)
        {
            if (await _context.Zonas.AnyAsync(z => z.Id != dto.Id && z.CodigoDTA == dto.CodigoDTA))
                ModelState.AddModelError(nameof(dto.CodigoDTA), "Este distrito ya está registrado como zona de cobertura.");
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var resultado = await _configuracionService.ActualizarZonaAsync(dto);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Zona actualizada exitosamente";
            return RedirectToAction(nameof(Zonas));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarZona(int id)
        {
            var resultado = await _configuracionService.EliminarZonaAsync(id);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Zona eliminada exitosamente";
            return RedirectToAction(nameof(Zonas));
        }

        //  CONFIG GENERAL 

        public async Task<IActionResult> ConfiguracionGeneral()
        {
            return View(await _configuracionService.GetRevistaNosotrosAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarRevistaNosotros(RevistaNosotrosDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return View("ConfiguracionGeneral", dto);
            }

            await _configuracionService.GuardarRevistaNosotrosAsync(dto);
            TempData["SuccessMessage"] = "La sección Nosotros de la revista se actualizó correctamente.";
            return RedirectToAction(nameof(ConfiguracionGeneral));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarConfiguracion(ConfiguracionSistemaDTO dto)
        {
            if (!ModelState.IsValid)
            {
                var configuraciones = await _configuracionService.GetConfiguracionesAsync();
                return View("ConfiguracionGeneral", configuraciones);
            }

            var resultado = await _configuracionService.ActualizarConfiguracionAsync(dto);
            if (!resultado) return NotFound();

            TempData["SuccessMessage"] = "Configuración actualizada exitosamente";
            return RedirectToAction(nameof(ConfiguracionGeneral));
        }
    }
}
