using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Services
{
    public class OrdenServicioService : IOrdenServicioService
    {
        private readonly ApplicationDbContext _context;

        public OrdenServicioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrdenServicioDTO>> GetAllAsync()
        {
            return await _context.OrdenesServicio
                .Include(o => o.Cliente)
                .Include(o => o.Empleado)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Cotizacion)
                .Select(o => new OrdenServicioDTO
                {
                    IdOrden = o.IdOrden,
                    CotizacionId = o.CotizacionId,
                    ClienteId = o.ClienteId,
                    NombreCliente = o.Cliente != null ? o.Cliente.Nombre : null,
                    EmpleadoId = o.EmpleadoId,
                    NombreTecnico = o.Empleado != null ? o.Empleado.NombreEmpleado + " " + o.Empleado.ApellidosEmpleado : null,
                    FechaCreacion = o.FechaCreacion,
                    FechaCompromiso = o.FechaCompromiso,
                    CompromisoConfirmado = o.CompromisoConfirmado,
                    UsarDireccionPerfil = o.UsarDireccionPerfil,
                    EnlaceWaze = o.EnlaceWaze,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    EstadoOrdenId = o.EstadoOrdenId,
                    NombreEstado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : null,
                    DescripcionServicio = o.Cotizacion != null ? o.Cotizacion.Descripcion : null
                })
                .ToListAsync();
        }

        public async Task<OrdenServicioDTO?> GetByIdAsync(int id)
        {
            var orden = await _context.OrdenesServicio
                .Include(o => o.Cliente)
                    .ThenInclude(c => c!.Direccion)
                    .ThenInclude(d => d!.UbicacionDTA)
                .Include(o => o.Empleado)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Cotizacion)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null) return null;

            var fotos = await _context.FotosOrdenServicio
                .Where(f => f.OrdenId == id)
                .OrderByDescending(f => f.FechaCarga)
                .Select(f => new FotoOrdenServicioDTO
                {
                    IdFotoOrden = f.IdFotoOrden,
                    OrdenId = f.OrdenId,
                    TipoFoto = f.TipoFoto,
                    Descripcion = f.Descripcion,
                    Ruta = f.Ruta,
                    NombreOriginal = f.NombreOriginal,
                    TipoContenido = f.TipoContenido,
                    FechaCarga = f.FechaCarga
                })
                .ToListAsync();

            var tieneFotosInicio = fotos.Any(f => f.TipoFoto == "Inicial");
            var tieneFotosFin = fotos.Any(f => f.TipoFoto == "Final");

            return new OrdenServicioDTO
            {
                IdOrden = orden.IdOrden,
                CotizacionId = orden.CotizacionId,
                ClienteId = orden.ClienteId,
                NombreCliente = orden.Cliente != null ? orden.Cliente.Nombre : null,
                EmpleadoId = orden.EmpleadoId,
                NombreTecnico = orden.Empleado != null ? orden.Empleado.NombreEmpleado + " " + orden.Empleado.ApellidosEmpleado : null,
                FechaCreacion = orden.FechaCreacion,
                FechaCompromiso = orden.FechaCompromiso,
                CompromisoConfirmado = orden.CompromisoConfirmado,
                UsarDireccionPerfil = orden.UsarDireccionPerfil,
                DireccionServicio = orden.UsarDireccionPerfil ? FormatearDireccion(orden.Cliente?.Direccion) : null,
                GoogleMapsUrl = orden.UsarDireccionPerfil ? CrearUrlGoogleMaps(orden.Cliente?.Direccion) : null,
                EnlaceWaze = orden.EnlaceWaze,
                FechaInicio = orden.FechaInicio,
                FechaFin = orden.FechaFin,
                EstadoOrdenId = orden.EstadoOrdenId,
                NombreEstado = orden.EstadoOrden != null ? orden.EstadoOrden.Nombre : null,
                DescripcionServicio = orden.Cotizacion != null ? orden.Cotizacion.Descripcion : null,
                MontoPresupuesto = orden.Cotizacion?.MontoPresupuesto,
                RequiereAdelanto = orden.Cotizacion?.RequiereAdelanto ?? false,
                PorcentajeAdelanto = orden.Cotizacion?.PorcentajeAdelanto,
                FormaPagoAceptada = orden.Cotizacion?.FormaPagoAceptada,
                ObservacionesTecnicas = orden.ObservacionesTecnicas,
                ComentariosFinales = orden.ComentariosFinales,
                RequiereFotosObligatorias = orden.RequiereFotosObligatorias,
                LlegadaConfirmada = orden.LlegadaConfirmada,
                TieneFotosInicio = tieneFotosInicio,
                TieneFotosFin = tieneFotosFin,
                PuedeFinalizarse = !orden.RequiereFotosObligatorias || (tieneFotosInicio && tieneFotosFin),
                Fotos = fotos
            };
        }

        public async Task<IEnumerable<OrdenServicioDTO>> GetByTecnicoAsync(int empleadoId)
        {
            return await _context.OrdenesServicio
                .Include(o => o.Cliente)
                .Include(o => o.Empleado)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Cotizacion)
                .Where(o => o.EmpleadoId == empleadoId)
                .Select(o => new OrdenServicioDTO
                {
                    IdOrden = o.IdOrden,
                    CotizacionId = o.CotizacionId,
                    ClienteId = o.ClienteId,
                    NombreCliente = o.Cliente != null ? o.Cliente.Nombre : null,
                    EmpleadoId = o.EmpleadoId,
                    NombreTecnico = o.Empleado != null ? o.Empleado.NombreEmpleado + " " + o.Empleado.ApellidosEmpleado : null,
                    FechaCreacion = o.FechaCreacion,
                    FechaCompromiso = o.FechaCompromiso,
                    CompromisoConfirmado = o.CompromisoConfirmado,
                    UsarDireccionPerfil = o.UsarDireccionPerfil,
                    EnlaceWaze = o.EnlaceWaze,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    EstadoOrdenId = o.EstadoOrdenId,
                    NombreEstado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : null,
                    DescripcionServicio = o.Cotizacion != null ? o.Cotizacion.Descripcion : null
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<OrdenServicioDTO>> GetByClienteAsync(int clienteId)
        {
            return await _context.OrdenesServicio
                .Include(o => o.Cliente)
                .Include(o => o.Empleado)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Cotizacion)
                .Where(o => o.ClienteId == clienteId)
                .Select(o => new OrdenServicioDTO
                {
                    IdOrden = o.IdOrden,
                    CotizacionId = o.CotizacionId,
                    ClienteId = o.ClienteId,
                    NombreCliente = o.Cliente != null ? o.Cliente.Nombre : null,
                    EmpleadoId = o.EmpleadoId,
                    NombreTecnico = o.Empleado != null ? o.Empleado.NombreEmpleado + " " + o.Empleado.ApellidosEmpleado : null,
                    FechaCreacion = o.FechaCreacion,
                    FechaCompromiso = o.FechaCompromiso,
                    CompromisoConfirmado = o.CompromisoConfirmado,
                    UsarDireccionPerfil = o.UsarDireccionPerfil,
                    EnlaceWaze = o.EnlaceWaze,
                    FechaInicio = o.FechaInicio,
                    FechaFin = o.FechaFin,
                    EstadoOrdenId = o.EstadoOrdenId,
                    NombreEstado = o.EstadoOrden != null ? o.EstadoOrden.Nombre : null,
                    DescripcionServicio = o.Cotizacion != null ? o.Cotizacion.Descripcion : null
                })
                .ToListAsync();
        }

        public async Task<OrdenServicioDTO> CreateAsync(OrdenServicioDTO ordenDto)
        {
            var orden = new OrdenServicio
            {
                CotizacionId = ordenDto.CotizacionId,
                ClienteId = ordenDto.ClienteId,
                EmpleadoId = ordenDto.EmpleadoId,
                FechaCreacion = DateTime.Now,
                FechaCompromiso = ordenDto.FechaCompromiso,
                CompromisoConfirmado = ordenDto.CompromisoConfirmado,
                UsarDireccionPerfil = ordenDto.UsarDireccionPerfil,
                EnlaceWaze = ordenDto.EnlaceWaze,
                FechaInicio = ordenDto.FechaInicio,
                FechaFin = ordenDto.FechaFin,
                EstadoOrdenId = ordenDto.EstadoOrdenId
            };

            _context.OrdenesServicio.Add(orden);
            await _context.SaveChangesAsync();

            ordenDto.IdOrden = orden.IdOrden;
            return ordenDto;
        }

        public async Task<bool> UpdateAsync(OrdenServicioDTO ordenDto)
        {
            var orden = await _context.OrdenesServicio.FindAsync(ordenDto.IdOrden);
            if (orden == null) return false;

            orden.CotizacionId = ordenDto.CotizacionId;
            orden.ClienteId = ordenDto.ClienteId;
            orden.EmpleadoId = ordenDto.EmpleadoId;
            orden.FechaCompromiso = ordenDto.FechaCompromiso;
            orden.CompromisoConfirmado = ordenDto.CompromisoConfirmado;
            orden.UsarDireccionPerfil = ordenDto.UsarDireccionPerfil;
            orden.EnlaceWaze = ordenDto.EnlaceWaze;
            orden.FechaLlegadaSitio = ordenDto.FechaLlegadaSitio;
            orden.FechaInicio = ordenDto.FechaInicio;
            orden.FechaFin = ordenDto.FechaFin;
            orden.FechaAceptacionCliente = ordenDto.FechaAceptacionCliente;
            orden.EstadoOrdenId = ordenDto.EstadoOrdenId;
            orden.ObservacionesTecnicas = ordenDto.ObservacionesTecnicas;
            orden.ComentariosFinales = ordenDto.ComentariosFinales;
            orden.RequiereFotosObligatorias = ordenDto.RequiereFotosObligatorias;
            orden.LlegadaConfirmada = ordenDto.LlegadaConfirmada;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            _context.OrdenesServicio.Remove(orden);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmarLlegadaSitioAsync(int id, decimal latitud, decimal longitud)
        {
            await AsegurarTablaEventoOrdenAsync();

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            orden.FechaLlegadaSitio = DateTime.Now;
            orden.LlegadaConfirmada = true;

            // Registrar evento de llegada
            var evento = new EventoOrdenServicio
            {
                OrdenId = id,
                TipoEvento = "LlegadaSitio",
                FechaEvento = DateTime.Now,
                Latitud = latitud,
                Longitud = longitud,
                Descripcion = "Técnico confirmó llegada al sitio"
            };
            _context.EventosOrdenServicio.Add(evento);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IniciarOrdenAsync(int id)
        {
            await AsegurarTablaEventoOrdenAsync();

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null || !orden.EmpleadoId.HasValue) return false;

            var estadoPendiente = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Pendiente");
            if (estadoPendiente == null || orden.EstadoOrdenId != estadoPendiente.Id) return false;

            orden.FechaInicio = DateTime.Now;
            var estadoEnProgreso = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "En Progreso");
            if (estadoEnProgreso != null)
            {
                orden.EstadoOrdenId = estadoEnProgreso.Id;
            }

            // Registrar evento de inicio
            var evento = new EventoOrdenServicio
            {
                OrdenId = id,
                TipoEvento = "InicioServicio",
                FechaEvento = DateTime.Now,
                Descripcion = "Servicio iniciado"
            };
            _context.EventosOrdenServicio.Add(evento);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FinalizarOrdenAsync(int id, string comentariosFinales)
        {
            await AsegurarTablaEventoOrdenAsync();

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            // Validar que se puede finalizar
            if (!await ValidarPuedeFinalizarAsync(id))
            {
                return false;
            }

            var estadoEnProgreso = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "En Progreso");
            if (estadoEnProgreso == null || orden.EstadoOrdenId != estadoEnProgreso.Id) return false;

            if (await ObtenerErrorStockMaterialesAsync(id) != null) return false;

            var consumos = await _context.ConsumosMaterial
                .Include(c => c.Material)
                .Where(c => c.OrdenId == id)
                .ToListAsync();
            foreach (var consumo in consumos)
            {
                if (consumo.Material != null)
                {
                    consumo.Material.StockActual = (consumo.Material.StockActual ?? 0) - (int)(consumo.CantidadUsada ?? 0);
                }
            }

            orden.FechaFin = DateTime.Now;
            orden.ComentariosFinales = comentariosFinales;

            var estadoCompletada = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Completada");
            if (estadoCompletada != null)
            {
                orden.EstadoOrdenId = estadoCompletada.Id;
            }

            // Registrar evento de finalización
            var evento = new EventoOrdenServicio
            {
                OrdenId = id,
                TipoEvento = "FinalizacionServicio",
                FechaEvento = DateTime.Now,
                Descripcion = comentariosFinales
            };
            _context.EventosOrdenServicio.Add(evento);

            await _context.SaveChangesAsync();

            // TODO: Enviar notificación al cliente

            return true;
        }

        public async Task<string?> ObtenerErrorStockMaterialesAsync(int id)
        {
            var consumos = await _context.ConsumosMaterial
                .AsNoTracking()
                .Include(c => c.Material)
                .Where(c => c.OrdenId == id)
                .ToListAsync();

            foreach (var consumo in consumos)
            {
                var requerido = (int)(consumo.CantidadUsada ?? 0);
                var disponible = consumo.Material?.StockActual ?? 0;
                if (disponible < requerido)
                {
                    return $"Stock insuficiente para {consumo.Material?.Nombre}: se requieren {requerido} y hay {disponible}.";
                }
            }

            return null;
        }

        public async Task<bool> AceptarFinalizacionClienteAsync(int id)
        {
            await AsegurarTablaEventoOrdenAsync();

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            var estadoCompletada = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Completada");
            if (estadoCompletada == null || orden.EstadoOrdenId != estadoCompletada.Id) return false;

            orden.FechaAceptacionCliente = DateTime.Now;

            // Registrar evento de aceptación
            var evento = new EventoOrdenServicio
            {
                OrdenId = id,
                TipoEvento = "AceptacionCliente",
                FechaEvento = DateTime.Now,
                Descripcion = "Cliente aceptó la finalización del servicio"
            };
            _context.EventosOrdenServicio.Add(evento);

            await _context.SaveChangesAsync();

            // TODO: Activar encuesta de satisfacción

            return true;
        }

        public async Task<bool> ActualizarObservacionesTecnicasAsync(int id, string observaciones)
        {
            await AsegurarTablaEventoOrdenAsync();

            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            orden.ObservacionesTecnicas = observaciones;

            // Registrar evento de observación
            var evento = new EventoOrdenServicio
            {
                OrdenId = id,
                TipoEvento = "ObservacionTecnica",
                FechaEvento = DateTime.Now,
                Descripcion = observaciones
            };
            _context.EventosOrdenServicio.Add(evento);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidarPuedeFinalizarAsync(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            // Si requiere fotos obligatorias, verificar que existan
            if (orden.RequiereFotosObligatorias)
            {
                var tieneFotosInicio = await _context.FotosOrdenServicio
                    .AnyAsync(f => f.OrdenId == id && f.TipoFoto == "Inicial");

                var tieneFotosFin = await _context.FotosOrdenServicio
                    .AnyAsync(f => f.OrdenId == id && f.TipoFoto == "Final");

                if (!tieneFotosInicio || !tieneFotosFin)
                {
                    return false;
                }
            }

            return true;
        }

        private static string? FormatearDireccion(Direccion? direccion)
        {
            if (direccion?.UbicacionDTA == null)
            {
                return null;
            }

            var ubicacion = direccion.UbicacionDTA;
            var partes = new List<string?>
            {
                direccion.OtrasSenas,
                ubicacion.Distrito,
                ubicacion.Canton,
                ubicacion.Provincia,
                "Costa Rica",
                $"DTA {ubicacion.CodigoDTA}"
            };

            return string.Join(", ", partes.Where(p => !string.IsNullOrWhiteSpace(p)));
        }

        private static string? CrearUrlGoogleMaps(Direccion? direccion)
        {
            var direccionTexto = FormatearDireccion(direccion);
            return string.IsNullOrWhiteSpace(direccionTexto)
                ? null
                : $"https://www.google.com/maps/search/?api=1&query={Uri.EscapeDataString(direccionTexto)}";
        }

        private async Task AsegurarTablaEventoOrdenAsync()
        {
            await _context.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'dbo.EventoOrdenServicio', N'U') IS NULL
                BEGIN
                    IF OBJECT_ID(N'dbo.EventosOrdenServicio', N'U') IS NOT NULL
                    BEGIN
                        EXEC sp_rename N'dbo.EventosOrdenServicio', N'EventoOrdenServicio';
                    END
                    ELSE
                    BEGIN
                        CREATE TABLE [dbo].[EventoOrdenServicio] (
                            [IdEvento]    INT             IDENTITY (1, 1) NOT NULL,
                            [OrdenId]     INT             NOT NULL,
                            [TipoEvento]  NVARCHAR (50)   NOT NULL,
                            [FechaEvento] DATETIME        CONSTRAINT [DF_EventoOrdenServicio_FechaEvento] DEFAULT (GETDATE()) NOT NULL,
                            [Descripcion] NVARCHAR (1000) NULL,
                            [Latitud]     DECIMAL (10, 7) NULL,
                            [Longitud]    DECIMAL (10, 7) NULL,
                            [UsuarioId]   NVARCHAR (450)  NULL,
                            CONSTRAINT [PK_EventoOrdenServicio] PRIMARY KEY CLUSTERED ([IdEvento] ASC),
                            CONSTRAINT [CK_EventoOrdenServicio_TipoEvento] CHECK (
                                [TipoEvento] = N'AceptacionCliente' OR
                                [TipoEvento] = N'ComentarioFinal' OR
                                [TipoEvento] = N'FinalizacionServicio' OR
                                [TipoEvento] = N'ObservacionTecnica' OR
                                [TipoEvento] = N'InicioServicio' OR
                                [TipoEvento] = N'LlegadaSitio'
                            )
                        );
                    END
                END

                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = N'IX_EventoOrdenServicio_OrdenId'
                      AND object_id = OBJECT_ID(N'dbo.EventoOrdenServicio')
                )
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_EventoOrdenServicio_OrdenId]
                        ON [dbo].[EventoOrdenServicio]([OrdenId] ASC);
                END
                """);
        }

        public async Task<int> CalcularTiempoEfectivoAsync(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null || !orden.FechaInicio.HasValue) return 0;

            var fechaFin = orden.FechaFin ?? DateTime.Now;
            var tiempoTranscurrido = fechaFin - orden.FechaInicio.Value;

            return (int)tiempoTranscurrido.TotalMinutes;
        }
    }
}
