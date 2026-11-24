using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Services
{
    public class CotizacionAprobacionService : ICotizacionAprobacionService
    {
        private readonly ApplicationDbContext _context;

        public CotizacionAprobacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CotizacionAprobacionResult> AprobarAsync(int cotizacionId, string usuarioId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            var cotizacion = await _context.Cotizaciones
                .Include(c => c.EstadoCotizacion)
                .Include(c => c.Documentos)
                .FirstOrDefaultAsync(c => c.IdCotizacion == cotizacionId);

            if (cotizacion == null)
                return new CotizacionAprobacionResult(false, "La cotización no existe.");

            if (cotizacion.EstadoCotizacion?.Nombre == "Aprobada")
                return new CotizacionAprobacionResult(false, "La cotización ya fue aprobada.",
                    await _context.OrdenesServicio.Where(o => o.CotizacionId == cotizacionId).Select(o => (int?)o.IdOrden).FirstOrDefaultAsync());

            if (cotizacion.EstadoCotizacion?.Nombre != "Evaluada")
                return new CotizacionAprobacionResult(false, "Solo se pueden aprobar cotizaciones evaluadas.");

            var documentosPdf = cotizacion.Documentos
                .Where(d => d.TipoContenido == "application/pdf" &&
                            d.TipoDocumento == "CotizacionFirmada" &&
                            d.EsReemplazable)
                .ToList();

            if (documentosPdf.Count == 0)
                return new CotizacionAprobacionResult(false, "Debe subir al menos un documento PDF antes de aprobar la cotización.");

            var ordenExistente = await _context.OrdenesServicio
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.CotizacionId == cotizacionId);
            if (ordenExistente != null)
                return new CotizacionAprobacionResult(false, "La cotización ya tiene una orden de servicio.", ordenExistente.IdOrden);

            var estadoAprobada = await _context.EstadosCotizacion
                .SingleOrDefaultAsync(e => e.Nombre == "Aprobada");
            var estadoPendiente = await _context.EstadosOrden
                .SingleOrDefaultAsync(e => e.Nombre == "Pendiente");
            if (estadoAprobada == null || estadoPendiente == null)
                return new CotizacionAprobacionResult(false, "No están configurados los estados requeridos.");

            var ahora = DateTime.Now;
            cotizacion.EstadoCotizacionId = estadoAprobada.Id;
            cotizacion.AprobadaPorCliente = false;
            cotizacion.FechaAprobacion = ahora;
            cotizacion.AprobadaPorUsuarioId = usuarioId;
            cotizacion.MotivoRechazo = null;
            cotizacion.FechaRechazo = null;
            cotizacion.RechazadaPorUsuarioId = null;

            var orden = new OrdenServicio
            {
                CotizacionId = cotizacion.IdCotizacion,
                ClienteId = cotizacion.ClienteId,
                EmpleadoId = null,
                EstadoOrdenId = estadoPendiente.Id,
                FechaCreacion = ahora,
                FechaCompromiso = cotizacion.FechaVisitaSolicitada,
                CompromisoConfirmado = false,
                UsarDireccionPerfil = cotizacion.UsarDireccionPerfil,
                EnlaceWaze = cotizacion.EnlaceWaze
            };

            _context.OrdenesServicio.Add(orden);
            await _context.SaveChangesAsync();

            foreach (var documento in documentosPdf)
            {
                _context.DocumentosOrdenServicio.Add(new DocumentoOrdenServicio
                {
                    OrdenId = orden.IdOrden,
                    NombreOriginal = documento.NombreOriginal,
                    Ruta = documento.Ruta,
                    TipoContenido = documento.TipoContenido,
                    TipoDocumento = documento.TipoDocumento == "ComprobantePago" ? "ComprobantePago" : "CotizacionFirmada",
                    Descripcion = "Evidencia contractual asociada automáticamente a la cotización.",
                    FechaCarga = documento.FechaCarga,
                    CargadoPorUsuarioId = documento.CargadoPorUsuarioId,
                    DocumentoCotizacionId = documento.IdDocumento,
                    EsEvidenciaContractual = true
                });
                documento.EsReemplazable = false;
            }

            _context.Auditorias.Add(new Auditoria
            {
                UsuarioId = usuarioId,
                Accion = "AprobarCotizacion",
                Fecha = ahora,
                Detalle = $"Cotización #{cotizacion.IdCotizacion} aprobada; orden #{orden.IdOrden}; {documentosPdf.Count} documento(s) PDF"
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CotizacionAprobacionResult(true, OrdenId: orden.IdOrden);
        }

        public async Task<CotizacionAprobacionResult> RechazarAsync(int cotizacionId, string usuarioId, string motivo)
        {
            if (string.IsNullOrWhiteSpace(motivo))
                return new CotizacionAprobacionResult(false, "Debe indicar el motivo del rechazo.");

            await using var transaction = await _context.Database.BeginTransactionAsync();
            var cotizacion = await _context.Cotizaciones
                .Include(c => c.EstadoCotizacion)
                .FirstOrDefaultAsync(c => c.IdCotizacion == cotizacionId);

            if (cotizacion == null)
                return new CotizacionAprobacionResult(false, "La cotización no existe.");

            if (cotizacion.EstadoCotizacion?.Nombre == "Aprobada")
                return new CotizacionAprobacionResult(false, "No se puede rechazar una cotización aprobada.");

            if (cotizacion.EstadoCotizacion?.Nombre == "Rechazada")
                return new CotizacionAprobacionResult(false, "La cotización ya fue rechazada.");

            if (cotizacion.EstadoCotizacion?.Nombre != "Evaluada")
                return new CotizacionAprobacionResult(false, "Solo se pueden rechazar cotizaciones evaluadas.");

            var estadoRechazada = await _context.EstadosCotizacion
                .SingleOrDefaultAsync(e => e.Nombre == "Rechazada");
            if (estadoRechazada == null)
                return new CotizacionAprobacionResult(false, "No está configurado el estado Rechazada.");

            var ahora = DateTime.Now;
            cotizacion.EstadoCotizacionId = estadoRechazada.Id;
            cotizacion.AprobadaPorCliente = false;
            cotizacion.MotivoRechazo = motivo.Trim();
            cotizacion.FechaRechazo = ahora;
            cotizacion.RechazadaPorUsuarioId = usuarioId;

            _context.Auditorias.Add(new Auditoria
            {
                UsuarioId = usuarioId,
                Accion = "RechazarCotizacion",
                Fecha = ahora,
                Detalle = $"Cotización #{cotizacion.IdCotizacion} rechazada: {motivo.Trim()}".Substring(0,
                    Math.Min(255, $"Cotización #{cotizacion.IdCotizacion} rechazada: {motivo.Trim()}".Length))
            });

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new CotizacionAprobacionResult(true);
        }
    }
}
