using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MultiservicioB.Models;

namespace MultiservicioB.Data
{
    public class AuditoriaSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditoriaSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            AgregarRegistros(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AgregarRegistros(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AgregarRegistros(DbContext? context)
        {
            if (context == null) return;

            var usuarioId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(usuarioId)) return;

            var cambios = context.ChangeTracker.Entries()
                .Where(e => e.Entity is not Auditoria && e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
                .Select(CrearRegistro)
                .Where(a => a != null)
                .Cast<Auditoria>()
                .ToList();

            foreach (var cambio in cambios)
            {
                cambio.UsuarioId = usuarioId;
                context.Set<Auditoria>().Add(cambio);
            }
        }

        private static Auditoria? CrearRegistro(EntityEntry entry)
        {
            var entidad = entry.Metadata.ClrType.Name;
            var accion = entry.State switch
            {
                EntityState.Added => "Creación",
                EntityState.Modified => "Actualización",
                EntityState.Deleted => "Eliminación",
                _ => null
            };
            if (accion == null) return null;

            var claves = entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey() && !p.IsTemporary)
                .Select(p => $"{p.Metadata.Name}={p.CurrentValue}")
                .ToList();
            var campos = entry.State == EntityState.Modified
                ? entry.Properties.Where(p => p.IsModified && !EsCampoSensible(p.Metadata.Name)).Select(p => p.Metadata.Name).ToList()
                : new List<string>();

            var detalle = $"{entidad}";
            if (claves.Count > 0) detalle += $" ({string.Join(", ", claves)})";
            if (campos.Count > 0) detalle += $". Campos: {string.Join(", ", campos)}";

            return new Auditoria
            {
                Accion = Limitar($"{accion} de {entidad}", 100),
                Detalle = Limitar(detalle, 255),
                Fecha = DateTime.Now
            };
        }

        private static bool EsCampoSensible(string nombre) =>
            nombre.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
            nombre.Contains("Token", StringComparison.OrdinalIgnoreCase) ||
            nombre.Contains("Stamp", StringComparison.OrdinalIgnoreCase) ||
            nombre.Contains("Secret", StringComparison.OrdinalIgnoreCase);

        private static string Limitar(string texto, int longitud) =>
            texto.Length <= longitud ? texto : texto[..longitud];
    }
}
