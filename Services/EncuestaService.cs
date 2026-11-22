using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.ViewModels;

namespace MultiservicioB.Services
{
    public class EncuestaService : IEncuestaService
    {
        private readonly ApplicationDbContext _context;

        public EncuestaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EncuestaClienteViewModel>> ObtenerEncuestasClienteAsync(string correo)
        {
            var correoNormalizado = correo.Trim().ToLowerInvariant();
            return await _context.Encuestas
                .AsNoTracking()
                .Where(e => e.Cliente != null && e.Cliente.Correo != null &&
                    e.Cliente.Correo.ToLower() == correoNormalizado && e.Orden != null &&
                    e.Orden.EstadoOrden != null && e.Orden.EstadoOrden.Nombre == "Completada")
                .OrderByDescending(e => e.OrdenId)
                .Select(e => new EncuestaClienteViewModel
                {
                    OrdenId = e.OrdenId,
                    Servicio = e.Orden!.Cotizacion != null && e.Orden.Cotizacion.TipoServicio != null
                        ? e.Orden.Cotizacion.TipoServicio.Nombre
                        : "Servicio",
                    Tecnico = e.Orden!.Empleado != null
                        ? e.Orden.Empleado.NombreEmpleado + " " + e.Orden.Empleado.ApellidosEmpleado
                        : "Sin técnico asignado",
                    CalificacionServicio = e.CalificacionServicio,
                    CalificacionTecnico = e.CalificacionTecnico,
                    Comentarios = e.Comentarios,
                    Fecha = e.Fecha,
                    Respondida = e.CalificacionServicio != null && e.CalificacionTecnico != null
                })
                .ToListAsync();
        }

        public Task<bool> TieneEncuestasPendientesClienteAsync(string correo)
        {
            var correoNormalizado = correo.Trim().ToLowerInvariant();
            return _context.Encuestas.AnyAsync(e => e.Cliente != null && e.Cliente.Correo != null &&
                e.Cliente.Correo.ToLower() == correoNormalizado && e.CalificacionServicio == null &&
                e.Orden != null && e.Orden.EstadoOrden != null && e.Orden.EstadoOrden.Nombre == "Completada");
        }

        public async Task<bool> GuardarRespuestaClienteAsync(string correo, ResponderEncuestaViewModel respuesta)
        {
            var correoNormalizado = correo.Trim().ToLowerInvariant();
            var encuesta = await _context.Encuestas
                .Include(e => e.Cliente)
                .Include(e => e.Orden)
                    .ThenInclude(o => o!.EstadoOrden)
                .FirstOrDefaultAsync(e => e.OrdenId == respuesta.OrdenId &&
                    e.Cliente != null && e.Cliente.Correo != null &&
                    e.Cliente.Correo.ToLower() == correoNormalizado &&
                    e.CalificacionServicio == null && e.Orden != null &&
                    e.Orden.EstadoOrden != null && e.Orden.EstadoOrden.Nombre == "Completada");

            if (encuesta == null) return false;

            encuesta.CalificacionServicio = respuesta.CalificacionServicio;
            encuesta.CalificacionTecnico = respuesta.CalificacionTecnico;
            encuesta.Comentarios = respuesta.Comentarios?.Trim();
            encuesta.Fecha = DateTime.Today;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
