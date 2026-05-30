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
    public class MaterialService : IMaterialService
    {
        private readonly ApplicationDbContext _context;

        public MaterialService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MaterialDTO>> GetAllAsync()
        {
            return await _context.Materiales
                .Select(m => new MaterialDTO
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion,
                    UnidadMedida = m.UnidadMedida,
                    StockActual = m.StockActual,
                    StockMinimo = m.StockMinimo,
                    PrecioUnitario = m.PrecioUnitario
                })
                .ToListAsync();
        }

        public async Task<MaterialDTO?> GetByIdAsync(int id)
        {
            var material = await _context.Materiales.FindAsync(id);
            if (material == null) return null;

            return new MaterialDTO
            {
                IdMaterial = material.IdMaterial,
                Nombre = material.Nombre,
                Descripcion = material.Descripcion,
                UnidadMedida = material.UnidadMedida,
                StockActual = material.StockActual,
                StockMinimo = material.StockMinimo,
                PrecioUnitario = material.PrecioUnitario
            };
        }

        public async Task<MaterialDTO> CreateAsync(MaterialDTO materialDto)
        {
            var material = new Material
            {
                Nombre = materialDto.Nombre,
                Descripcion = materialDto.Descripcion,
                UnidadMedida = materialDto.UnidadMedida,
                StockActual = materialDto.StockActual ?? 0,
                StockMinimo = materialDto.StockMinimo ?? 0,
                PrecioUnitario = materialDto.PrecioUnitario ?? 0
            };

            _context.Materiales.Add(material);
            await _context.SaveChangesAsync();

            materialDto.IdMaterial = material.IdMaterial;
            return materialDto;
        }

        public async Task<bool> UpdateAsync(MaterialDTO materialDto)
        {
            var material = await _context.Materiales.FindAsync(materialDto.IdMaterial);
            if (material == null) return false;

            material.Nombre = materialDto.Nombre;
            material.Descripcion = materialDto.Descripcion;
            material.UnidadMedida = materialDto.UnidadMedida;
            material.StockActual = materialDto.StockActual;
            material.StockMinimo = materialDto.StockMinimo;
            material.PrecioUnitario = materialDto.PrecioUnitario;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var material = await _context.Materiales.FindAsync(id);
            if (material == null) return false;

            _context.Materiales.Remove(material);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MaterialDTO>> GetBajoStockAsync()
        {
            return await _context.Materiales
                .Where(m => m.StockActual.HasValue && m.StockMinimo.HasValue && m.StockActual < m.StockMinimo)
                .Select(m => new MaterialDTO
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    Descripcion = m.Descripcion,
                    UnidadMedida = m.UnidadMedida,
                    StockActual = m.StockActual,
                    StockMinimo = m.StockMinimo,
                    PrecioUnitario = m.PrecioUnitario
                })
                .ToListAsync();
        }
    }
}
