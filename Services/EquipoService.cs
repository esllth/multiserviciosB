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
    public class EquipoService : IEquipoService
    {
        private readonly ApplicationDbContext _context;

        public EquipoService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EquipoDTO>> GetAllAsync()
        {
            return await _context.Equipos
                .Include(e => e.Cliente)
                .Select(e => new EquipoDTO
                {
                    IdEquipo = e.IdEquipo,
                    Nombre = e.Nombre,
                    Categoria = e.Categoria,
                    Especificaciones = e.Especificaciones,
                    Estado = e.Estado,
                    ClienteId = e.ClienteId,
                    NombreCliente = e.Cliente != null ? e.Cliente.Nombre : null
                })
                .ToListAsync();
        }

        public async Task<EquipoDTO?> GetByIdAsync(int id)
        {
            var equipo = await _context.Equipos
                .Include(e => e.Cliente)
                .FirstOrDefaultAsync(e => e.IdEquipo == id);

            if (equipo == null) return null;

            return new EquipoDTO
            {
                IdEquipo = equipo.IdEquipo,
                Nombre = equipo.Nombre,
                Categoria = equipo.Categoria,
                Especificaciones = equipo.Especificaciones,
                Estado = equipo.Estado,
                ClienteId = equipo.ClienteId,
                NombreCliente = equipo.Cliente != null ? equipo.Cliente.Nombre : null
            };
        }

        public async Task<IEnumerable<EquipoDTO>> GetByClienteAsync(int clienteId)
        {
            return await _context.Equipos
                .Include(e => e.Cliente)
                .Where(e => e.ClienteId == clienteId)
                .Select(e => new EquipoDTO
                {
                    IdEquipo = e.IdEquipo,
                    Nombre = e.Nombre,
                    Categoria = e.Categoria,
                    Especificaciones = e.Especificaciones,
                    Estado = e.Estado,
                    ClienteId = e.ClienteId,
                    NombreCliente = e.Cliente != null ? e.Cliente.Nombre : null
                })
                .ToListAsync();
        }

        public async Task<EquipoDTO> CreateAsync(EquipoDTO equipoDto)
        {
            var equipo = new Equipo
            {
                Nombre = equipoDto.Nombre,
                Categoria = equipoDto.Categoria,
                Especificaciones = equipoDto.Especificaciones,
                Estado = equipoDto.Estado ?? "Activo",
                ClienteId = equipoDto.ClienteId
            };

            _context.Equipos.Add(equipo);
            await _context.SaveChangesAsync();

            equipoDto.IdEquipo = equipo.IdEquipo;
            return equipoDto;
        }

        public async Task<bool> UpdateAsync(EquipoDTO equipoDto)
        {
            var equipo = await _context.Equipos.FindAsync(equipoDto.IdEquipo);
            if (equipo == null) return false;

            equipo.Nombre = equipoDto.Nombre;
            equipo.Categoria = equipoDto.Categoria;
            equipo.Especificaciones = equipoDto.Especificaciones;
            equipo.Estado = equipoDto.Estado;
            equipo.ClienteId = equipoDto.ClienteId;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var equipo = await _context.Equipos.FindAsync(id);
            if (equipo == null) return false;

            _context.Equipos.Remove(equipo);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
