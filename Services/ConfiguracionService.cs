using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services.Interfaces;

namespace MultiservicioB.Services
{
    public class ConfiguracionService : IConfiguracionService
    {
        private const string ClaveUbicacionTaller = "RevistaNosotrosUbicacion";
        private const string ClaveCorreoElectronico = "RevistaNosotrosCorreo";
        private const string ClaveNumeroTelefono = "RevistaNosotrosTelefono";
        private const string ClaveLeyenda = "RevistaNosotrosLeyenda";
        private readonly ApplicationDbContext _context;

        public ConfiguracionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Horarios 

        public async Task<IEnumerable<HorarioDTO>> GetHorariosAsync()
        {
            return await _context.Horarios
                .Select(h => new HorarioDTO
                {
                    Id = h.Id,
                    DiaSemana = h.DiaSemana,
                    HoraInicio = h.HoraInicio,
                    HoraFin = h.HoraFin,
                    Activo = h.Activo
                })
                .ToListAsync();
        }

        public async Task<HorarioDTO?> GetHorarioByIdAsync(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);
            if (horario == null) return null;

            return new HorarioDTO
            {
                Id = horario.Id,
                DiaSemana = horario.DiaSemana,
                HoraInicio = horario.HoraInicio,
                HoraFin = horario.HoraFin,
                Activo = horario.Activo
            };
        }

