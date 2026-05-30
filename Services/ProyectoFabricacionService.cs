using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MultiservicioB.Services
{
    public class ProyectoFabricacionService : IProyectoFabricacionService
    {
        private readonly ApplicationDbContext _context;

        public ProyectoFabricacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProyectoFabricacionDTO>> GetAllAsync()
        {
            return await _context.ProyectosFabricacion
                .Include(p => p.Cliente)
                .Select(p => new ProyectoFabricacionDTO
                {
                    IdProyecto = p.IdProyecto,
                    ClienteId = p.ClienteId,
                    NombreCliente = p.Cliente != null ? p.Cliente.Nombre : null,
                    Descripcion = p.Descripcion,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Estado = p.Estado
                })
                .ToListAsync();
        }

        public async Task<ProyectoFabricacionDTO?> GetByIdAsync(int id)
        {
            var proyecto = await _context.ProyectosFabricacion
                .Include(p => p.Cliente)
                .FirstOrDefaultAsync(p => p.IdProyecto == id);

            if (proyecto == null) return null;

            return new ProyectoFabricacionDTO
            {
                IdProyecto = proyecto.IdProyecto,
                ClienteId = proyecto.ClienteId,
                NombreCliente = proyecto.Cliente != null ? proyecto.Cliente.Nombre : null,
                Descripcion = proyecto.Descripcion,
                FechaInicio = proyecto.FechaInicio,
                FechaFin = proyecto.FechaFin,
                Estado = proyecto.Estado
            };
        }

        public async Task<IEnumerable<ProyectoFabricacionDTO>> GetByClienteAsync(int clienteId)
        {
            return await _context.ProyectosFabricacion
                .Include(p => p.Cliente)
                .Where(p => p.ClienteId == clienteId)
                .Select(p => new ProyectoFabricacionDTO
                {
                    IdProyecto = p.IdProyecto,
                    ClienteId = p.ClienteId,
                    NombreCliente = p.Cliente != null ? p.Cliente.Nombre : null,
                    Descripcion = p.Descripcion,
                    FechaInicio = p.FechaInicio,
                    FechaFin = p.FechaFin,
                    Estado = p.Estado
                })
                .ToListAsync();
        }

        public async Task<ProyectoFabricacionDTO> CreateAsync(ProyectoFabricacionDTO proyectoDto)
        {
            var proyecto = new ProyectoFabricacion
            {
                ClienteId = proyectoDto.ClienteId,
                Descripcion = proyectoDto.Descripcion,
                FechaInicio = proyectoDto.FechaInicio,
                FechaFin = proyectoDto.FechaFin,
                Estado = proyectoDto.Estado ?? "Pendiente"
            };

            _context.ProyectosFabricacion.Add(proyecto);
            await _context.SaveChangesAsync();

            proyectoDto.IdProyecto = proyecto.IdProyecto;
            return proyectoDto;
        }

        public async Task<bool> UpdateAsync(ProyectoFabricacionDTO proyectoDto)
        {
            var proyecto = await _context.ProyectosFabricacion.FindAsync(proyectoDto.IdProyecto);
            if (proyecto == null) return false;

            proyecto.ClienteId = proyectoDto.ClienteId;
            proyecto.Descripcion = proyectoDto.Descripcion;
            proyecto.FechaInicio = proyectoDto.FechaInicio;
            proyecto.FechaFin = proyectoDto.FechaFin;
            proyecto.Estado = proyectoDto.Estado;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var proyecto = await _context.ProyectosFabricacion.FindAsync(id);
            if (proyecto == null) return false;

            _context.ProyectosFabricacion.Remove(proyecto);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
