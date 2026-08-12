using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    public class RevistaController : BaseController
    {
        private const int MaximoBytesImagen = 5_000_000;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguracionService _configuracionService;
        private readonly ApplicationDbContext _context;
        private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public RevistaController(
            IWebHostEnvironment environment,
            IConfiguracionService configuracionService,
            ApplicationDbContext context)
        {
            _environment = environment;
            _configuracionService = configuracionService;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var contenido = await CargarContenidoAsync();
            contenido.HorariosDisponibles = await ObtenerHorariosDisponiblesAsync();
            contenido.ZonasCobertura = (await _configuracionService.GetZonasAsync())
                .Where(z => z.Activo)
                .OrderBy(z => z.Provincia).ThenBy(z => z.Canton).ThenBy(z => z.Distrito)
                .ToList();
            contenido.Nosotros = await _configuracionService.GetRevistaNosotrosAsync();
            return View(contenido);
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar()
        {
            return View(MapearEdicion(await CargarContenidoAsync()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        [RequestSizeLimit(100_000_000)]
        public async Task<IActionResult> Editar(RevistaEditarViewModel model)
        {
            var publicacionesEliminadas = model.Tarjetas
                .Where(t => t.Eliminar && t.IdPublicacion > 0)
                .Select(t => t.IdPublicacion)
                .ToList();
            model.Tarjetas = model.Tarjetas.Where(t => !t.Eliminar).ToList();

            await ValidarImagenAsync(model.ImagenPrincipal, nameof(model.ImagenPrincipal));
            for (var i = 0; i < model.Tarjetas.Count; i++)
            {
                await ValidarImagenAsync(model.Tarjetas[i].NuevaImagen, $"Tarjetas[{i}].NuevaImagen");
                if (string.IsNullOrWhiteSpace(model.Tarjetas[i].ImagenActual) && model.Tarjetas[i].NuevaImagen == null)
                    ModelState.AddModelError($"Tarjetas[{i}].NuevaImagen", "Seleccione una imagen para esta publicación.");
            }
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var contenido = new RevistaViewModel
            {
                Titulo = model.Titulo.Trim(),
                Descripcion = model.Descripcion.Trim(),
                Encabezado = model.Encabezado.Trim(),
                Subtitulo = model.Subtitulo.Trim(),
                ImagenPrincipal = await GuardarImagenAsync(model.ImagenPrincipal, model.ImagenPrincipalActual),
                Tarjetas = new List<RevistaTarjetaViewModel>()
            };

            foreach (var tarjeta in model.Tarjetas)
            {
                contenido.Tarjetas.Add(new RevistaTarjetaViewModel
                {
                    Titulo = tarjeta.Titulo.Trim(),
                    IdPublicacion = tarjeta.IdPublicacion,
                    Descripcion = tarjeta.Descripcion.Trim(),
                    TextoEnlace = tarjeta.TextoEnlace.Trim(),
                    Imagen = await GuardarImagenAsync(tarjeta.NuevaImagen, tarjeta.ImagenActual)
                });
            }

            await GuardarPublicacionesAsync(contenido.Tarjetas, publicacionesEliminadas);
            await GuardarContenidoAsync(contenido);
            TempData["SuccessMessage"] = "La revista se actualizó correctamente.";
            return RedirectToAction(nameof(Editar));
        }

        [AllowAnonymous]
        public IActionResult Imagen(string nombre)
        {
            var nombreSeguro = Path.GetFileName(nombre);
            if (string.IsNullOrWhiteSpace(nombreSeguro) || nombreSeguro != nombre)
            {
                return NotFound();
            }

            var ruta = Path.Combine(ObtenerCarpeta(), nombreSeguro);
            return System.IO.File.Exists(ruta)
                ? PhysicalFile(ruta, ObtenerTipoContenido(Path.GetExtension(nombreSeguro)))
                : NotFound();
        }

        private async Task<RevistaViewModel> CargarContenidoAsync()
        {
            var ruta = ObtenerRutaContenido();
            RevistaViewModel contenido;
            if (!System.IO.File.Exists(ruta))
            {
                contenido = new RevistaViewModel();
            }
            else try
            {
                await using var archivo = System.IO.File.OpenRead(ruta);
                contenido = await JsonSerializer.DeserializeAsync<RevistaViewModel>(archivo)
                    ?? new RevistaViewModel();
            }
            catch (JsonException)
            {
                contenido = new RevistaViewModel();
            }

            var publicaciones = await _context.RevistaPublicaciones.AsNoTracking()
                .Where(p => p.Activo).OrderBy(p => p.Orden).ThenBy(p => p.IdPublicacion).ToListAsync();
            if (publicaciones.Count > 0)
            {
                contenido.Tarjetas = publicaciones.Select(p => new RevistaTarjetaViewModel
                {
                    IdPublicacion = p.IdPublicacion,
                    Titulo = p.Titulo,
                    Descripcion = p.Descripcion,
                    Imagen = p.Imagen,
                    TextoEnlace = p.TextoEnlace
                }).ToList();
            }
            return contenido;
        }

        private async Task GuardarPublicacionesAsync(
            List<RevistaTarjetaViewModel> tarjetas,
            List<int> idsEliminados)
        {
            if (idsEliminados.Count > 0)
            {
                var eliminadas = await _context.RevistaPublicaciones
                    .Where(p => idsEliminados.Contains(p.IdPublicacion)).ToListAsync();
                _context.RevistaPublicaciones.RemoveRange(eliminadas);
            }

            for (var i = 0; i < tarjetas.Count; i++)
            {
                var tarjeta = tarjetas[i];
                RevistaPublicacion? publicacion = null;
                if (tarjeta.IdPublicacion > 0)
                    publicacion = await _context.RevistaPublicaciones.FindAsync(tarjeta.IdPublicacion);

                if (publicacion == null)
                {
                    publicacion = new RevistaPublicacion();
                    _context.RevistaPublicaciones.Add(publicacion);
                }
                publicacion.Titulo = tarjeta.Titulo;
                publicacion.Descripcion = tarjeta.Descripcion;
                publicacion.Imagen = tarjeta.Imagen;
                publicacion.TextoEnlace = tarjeta.TextoEnlace;
                publicacion.Orden = i + 1;
                publicacion.Activo = true;
            }
            await _context.SaveChangesAsync();
        }

        private async Task GuardarContenidoAsync(RevistaViewModel contenido)
        {
            contenido.HorariosDisponibles = new List<HorarioDTO>();
            contenido.ZonasCobertura = new List<ZonaDTO>();
            Directory.CreateDirectory(ObtenerCarpeta());
            await using var archivo = System.IO.File.Create(ObtenerRutaContenido());
            await JsonSerializer.SerializeAsync(archivo, contenido, _jsonOptions);
        }

        private async Task<List<HorarioDTO>> ObtenerHorariosDisponiblesAsync()
        {
            var horarios = await _configuracionService.GetHorariosAsync();
            return horarios
                .Where(h => h.Activo)
                .OrderBy(h => OrdenDia(h.DiaSemana))
                .ThenBy(h => h.HoraInicio)
                .ToList();
        }

        private static int OrdenDia(string dia)
        {
            return NormalizarDia(dia) switch
            {
                "lunes" => 1,
                "martes" => 2,
                "miercoles" => 3,
                "jueves" => 4,
                "viernes" => 5,
                "sabado" => 6,
                "domingo" => 7,
                _ => 99
            };
        }

        private static string NormalizarDia(string dia)
        {
            return dia.Trim().ToLowerInvariant()
                .Replace("á", "a")
                .Replace("é", "e")
                .Replace("í", "i")
                .Replace("ó", "o")
                .Replace("ú", "u");
        }

        private async Task<string> GuardarImagenAsync(IFormFile? imagen, string? actual)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return actual ?? string.Empty;
            }

            Directory.CreateDirectory(ObtenerCarpeta());
            var extension = imagen.ContentType.ToLowerInvariant() switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };
            var nombre = $"{Guid.NewGuid():N}{extension}";
            await using var destino = System.IO.File.Create(Path.Combine(ObtenerCarpeta(), nombre));
            await imagen.CopyToAsync(destino);

            EliminarImagenAnterior(actual);
            return $"archivo:{nombre}";
        }

        private void EliminarImagenAnterior(string? imagen)
        {
            if (string.IsNullOrWhiteSpace(imagen) ||
                !imagen.StartsWith("archivo:", StringComparison.Ordinal))
            {
                return;
            }

            var nombre = Path.GetFileName(imagen["archivo:".Length..]);
            var ruta = Path.Combine(ObtenerCarpeta(), nombre);
            if (System.IO.File.Exists(ruta))
            {
                System.IO.File.Delete(ruta);
            }
        }

        private async Task ValidarImagenAsync(IFormFile? imagen, string campo)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return;
            }

            if (imagen.Length > MaximoBytesImagen)
            {
                ModelState.AddModelError(campo, "La imagen no puede superar los 5 MB.");
                return;
            }

            if (!await EsImagenPermitidaAsync(imagen))
            {
                ModelState.AddModelError(campo, "Seleccione una imagen JPEG, PNG o WebP válida.");
            }
        }

        private static async Task<bool> EsImagenPermitidaAsync(IFormFile imagen)
        {
            var tipo = imagen.ContentType.ToLowerInvariant();
            if (tipo is not ("image/jpeg" or "image/png" or "image/webp"))
            {
                return false;
            }

            var encabezado = new byte[12];
            await using var stream = imagen.OpenReadStream();
            var leidos = await stream.ReadAsync(encabezado.AsMemory());
            return tipo switch
            {
                "image/jpeg" => leidos >= 3 && encabezado[0] == 0xFF && encabezado[1] == 0xD8 && encabezado[2] == 0xFF,
                "image/png" => leidos >= 8 && encabezado[..8].SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
                "image/webp" => leidos >= 12
                    && encabezado[..4].SequenceEqual("RIFF"u8)
                    && encabezado[8..12].SequenceEqual("WEBP"u8),
                _ => false
            };
        }

        private static RevistaEditarViewModel MapearEdicion(RevistaViewModel contenido) =>
            new()
            {
                Titulo = contenido.Titulo,
                Descripcion = contenido.Descripcion,
                Encabezado = contenido.Encabezado,
                Subtitulo = contenido.Subtitulo,
                ImagenPrincipalActual = contenido.ImagenPrincipal,
                Tarjetas = contenido.Tarjetas.Select(t => new RevistaTarjetaEditarViewModel
                {
                    IdPublicacion = t.IdPublicacion,
                    Titulo = t.Titulo,
                    Descripcion = t.Descripcion,
                    ImagenActual = t.Imagen,
                    TextoEnlace = t.TextoEnlace
                }).ToList()
            };

        private string ObtenerCarpeta() =>
            Path.Combine(_environment.ContentRootPath, "App_Data", "revista");

        private string ObtenerRutaContenido() =>
            Path.Combine(ObtenerCarpeta(), "contenido.json");

        private static string ObtenerTipoContenido(string extension) =>
            extension.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "image/jpeg"
            };
    }
}
