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
                    Codigo = m.Codigo,
                    Categoria = m.Categoria,
                    Descripcion = m.Descripcion,
                    UnidadMedida = m.UnidadMedida,
                    StockActual = m.StockActual,
                    StockMinimo = m.StockMinimo,
                    PrecioUnitario = m.PrecioUnitario,
                    AlertaStockActiva = m.AlertaStockActiva,
                    Estado = m.Estado
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialDTO>> GetActivosAsync()
        {
            return await _context.Materiales
                .Where(m => m.Estado == "Activo")
                .Select(m => new MaterialDTO
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    Codigo = m.Codigo,
                    Categoria = m.Categoria,
                    Descripcion = m.Descripcion,
                    UnidadMedida = m.UnidadMedida,
                    StockActual = m.StockActual,
                    StockMinimo = m.StockMinimo,
                    PrecioUnitario = m.PrecioUnitario,
                    AlertaStockActiva = m.AlertaStockActiva,
                    Estado = m.Estado
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
                Codigo = material.Codigo,
                Categoria = material.Categoria,
                Descripcion = material.Descripcion,
                UnidadMedida = material.UnidadMedida,
                StockActual = material.StockActual,
                StockMinimo = material.StockMinimo,
                PrecioUnitario = material.PrecioUnitario,
                AlertaStockActiva = material.AlertaStockActiva,
                Estado = material.Estado
            };
        }

        public async Task<MaterialDTO> CreateAsync(MaterialDTO materialDto)
        {
            var material = new Material
            {
                Nombre = materialDto.Nombre,
                Codigo = materialDto.Codigo,
                Categoria = materialDto.Categoria,
                Descripcion = materialDto.Descripcion,
                UnidadMedida = materialDto.UnidadMedida,
                StockActual = materialDto.StockActual ?? 0,
                StockMinimo = materialDto.StockMinimo ?? 0,
                PrecioUnitario = materialDto.PrecioUnitario ?? 0,
                AlertaStockActiva = materialDto.AlertaStockActiva,
                Estado = materialDto.Estado
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
            material.Codigo = materialDto.Codigo;
            material.Categoria = materialDto.Categoria;
            material.Descripcion = materialDto.Descripcion;
            material.UnidadMedida = materialDto.UnidadMedida;
            material.StockActual = materialDto.StockActual;
            material.StockMinimo = materialDto.StockMinimo;
            material.PrecioUnitario = materialDto.PrecioUnitario;
            material.AlertaStockActiva = materialDto.AlertaStockActiva;
            material.Estado = materialDto.Estado;

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
                .Where(m => m.Estado == "Activo" && 
                           m.StockActual.HasValue && 
                           m.StockMinimo.HasValue && 
                           m.StockActual < m.StockMinimo)
                .Select(m => new MaterialDTO
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    Codigo = m.Codigo,
                    Categoria = m.Categoria,
                    Descripcion = m.Descripcion,
                    UnidadMedida = m.UnidadMedida,
                    StockActual = m.StockActual,
                    StockMinimo = m.StockMinimo,
                    PrecioUnitario = m.PrecioUnitario,
                    AlertaStockActiva = m.AlertaStockActiva,
                    Estado = m.Estado
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<MaterialDTO>> GetStockCriticoAsync()
        {
            return await _context.Materiales
                .Where(m => m.Estado == "Activo" &&
                           m.StockActual.HasValue &&
                           m.StockMinimo.HasValue &&
                           m.StockActual <= (m.StockMinimo * 0.5))
                .Select(m => new MaterialDTO
                {
                    IdMaterial = m.IdMaterial,
                    Nombre = m.Nombre,
                    Codigo = m.Codigo,
                    Categoria = m.Categoria,
                    Descripcion = m.Descripcion,
                    UnidadMedida = m.UnidadMedida,
                    StockActual = m.StockActual,
                    StockMinimo = m.StockMinimo,
                    PrecioUnitario = m.PrecioUnitario,
                    AlertaStockActiva = m.AlertaStockActiva,
                    Estado = m.Estado
                })
                .ToListAsync();
        }

        public async Task<bool> ActualizarStockAsync(int materialId, int cantidad)
        {
            var material = await _context.Materiales.FindAsync(materialId);
            if (material == null) return false;

            material.StockActual = cantidad;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DescontarStockAsync(int materialId, decimal cantidad)
        {
            var material = await _context.Materiales.FindAsync(materialId);
            if (material == null) return false;

            if (!material.StockActual.HasValue)
            {
                material.StockActual = 0;
            }

            material.StockActual = (int)(material.StockActual.Value - cantidad);

            if (material.StockActual < 0)
            {
                material.StockActual = 0;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> VerificarDisponibilidadAsync(int materialId, decimal cantidadRequerida)
        {
            var material = await _context.Materiales.FindAsync(materialId);
            if (material == null) return false;

            return material.StockActual.HasValue && material.StockActual >= cantidadRequerida;
        }

        public async Task<IEnumerable<ConsumoMaterialDTO>> GetHistorialConsumoAsync(int materialId)
        {
            return await _context.ConsumosMaterial
                .Where(c => c.MaterialId == materialId)
                .Include(c => c.Orden)
                    .ThenInclude(o => o!.Cliente)
                .Include(c => c.Material)
                .OrderByDescending(c => c.FechaRegistro)
                .Select(c => new ConsumoMaterialDTO
                {
                    IdConsumo = c.IdConsumo,
                    OrdenId = c.OrdenId,
                    MaterialId = c.MaterialId,
                    NombreMaterial = c.Material!.Nombre,
                    UnidadMedida = c.Material.UnidadMedida,
                    CantidadUsada = c.CantidadUsada ?? 0,
                    PrecioUnitario = c.Material.PrecioUnitario,
                    FechaRegistro = c.FechaRegistro
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsumoMaterialDTO>> GetConsumosPorClienteAsync(int clienteId)
        {
            return await _context.ConsumosMaterial
                .Include(c => c.Orden)
                .Include(c => c.Material)
                .Where(c => c.Orden!.ClienteId == clienteId)
                .OrderByDescending(c => c.FechaRegistro)
                .Select(c => new ConsumoMaterialDTO
                {
                    IdConsumo = c.IdConsumo,
                    OrdenId = c.OrdenId,
                    MaterialId = c.MaterialId,
                    NombreMaterial = c.Material!.Nombre,
                    UnidadMedida = c.Material.UnidadMedida,
                    CantidadUsada = c.CantidadUsada ?? 0,
                    PrecioUnitario = c.Material.PrecioUnitario,
                    FechaRegistro = c.FechaRegistro
                })
                .ToListAsync();
        }

        public async Task VerificarYGenerarAlertasStockAsync()
        {
            // Este método generaría notificaciones/alertas para materiales con stock bajo
            // Se implementará con el sistema de notificaciones
            var materialesBajoStock = await GetBajoStockAsync();

            // TODO: Implementar lógica de envío de notificaciones por email
            // usando el servicio SmtpEmailSender existente

            await Task.CompletedTask;
        }
    }
}
