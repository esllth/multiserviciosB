using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;

namespace MultiservicioB.Controllers
{
    [Authorize(Roles = "Empleado,Administrador")]
    public class GeolocalizacionController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GeolocalizacionController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> Buscar(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
                return Json(new List<object>());

            var termino = q.Trim().ToLower();

            var clientes = await _context.Clientes
                .AsNoTracking()
                .Include(c => c.Direccion)
                    .ThenInclude(d => d!.UbicacionDTA)
                .Where(c => c.Estado == "Activo" && c.Direccion != null &&
                    (c.Nombre.ToLower().Contains(termino) ||
                     (c.Apellidos != null && c.Apellidos.ToLower().Contains(termino)) ||
                     (c.NombreNegocio != null && c.NombreNegocio.ToLower().Contains(termino)) ||
                     c.Identificacion.Contains(termino)))
                .Select(c => new
                {
                    tipo = "Cliente",
                    id = c.IdCliente,
                    nombre = c.Nombre + (c.Apellidos != null ? " " + c.Apellidos : ""),
                    subtitulo = c.NombreNegocio,
                    telefono = c.Telefono,
                    correo = c.Correo,
                    provincia = c.Direccion!.UbicacionDTA != null ? c.Direccion.UbicacionDTA.Provincia : null,
                    canton = c.Direccion!.UbicacionDTA != null ? c.Direccion.UbicacionDTA.Canton : null,
                    distrito = c.Direccion!.UbicacionDTA != null ? c.Direccion.UbicacionDTA.Distrito : null,
                    otrasSenas = c.Direccion!.OtrasSenas,
                    ordenes = _context.OrdenesServicio
                        .Where(o => o.ClienteId == c.IdCliente)
                        .OrderByDescending(o => o.FechaCreacion)
                        .Take(3)
                        .Select(o => new
                        {
                            id = o.IdOrden,
                            fecha = o.FechaCreacion,
                            estado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : "—",
                            tecnico = o.Empleado != null ? o.Empleado.NombreEmpleado + " " + o.Empleado.ApellidosEmpleado : null
                        }).ToList()
                })
                .ToListAsync();

            var empleados = await (
                from e in _context.Empleados.AsNoTracking()
                join d in _context.Direcciones.AsNoTracking() on e.DireccionId equals d.Id into dj
                from d in dj.DefaultIfEmpty()
                join dta in _context.UbicacionDTA.AsNoTracking() on d.UbicacionDTAId equals dta.Id into dtaj
                from dta in dtaj.DefaultIfEmpty()
                where e.EstadoEmpleado && e.DireccionId != null &&
                      (e.NombreEmpleado.ToLower().Contains(termino) ||
                       e.ApellidosEmpleado.ToLower().Contains(termino) ||
                       e.IdentificacionEmpleado.Contains(termino))
                select new
                {
                    tipo = "Empleado",
                    id = e.IdEmpleado,
                    nombre = e.NombreEmpleado + " " + e.ApellidosEmpleado,
                    subtitulo = (string?)null,
                    telefono = e.TelefonoEmpleado,
                    correo = e.CorreoElectronicoEmpleado,
                    provincia = dta != null ? dta.Provincia : null,
                    canton = dta != null ? dta.Canton : null,
                    distrito = dta != null ? dta.Distrito : null,
                    otrasSenas = d != null ? d.OtrasSenas : null,
                    ordenes = _context.OrdenesServicio
                        .Where(o => o.EmpleadoId == e.IdEmpleado)
                        .OrderByDescending(o => o.FechaCreacion)
                        .Take(3)
                        .Select(o => new
                        {
                            id = o.IdOrden,
                            fecha = o.FechaCreacion,
                            estado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : "—",
                            tecnico = (string?)null
                        }).ToList()
                }).ToListAsync();

            var resultados = clientes.Cast<object>().Concat(empleados.Cast<object>()).ToList();
            return Json(resultados);
        }
    }
}
