using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Controllers
{
    public class RevistaController : BaseController
    {
        private const int MaximoBytesImagen = 5_000_000;
        private readonly IWebHostEnvironment _environment;
        private readonly JsonSerializerOptions _jsonOptions = new() { WriteIndented = true };

        public RevistaController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await CargarContenidoAsync());
        }

        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Editar()
        {
            return View(MapearEdicion(await CargarContenidoAsync()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        [RequestSizeLimit(40_000_000)]
        public async Task<IActionResult> Editar(RevistaEditarViewModel model)
        {
            if (model.Tarjetas.Count != 6)
            {
                ModelState.AddModelError("", "La revista debe conservar sus seis secciones de trabajos.");
            }

            await ValidarImagenAsync(model.ImagenPrincipal, nameof(model.ImagenPrincipal));
            for (var i = 0; i < model.Tarjetas.Count; i++)
            {
                await ValidarImagenAsync(model.Tarjetas[i].NuevaImagen, $"Tarjetas[{i}].NuevaImagen");
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
                    Descripcion = tarjeta.Descripcion.Trim(),
                    TextoEnlace = tarjeta.TextoEnlace.Trim(),
                    Imagen = await GuardarImagenAsync(tarjeta.NuevaImagen, tarjeta.ImagenActual)
                });
            }

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
            if (!System.IO.File.Exists(ruta))
            {
                return new RevistaViewModel();
            }

            try
            {
                await using var archivo = System.IO.File.OpenRead(ruta);
                return await JsonSerializer.DeserializeAsync<RevistaViewModel>(archivo)
                    ?? new RevistaViewModel();
            }
            catch (JsonException)
            {
                return new RevistaViewModel();
            }
        }

        private async Task GuardarContenidoAsync(RevistaViewModel contenido)
        {
            Directory.CreateDirectory(ObtenerCarpeta());
            await using var archivo = System.IO.File.Create(ObtenerRutaContenido());
            await JsonSerializer.SerializeAsync(archivo, contenido, _jsonOptions);
        }

        private async Task<string> GuardarImagenAsync(IFormFile? imagen, string actual)
        {
            if (imagen == null || imagen.Length == 0)
            {
                return actual;
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

        private void EliminarImagenAnterior(string imagen)
        {
            if (!imagen.StartsWith("archivo:", StringComparison.Ordinal))
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
