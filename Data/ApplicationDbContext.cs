using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Models;

namespace MultiservicioB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<UbicacionDTA> UbicacionDTA { get; set; }
        public DbSet<Direccion> Direcciones { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Material> Materiales { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<ProyectoFabricacion> ProyectosFabricacion { get; set; }
        public DbSet<TipoServicio> TiposServicio { get; set; }
        public DbSet<EstadoCotizacion> EstadosCotizacion { get; set; }
        public DbSet<EstadoOrden> EstadosOrden { get; set; }
        public DbSet<Cotizacion> Cotizaciones { get; set; }
        public DbSet<OrdenServicio> OrdenesServicio { get; set; }
        public DbSet<ConsumoMaterial> ConsumosMaterial { get; set; }
        public DbSet<HistorialEquipo> HistorialEquipos { get; set; }
        public DbSet<Horario> Horarios { get; set; }          
        public DbSet<Zona> Zonas { get; set; }               
        public DbSet<ConfiguracionSistema> ConfiguracionSistema { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<TipoServicio>().ToTable("TipoServicio");
            builder.Entity<EstadoCotizacion>().ToTable("EstadoCotizacion");
            builder.Entity<EstadoOrden>().ToTable("EstadoOrden");
            builder.Entity<ConfiguracionSistema>().ToTable("ConfiguracionSistema");

            builder.Entity<Empleado>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Direccion>()
                .HasOne(d => d.UbicacionDTA)
                .WithMany()
                .HasForeignKey(d => d.UbicacionDTAId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cliente>()
                .HasOne(c => c.Direccion)
                .WithMany()
                .HasForeignKey(c => c.DireccionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Equipo>()
                .HasOne(e => e.Cliente)
                .WithMany()
                .HasForeignKey(e => e.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ProyectoFabricacion>()
                .HasOne(p => p.Cliente)
                .WithMany()
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Cotizacion>()
                .HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrdenServicio>()
                .HasOne(o => o.Cotizacion)
                .WithMany()
                .HasForeignKey(o => o.CotizacionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrdenServicio>()
                .HasOne(o => o.Cliente)
                .WithMany()
                .HasForeignKey(o => o.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<OrdenServicio>()
                .HasOne(o => o.Empleado)
                .WithMany()
                .HasForeignKey(o => o.EmpleadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ConsumoMaterial>()
                .HasOne(c => c.Orden)
                .WithMany()
                .HasForeignKey(c => c.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ConsumoMaterial>()
                .HasOne(c => c.Material)
                .WithMany()
                .HasForeignKey(c => c.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HistorialEquipo>()
                .HasOne(h => h.Equipo)
                .WithMany()
                .HasForeignKey(h => h.EquipoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HistorialEquipo>()
                .HasOne(h => h.Orden)
                .WithMany()
                .HasForeignKey(h => h.OrdenId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}