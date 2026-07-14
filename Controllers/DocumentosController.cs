using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Administrador")]
    public class DocumentosController : Controller
    {
        private const long MaxBytes = 20_000_000; // 20 MB
        private static readonly HashSet<string> TiposPermitidos = new(StringComparer.OrdinalIgnoreCase)
        {
            "application/pdf",
            "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.ms-excel",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "text/plain",
        };

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<IdentityUser> _userManager;

        public DocumentosController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subir(int ordenId, IFormFile archivo, string tipoDocumento, string? descripcion)
        {
            var orden = await _context.OrdenesServicio.AsNoTracking().FirstOrDefaultAsync(o => o.IdOrden == ordenId);
            if (orden == null) return NotFound();

            if (archivo == null || archivo.Length == 0)
            {
                TempData["ErrorMessage"] = "No se seleccionó ningún archivo.";
                return RedirectToAction("Detalle", "Tecnicos", new { id = ordenId });
            }

            if (archivo.Length > MaxBytes)
            {
                TempData["ErrorMessage"] = "El archivo excede el tamaño máximo de 20 MB.";
                return RedirectToAction("Detalle", "Tecnicos", new { id = ordenId });
            }

            if (!TiposPermitidos.Contains(archivo.ContentType))
            {
                TempData["ErrorMessage"] = "Tipo de archivo no permitido. Solo PDF, Word, Excel y TXT.";
                return RedirectToAction("Detalle", "Tecnicos", new { id = ordenId });
            }

            var carpeta = Path.Combine(_env.WebRootPath, "uploads", "documentos", ordenId.ToString());
            Directory.CreateDirectory(carpeta);

            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"{Guid.NewGuid()}{extension}";
            var rutaFisica = Path.Combine(carpeta, nombreArchivo);

            using (var fs = new FileStream(rutaFisica, FileMode.Create))
                await archivo.CopyToAsync(fs);

            var usuario = await _userManager.GetUserAsync(User);

            _context.DocumentosOrdenServicio.Add(new DocumentoOrdenServicio
            {
                OrdenId = ordenId,
                NombreOriginal = Path.GetFileName(archivo.FileName),
                Ruta = Path.Combine("uploads", "documentos", ordenId.ToString(), nombreArchivo),
                TipoContenido = archivo.ContentType,
                TipoDocumento = tipoDocumento ?? "Otro",
                Descripcion = descripcion?.Trim(),
                FechaCarga = DateTime.Now,
                CargadoPorUsuarioId = usuario?.Id,
            });

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Documento adjuntado correctamente.";
            return RedirectToAction("Detalle", "Tecnicos", new { id = ordenId });
        }

        [HttpGet]
        public async Task<IActionResult> Descargar(int id)
        {
            var doc = await _context.DocumentosOrdenServicio.AsNoTracking().FirstOrDefaultAsync(d => d.IdDocumento == id);
            if (doc == null) return NotFound();

            var rutaFisica = Path.Combine(_env.WebRootPath, doc.Ruta.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
            if (!System.IO.File.Exists(rutaFisica)) return NotFound();

            return PhysicalFile(rutaFisica, doc.TipoContenido, doc.NombreOriginal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id, int ordenId)
        {
            var doc = await _context.DocumentosOrdenServicio.FirstOrDefaultAsync(d => d.IdDocumento == id);
            if (doc == null || doc.OrdenId != ordenId) return NotFound();

            var rutaFisica = Path.Combine(_env.WebRootPath, doc.Ruta.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(rutaFisica))
                System.IO.File.Delete(rutaFisica);

            _context.DocumentosOrdenServicio.Remove(doc);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Documento eliminado.";
            return RedirectToAction("Detalle", "Tecnicos", new { id = ordenId });
        }
    }
}
