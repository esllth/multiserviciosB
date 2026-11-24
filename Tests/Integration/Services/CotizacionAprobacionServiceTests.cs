using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;
using MultiservicioB.Services;

namespace MultiservicioB.Tests.Integration.Services;

public sealed class CotizacionAprobacionServiceTests
{
    [Fact]
    public async Task AprobarAsync_WithoutPdf_DoesNotApproveOrCreateOrder()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddPendingFixtureAsync(database.Context);

        var service = new CotizacionAprobacionService(database.Context);
        var result = await service.AprobarAsync(fixture.CotizacionId, fixture.UsuarioId);

        Assert.False(result.Exito);
        Assert.Contains("PDF", result.Error);
        Assert.False(await database.Context.OrdenesServicio.AnyAsync(o => o.CotizacionId == fixture.CotizacionId));

        var quotation = await database.Context.Cotizaciones.Include(c => c.EstadoCotizacion).SingleAsync(c => c.IdCotizacion == fixture.CotizacionId);
        Assert.NotEqual("Aprobada", quotation.EstadoCotizacion!.Nombre);
    }

    [Fact]
    public async Task AprobarAsync_WithPdf_ApprovesLocksDocumentAndCreatesOneOrderWithEvidence()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddPendingFixtureAsync(database.Context);
        database.Context.DocumentosCotizacion.Add(new DocumentoCotizacion
        {
            CotizacionId = fixture.CotizacionId,
            NombreOriginal = "cotizacion-firmada.pdf",
            Ruta = "uploads/cotizaciones/1/firmada.pdf",
            TipoContenido = "application/pdf",
            TipoDocumento = "CotizacionFirmada",
            HashSha256 = new string('a', 64),
            TamanoBytes = 1024,
            FechaCarga = DateTime.Now,
            CargadoPorUsuarioId = fixture.UsuarioId,
            EsReemplazable = true
        });
        await database.Context.SaveChangesAsync();

        var service = new CotizacionAprobacionService(database.Context);
        var result = await service.AprobarAsync(fixture.CotizacionId, fixture.UsuarioId);

        Assert.True(result.Exito);
        Assert.True(result.OrdenId > 0);
        Assert.Single(database.Context.OrdenesServicio.Where(o => o.CotizacionId == fixture.CotizacionId));

        var quotation = await database.Context.Cotizaciones.Include(c => c.EstadoCotizacion).SingleAsync(c => c.IdCotizacion == fixture.CotizacionId);
        Assert.Equal("Aprobada", quotation.EstadoCotizacion!.Nombre);
        Assert.Equal(fixture.UsuarioId, quotation.AprobadaPorUsuarioId);
        Assert.NotNull(quotation.FechaAprobacion);

        var sourceDocument = await database.Context.DocumentosCotizacion.SingleAsync(d => d.CotizacionId == fixture.CotizacionId);
        Assert.False(sourceDocument.EsReemplazable);

        var orderDocument = await database.Context.DocumentosOrdenServicio.SingleAsync(d => d.OrdenId == result.OrdenId);
        Assert.True(orderDocument.EsEvidenciaContractual);
        Assert.Equal(sourceDocument.IdDocumento, orderDocument.DocumentoCotizacionId);
        Assert.Equal("application/pdf", orderDocument.TipoContenido);

        Assert.True(await database.Context.Auditorias.AnyAsync(a =>
            a.UsuarioId == fixture.UsuarioId && a.Accion == "AprobarCotizacion"));
    }

    [Fact]
    public async Task AprobarAsync_SecondAttempt_IsDuplicateProtected()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddPendingFixtureAsync(database.Context);
        database.Context.DocumentosCotizacion.Add(new DocumentoCotizacion
        {
            CotizacionId = fixture.CotizacionId,
            NombreOriginal = "cotizacion.pdf",
            Ruta = "uploads/cotizaciones/1/cotizacion.pdf",
            TipoContenido = "application/pdf",
            TipoDocumento = "CotizacionFirmada",
            HashSha256 = new string('b', 64),
            TamanoBytes = 512,
            FechaCarga = DateTime.Now,
            CargadoPorUsuarioId = fixture.UsuarioId,
            EsReemplazable = true
        });
        await database.Context.SaveChangesAsync();

        var service = new CotizacionAprobacionService(database.Context);
        var first = await service.AprobarAsync(fixture.CotizacionId, fixture.UsuarioId);
        var second = await service.AprobarAsync(fixture.CotizacionId, fixture.UsuarioId);

        Assert.True(first.Exito);
        Assert.False(second.Exito);
        Assert.Contains("ya fue aprobada", second.Error);
        Assert.Single(database.Context.OrdenesServicio.Where(o => o.CotizacionId == fixture.CotizacionId));
    }

    [Fact]
    public async Task AprobarAsync_WhenQuotationIsEvaluated_WithoutPdf_DoesNotApprove()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddPendingFixtureAsync(database.Context);

        var service = new CotizacionAprobacionService(database.Context);
        var result = await service.AprobarAsync(fixture.CotizacionId, fixture.UsuarioId);

        Assert.False(result.Exito);
        Assert.Contains("PDF", result.Error);
        Assert.False(await database.Context.OrdenesServicio.AnyAsync(o => o.CotizacionId == fixture.CotizacionId));
    }

    [Fact]
    public async Task RechazarAsync_WithMotivo_RegistersAuditAndState()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddPendingFixtureAsync(database.Context);

        var service = new CotizacionAprobacionService(database.Context);
        var result = await service.RechazarAsync(fixture.CotizacionId, fixture.UsuarioId, "Documentación incompleta");

        Assert.True(result.Exito);
        var quotation = await database.Context.Cotizaciones.Include(c => c.EstadoCotizacion).SingleAsync(c => c.IdCotizacion == fixture.CotizacionId);
        Assert.Equal("Rechazada", quotation.EstadoCotizacion!.Nombre);
        Assert.Equal("Documentación incompleta", quotation.MotivoRechazo);
        Assert.Equal(fixture.UsuarioId, quotation.RechazadaPorUsuarioId);
        Assert.NotNull(quotation.FechaRechazo);
        Assert.False(await database.Context.OrdenesServicio.AnyAsync(o => o.CotizacionId == fixture.CotizacionId));
    }

    [Fact]
    public async Task RechazarAsync_WithoutMotivo_DoesNotChangeState()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddPendingFixtureAsync(database.Context);

        var service = new CotizacionAprobacionService(database.Context);
        var result = await service.RechazarAsync(fixture.CotizacionId, fixture.UsuarioId, "  ");

        Assert.False(result.Exito);
        Assert.Contains("motivo", result.Error);
        var quotation = await database.Context.Cotizaciones.Include(c => c.EstadoCotizacion).SingleAsync(c => c.IdCotizacion == fixture.CotizacionId);
        Assert.NotEqual("Rechazada", quotation.EstadoCotizacion!.Nombre);
    }

    private static async Task<Fixture> AddPendingFixtureAsync(ApplicationDbContext context)
    {
        var evaluatedQuotationState = new EstadoCotizacion { Nombre = "Evaluada" };
        var approvedQuotationState = new EstadoCotizacion { Nombre = "Aprobada" };
        var rejectedQuotationState = new EstadoCotizacion { Nombre = "Rechazada" };
        context.EstadosCotizacion.AddRange(evaluatedQuotationState, approvedQuotationState, rejectedQuotationState);

        var pendingOrderState = new EstadoOrden { Nombre = "Pendiente" };
        context.EstadosOrden.Add(pendingOrderState);

        var user = new IdentityUser
        {
            Id = Guid.NewGuid().ToString("N"),
            UserName = "admin@example.test",
            NormalizedUserName = "ADMIN@EXAMPLE.TEST",
            Email = "admin@example.test",
            NormalizedEmail = "ADMIN@EXAMPLE.TEST"
        };
        context.Users.Add(user);

        var serviceType = new TipoServicio { Nombre = "Prueba documental", Estado = "Activo", RequiereVisita = true };
        context.TiposServicio.Add(serviceType);

        var client = new Cliente
        {
            Identificacion = "123456789",
            Nombre = "Cliente",
            Apellidos = "Documento",
            Estado = "Activo"
        };
        context.Clientes.Add(client);
        await context.SaveChangesAsync();

        var quotation = new Cotizacion
        {
            ClienteId = client.IdCliente,
            TipoServicioId = serviceType.Id,
            EstadoCotizacionId = evaluatedQuotationState.Id,
            Descripcion = "Cotización de prueba",
            MontoPresupuesto = 50000m,
            RequiereAdelanto = false,
            FechaSolicitud = DateTime.UtcNow
        };
        context.Cotizaciones.Add(quotation);
        await context.SaveChangesAsync();

        return new Fixture(quotation.IdCotizacion, user.Id);
    }

    private sealed record Fixture(int CotizacionId, string UsuarioId);

    private sealed class SqliteApplicationDatabase : IAsyncDisposable
    {
        private readonly SqliteConnection _connection;

        private SqliteApplicationDatabase(SqliteConnection connection, ApplicationDbContext context)
        {
            _connection = connection;
            Context = context;
        }

        public ApplicationDbContext Context { get; }

        public static async Task<SqliteApplicationDatabase> CreateAsync()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            await connection.OpenAsync();

            try
            {
                var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseSqlite(connection)
                    .Options;
                var context = new ApplicationDbContext(options);
                await context.Database.EnsureCreatedAsync();
                return new SqliteApplicationDatabase(connection, context);
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _connection.DisposeAsync();
        }
    }
}
