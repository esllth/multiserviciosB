using Microsoft.AspNetCore.Mvc;

namespace MultiservicioB.Controllers
{
	public class ReportesController : Controller
	{
		public IActionResult Indicadores() => View();
		public IActionResult ReporteFechas() => View();
		public IActionResult DesempenoTecnicos() => View();
		public IActionResult Satisfaccion() => View();
		public IActionResult Exportar() => View();
	}
}