        public async Task<bool> CrearHorarioAsync(HorarioDTO dto)
        {
            var horario = new Horario
            {
                DiaSemana = dto.DiaSemana,
                HoraInicio = dto.HoraInicio,
                HoraFin = dto.HoraFin,
                Activo = dto.Activo
            };

            _context.Horarios.Add(horario);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarHorarioAsync(HorarioDTO dto)
        {
            var horario = await _context.Horarios.FindAsync(dto.Id);
            if (horario == null) return false;

            horario.DiaSemana = dto.DiaSemana;
            horario.HoraInicio = dto.HoraInicio;
            horario.HoraFin = dto.HoraFin;
            horario.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarHorarioAsync(int id)
        {
            var horario = await _context.Horarios.FindAsync(id);
            if (horario == null) return false;

            _context.Horarios.Remove(horario);
            await _context.SaveChangesAsync();
            return true;
        }

        //  ZONAS 

        public async Task<IEnumerable<ZonaDTO>> GetZonasAsync()
        {
            return await _context.Zonas
                .Select(z => new ZonaDTO
                {
                    Id = z.Id,
                    Provincia = z.Provincia,
                    Canton = z.Canton,
                    Distrito = z.Distrito,
                    CodigoDTA = z.CodigoDTA,
                    Descripcion = z.Descripcion,
                    Activo = z.Activo
                })
                .ToListAsync();
        }

        public async Task<ZonaDTO?> GetZonaByIdAsync(int id)
        {
            var zona = await _context.Zonas.FindAsync(id);
            if (zona == null) return null;

            return new ZonaDTO
            {
                Id = zona.Id,
                Provincia = zona.Provincia,
                Canton = zona.Canton,
                Distrito = zona.Distrito,
                CodigoDTA = zona.CodigoDTA,
                Descripcion = zona.Descripcion,
                Activo = zona.Activo
            };
        }

        public async Task<bool> CrearZonaAsync(ZonaDTO dto)
        {
            var zona = new Zona
            {
                Provincia = dto.Provincia,
                Canton = dto.Canton,
                Distrito = dto.Distrito,
                CodigoDTA = dto.CodigoDTA,
                Descripcion = dto.Descripcion,
                Activo = dto.Activo
            };

            _context.Zonas.Add(zona);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActualizarZonaAsync(ZonaDTO dto)
        {
            var zona = await _context.Zonas.FindAsync(dto.Id);
            if (zona == null) return false;

            zona.Provincia = dto.Provincia;
            zona.Canton = dto.Canton;
            zona.Distrito = dto.Distrito;
            zona.CodigoDTA = dto.CodigoDTA;
            zona.Descripcion = dto.Descripcion;
            zona.Activo = dto.Activo;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarZonaAsync(int id)
        {
            var zona = await _context.Zonas.FindAsync(id);
            if (zona == null) return false;

            _context.Zonas.Remove(zona);
            await _context.SaveChangesAsync();
            return true;
        }

        //  CONFIGURACION GENERAL 

        public async Task<IEnumerable<ConfiguracionSistemaDTO>> GetConfiguracionesAsync()
        {
            return await _context.ConfiguracionSistema
                .Select(c => new ConfiguracionSistemaDTO
                {
                    Id = c.Id,
                    Clave = c.Clave,
                    Valor = c.Valor,
                    Descripcion = c.Descripcion
                })
                .ToListAsync();
        }

        public async Task<ConfiguracionSistemaDTO?> GetConfiguracionByClaveAsync(string clave)
        {
            var config = await _context.ConfiguracionSistema
                .FirstOrDefaultAsync(c => c.Clave == clave);
            if (config == null) return null;

            return new ConfiguracionSistemaDTO
            {
                Id = config.Id,
                Clave = config.Clave,
                Valor = config.Valor,
                Descripcion = config.Descripcion
            };
        }

        public async Task<bool> ActualizarConfiguracionAsync(ConfiguracionSistemaDTO dto)
        {
            var config = await _context.ConfiguracionSistema.FindAsync(dto.Id);
            if (config == null) return false;

            config.Valor = dto.Valor;
            config.Descripcion = dto.Descripcion;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<RevistaNosotrosDTO> GetRevistaNosotrosAsync()
        {
            var valores = await _context.ConfiguracionSistema
                .AsNoTracking()
                .Where(c => c.Clave == ClaveUbicacionTaller ||
                            c.Clave == ClaveCorreoElectronico ||
                            c.Clave == ClaveNumeroTelefono ||
                            c.Clave == ClaveLeyenda)
                .ToDictionaryAsync(c => c.Clave, c => c.Valor);

            return new RevistaNosotrosDTO
            {
                UbicacionTaller = ObtenerValor(valores, ClaveUbicacionTaller),
                CorreoElectronico = ObtenerValor(valores, ClaveCorreoElectronico),
                NumeroTelefono = ObtenerValor(valores, ClaveNumeroTelefono),
                Leyenda = ObtenerValor(valores, ClaveLeyenda)
            };
        }

        public async Task GuardarRevistaNosotrosAsync(RevistaNosotrosDTO dto)
        {
            await GuardarValorAsync(ClaveUbicacionTaller, dto.UbicacionTaller, "Ubicación del taller mostrada en la revista");
            await GuardarValorAsync(ClaveCorreoElectronico, dto.CorreoElectronico, "Correo de contacto mostrado en la revista");
            await GuardarValorAsync(ClaveNumeroTelefono, dto.NumeroTelefono, "Teléfono de contacto mostrado en la revista");
            await GuardarValorAsync(ClaveLeyenda, dto.Leyenda, "Leyenda de la sección Nosotros de la revista");
            await _context.SaveChangesAsync();
        }

        private async Task GuardarValorAsync(string clave, string valor, string descripcion)
        {
            var config = await _context.ConfiguracionSistema.FirstOrDefaultAsync(c => c.Clave == clave);
            if (config == null)
            {
                _context.ConfiguracionSistema.Add(new ConfiguracionSistema
                {
                    Clave = clave,
                    Valor = valor.Trim(),
                    Descripcion = descripcion
                });
                return;
            }

            config.Valor = valor.Trim();
            config.Descripcion = descripcion;
        }

        private static string ObtenerValor(IReadOnlyDictionary<string, string> valores, string clave) =>
            valores.TryGetValue(clave, out var valor) ? valor : "";
    }
}
