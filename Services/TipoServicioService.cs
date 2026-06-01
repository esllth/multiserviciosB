using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Services
{
    public class TipoServicioService : ITipoServicioService
    {
        private readonly ApplicationDbContext _context;

        public TipoServicioService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TipoServicioDTO>> GetAllAsync()
        {
            return await _context.TiposServicio
                .Select(t => new TipoServicioDTO
                {
                    Id = t.Id,
                    Nombre = t.Nombre,
                    Estado = t.Estado
                })
                .ToListAsync();
        }

        public async Task<TipoServicioDTO?> GetByIdAsync(int id)
        {
            var tipoServicio = await _context.TiposServicio.FindAsync(id);
            if (tipoServicio == null) return null;

            return new TipoServicioDTO
            {
                Id = tipoServicio.Id,
                Nombre = tipoServicio.Nombre,
                Estado = tipoServicio.Estado
            };
        }

        public async Task<bool> CrearAsync(TipoServicioDTO dto)
        {
            var tipoServicio = new TipoServicio
            {
                Nombre = dto.Nombre,
                Estado = dto.Estado ?? "Activo"
            };

            _context.TiposServicio.Add(tipoServicio);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarAsync(TipoServicioDTO dto)
        {
            var tipoServicio = await _context.TiposServicio.FindAsync(dto.Id);
            if (tipoServicio == null) return false;

            tipoServicio.Nombre = dto.Nombre;
            tipoServicio.Estado = dto.Estado;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var tipoServicio = await _context.TiposServicio.FindAsync(id);
            if (tipoServicio == null) return false;

            _context.TiposServicio.Remove(tipoServicio);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}

