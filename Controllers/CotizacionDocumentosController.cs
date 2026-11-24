using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class CotizacionDocumentosController : Controller
    {
        private const long MaxBytes = 20_000_000;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;

        public CotizacionDocumentosController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(int cotizacionId, IFormFile archivo)
        {
            const string tipoDocumento = "CotizacionFirmada";

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == cotizacionId);
            if (cotizacion == null)
                return NotFound();

            if (cotizacion.EstadoCotizacion?.Nombre != "Evaluada")
            {
                TempData["ErrorMessage"] = "Solo se pueden adjuntar documentos a cotizaciones evaluadas.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            if (archivo == null || archivo.Length == 0)
            {
                TempData["ErrorMessage"] = "No se seleccionó ningún archivo.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            if (archivo.Length > MaxBytes)
            {
                TempData["ErrorMessage"] = "El archivo excede el tamaño máximo de 20 MB.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            if (!string.Equals(Path.GetExtension(archivo.FileName), ".pdf", StringComparison.OrdinalIgnoreCase) ||
                archivo.ContentType != "application/pdf")
            {
                TempData["ErrorMessage"] = "Solo se permiten archivos PDF.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            await using var lectura = archivo.OpenReadStream();
            var encabezado = new byte[5];
            var bytesLeidos = await lectura.ReadAsync(encabezado);
            await lectura.DisposeAsync();
            if (bytesLeidos < 5 || !encabezado.AsSpan(0, 5).SequenceEqual("%PDF-"u8))
            {
                TempData["ErrorMessage"] = "El archivo no contiene un PDF válido.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            string hash;
            await using (var hashStream = archivo.OpenReadStream())
            using (var sha = SHA256.Create())
            {
                var bytes = await sha.ComputeHashAsync(hashStream);
                hash = Convert.ToHexString(bytes).ToLowerInvariant();
            }

            var existente = await _context.DocumentosCotizacion
                .FirstOrDefaultAsync(d => d.CotizacionId == cotizacionId &&
                                          d.TipoDocumento == tipoDocumento &&
                                          d.EsReemplazable);
            if (existente != null)
            {
                EliminarArchivoFisico(existente.Ruta);
                _context.DocumentosCotizacion.Remove(existente);
            }

            var carpeta = Path.Combine(_environment.WebRootPath, "uploads", "cotizaciones", cotizacionId.ToString());
            Directory.CreateDirectory(carpeta);

            var nombreArchivo = $"{Guid.NewGuid():N}.pdf";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);
            await using (var salida = new FileStream(rutaFisica, FileMode.CreateNew))
                await archivo.CopyToAsync(salida);

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null)
                return Challenge();

            _context.DocumentosCotizacion.Add(new DocumentoCotizacion
            {
                CotizacionId = cotizacionId,
                NombreOriginal = Path.GetFileName(archivo.FileName),
                Ruta = Path.Combine("uploads", "cotizaciones", cotizacionId.ToString(), nombreArchivo),
                TipoContenido = "application/pdf",
                TipoDocumento = tipoDocumento,
                HashSha256 = hash,
                TamanoBytes = archivo.Length,
                FechaCarga = DateTime.Now,
                CargadoPorUsuarioId = usuario.Id,
                EsReemplazable = true
            });

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Documento PDF adjuntado correctamente.";
            return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
        }

        [HttpGet]
        public async Task<IActionResult> Descargar(int id)
        {
            var documento = await _context.DocumentosCotizacion
                .AsNoTracking()
                .Include(d => d.Cotizacion)
                .FirstOrDefaultAsync(d => d.IdDocumento == id);
            if (documento == null)
                return NotFound();

            var ruta = ResolveerRuta(documento.Ruta);
            if (!System.IO.File.Exists(ruta))
                return NotFound();

            return PhysicalFile(ruta, "application/pdf", documento.NombreOriginal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, int cotizacionId)
        {
            var documento = await _context.DocumentosCotizacion
                .FirstOrDefaultAsync(d => d.IdDocumento == id && d.CotizacionId == cotizacionId);
            if (documento == null)
                return NotFound();

            if (!documento.EsReemplazable)
            {
                TempData["ErrorMessage"] = "Los documentos contractuales aprobados no pueden eliminarse.";
                return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
            }

            EliminarArchivoFisico(documento.Ruta);
            _context.DocumentosCotizacion.Remove(documento);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Documento eliminado.";
            return RedirectToAction("Detalle", "Cotizaciones", new { id = cotizacionId });
        }

        private string ResolveerRuta(string rutaRelativa)
        {
            return Path.Combine(_environment.WebRootPath,
                rutaRelativa.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
        }

        private void EliminarArchivoFisico(string rutaRelativa)
        {
            var ruta = ResolveerRuta(rutaRelativa);
            if (System.IO.File.Exists(ruta))
                System.IO.File.Delete(ruta);
        }
    }
}
