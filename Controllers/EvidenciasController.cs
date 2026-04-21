using Microsoft.AspNetCore.Mvc;

public class EvidenciasController : Controller
{
    private readonly string ruta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/evidencias");

    public IActionResult Index()
    {
        if (!Directory.Exists(ruta))
            Directory.CreateDirectory(ruta);

        var archivos = Directory.GetFiles(ruta)
                        .Select(Path.GetFileName)
                        .ToList();

        return View(archivos);
    }

    [HttpPost]
    public async Task<IActionResult> Subir(IFormFile archivo, string descripcion)
    {
        if (archivo != null && archivo.Length > 0)
        {
            var path = Path.Combine(ruta, archivo.FileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Eliminar(string nombreArchivo)
    {
        var path = Path.Combine(ruta, nombreArchivo);

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
        }

        return RedirectToAction("Index");
    }
}