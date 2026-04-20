using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
	public class ConfiguracionController : Controller
	{
		public IActionResult Roles() => View();
		public IActionResult Servicios() => View();
		public IActionResult Estados() => View();
		public IActionResult Materiales() => View();
		public IActionResult Parametros() => View();
	}
}

