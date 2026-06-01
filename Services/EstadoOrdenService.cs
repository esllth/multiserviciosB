using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Services
{
    public class EstadoOrdenService : IEstadoOrdenService
    {
        private readonly ApplicationDbContext _context;

        public EstadoOrdenService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EstadoOrdenDTO>> GetAllAsync()
        {
            return await _context.EstadosOrden
                .Select(e => new EstadoOrdenDTO
                {
                    Id = e.Id,
                    Nombre = e.Nombre
                })
                .ToListAsync();
        }

        public async Task<EstadoOrdenDTO?> GetByIdAsync(int id)
        {
            var estado = await _context.EstadosOrden.FindAsync(id);
            if (estado == null) return null;

            return new EstadoOrdenDTO
            {
                Id = estado.Id,
                Nombre = estado.Nombre
            };
        }

        public async Task<bool> CrearAsync(EstadoOrdenDTO dto)
        {
            var existe = await _context.EstadosOrden
                .AnyAsync(e => e.Nombre == dto.Nombre);
            if (existe) return false; 

            var estado = new EstadoOrden { Nombre = dto.Nombre };
            _context.EstadosOrden.Add(estado);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarAsync(EstadoOrdenDTO dto)
        {
            var estado = await _context.EstadosOrden.FindAsync(dto.Id);
            if (estado == null) return false;

            estado.Nombre = dto.Nombre;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var estado = await _context.EstadosOrden.FindAsync(id);
            if (estado == null) return false;

            _context.EstadosOrden.Remove(estado);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}