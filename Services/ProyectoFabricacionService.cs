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
                    NombreProyecto = p.NombreProyecto,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado,
                    FechaSolicitud = p.FechaSolicitud,
                    FechaInicioEstimada = p.FechaInicioEstimada,
                    FechaFinEstimada = p.FechaFinEstimada,
                    FechaInicioReal = p.FechaInicioReal,
                    FechaFinReal = p.FechaFinReal,
                    CostoEstimado = p.CostoEstimado,
                    CostoReal = p.CostoReal,
                    DiseñoAprobado = p.DiseñoAprobado,
                    FechaAprobacionDiseño = p.FechaAprobacionDiseño
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
                NombreProyecto = proyecto.NombreProyecto,
                Descripcion = proyecto.Descripcion,
                Estado = proyecto.Estado,
                FechaSolicitud = proyecto.FechaSolicitud,
                FechaInicioEstimada = proyecto.FechaInicioEstimada,
                FechaFinEstimada = proyecto.FechaFinEstimada,
                FechaInicioReal = proyecto.FechaInicioReal,
                FechaFinReal = proyecto.FechaFinReal,
                CostoEstimado = proyecto.CostoEstimado,
                CostoReal = proyecto.CostoReal,
                DiseñoAprobado = proyecto.DiseñoAprobado,
                FechaAprobacionDiseño = proyecto.FechaAprobacionDiseño,
                ObservacionesCliente = proyecto.ObservacionesCliente,
                ObservacionesInternas = proyecto.ObservacionesInternas
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
                    NombreProyecto = p.NombreProyecto,
                    Descripcion = p.Descripcion,
                    Estado = p.Estado,
                    FechaSolicitud = p.FechaSolicitud,
                    FechaInicioEstimada = p.FechaInicioEstimada,
                    FechaFinEstimada = p.FechaFinEstimada,
                    FechaInicioReal = p.FechaInicioReal,
                    FechaFinReal = p.FechaFinReal,
                    CostoEstimado = p.CostoEstimado,
                    CostoReal = p.CostoReal,
                    DiseñoAprobado = p.DiseñoAprobado
                })
                .ToListAsync();
        }

        public async Task<ProyectoFabricacionDTO> CreateAsync(ProyectoFabricacionDTO proyectoDto)
        {
            var proyecto = new ProyectoFabricacion
            {
                ClienteId = proyectoDto.ClienteId,
                NombreProyecto = proyectoDto.NombreProyecto,
                Descripcion = proyectoDto.Descripcion,
                Estado = proyectoDto.Estado ?? "Pendiente",
                FechaSolicitud = DateTime.Now,
                FechaInicioEstimada = proyectoDto.FechaInicioEstimada,
                FechaFinEstimada = proyectoDto.FechaFinEstimada,
                CostoEstimado = proyectoDto.CostoEstimado
            };

            _context.ProyectosFabricacion.Add(proyecto);
            await _context.SaveChangesAsync();

            proyectoDto.IdProyecto = proyecto.IdProyecto;
            proyectoDto.FechaSolicitud = proyecto.FechaSolicitud;
            return proyectoDto;
        }

        public async Task<bool> UpdateAsync(ProyectoFabricacionDTO proyectoDto)
        {
            var proyecto = await _context.ProyectosFabricacion.FindAsync(proyectoDto.IdProyecto);
            if (proyecto == null) return false;

            proyecto.ClienteId = proyectoDto.ClienteId;
            proyecto.NombreProyecto = proyectoDto.NombreProyecto;
            proyecto.Descripcion = proyectoDto.Descripcion;
            proyecto.Estado = proyectoDto.Estado;
            proyecto.FechaInicioEstimada = proyectoDto.FechaInicioEstimada;
            proyecto.FechaFinEstimada = proyectoDto.FechaFinEstimada;
            proyecto.FechaInicioReal = proyectoDto.FechaInicioReal;
            proyecto.FechaFinReal = proyectoDto.FechaFinReal;
            proyecto.CostoEstimado = proyectoDto.CostoEstimado;
            proyecto.CostoReal = proyectoDto.CostoReal;
            proyecto.DiseñoAprobado = proyectoDto.DiseñoAprobado;
            proyecto.FechaAprobacionDiseño = proyectoDto.FechaAprobacionDiseño;
            proyecto.ObservacionesCliente = proyectoDto.ObservacionesCliente;
            proyecto.ObservacionesInternas = proyectoDto.ObservacionesInternas;

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
