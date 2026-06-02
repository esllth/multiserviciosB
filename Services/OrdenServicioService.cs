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
                .Include(o => o.Empleado)
                .Include(o => o.EstadoOrden)
                .Include(o => o.Cotizacion)
                .FirstOrDefaultAsync(o => o.IdOrden == id);

            if (orden == null) return null;

            return new OrdenServicioDTO
            {
                IdOrden = orden.IdOrden,
                CotizacionId = orden.CotizacionId,
                ClienteId = orden.ClienteId,
                NombreCliente = orden.Cliente != null ? orden.Cliente.Nombre : null,
                EmpleadoId = orden.EmpleadoId,
                NombreTecnico = orden.Empleado != null ? orden.Empleado.NombreEmpleado + " " + orden.Empleado.ApellidosEmpleado : null,
                FechaCreacion = orden.FechaCreacion,
                FechaInicio = orden.FechaInicio,
                FechaFin = orden.FechaFin,
                EstadoOrdenId = orden.EstadoOrdenId,
                NombreEstado = orden.EstadoOrden != null ? orden.EstadoOrden.Nombre : null,
                DescripcionServicio = orden.Cotizacion != null ? orden.Cotizacion.Descripcion : null
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
            orden.FechaInicio = ordenDto.FechaInicio;
            orden.FechaFin = ordenDto.FechaFin;
            orden.EstadoOrdenId = ordenDto.EstadoOrdenId;

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

        public async Task<bool> IniciarOrdenAsync(int id)
        {
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

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FinalizarOrdenAsync(int id)
        {
            var orden = await _context.OrdenesServicio.FindAsync(id);
            if (orden == null) return false;

            var estadoEnProgreso = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "En Progreso");
            if (estadoEnProgreso == null || orden.EstadoOrdenId != estadoEnProgreso.Id) return false;

            orden.FechaFin = DateTime.Now;
            var estadoCompletada = await _context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Completada");
            if (estadoCompletada != null)
            {
                orden.EstadoOrdenId = estadoCompletada.Id;
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
