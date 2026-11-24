using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services;

namespace MultiservicioB.Tests.Integration.Services;

public sealed class OrdenServicioFlowTests
{
    [Fact]
    public async Task CreateAsync_WithApprovedCompletePayment_CreatesPendingOrder()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddApprovedFixtureAsync(database.Context, "Completo", false, null);
        var service = new OrdenServicioService(database.Context);

        var created = await service.CreateAsync(CreateDto(fixture, confirmed: false));

        Assert.True(created.IdOrden > 0);
        Assert.Equal(fixture.CotizacionId, created.CotizacionId);
        Assert.False(created.CompromisoConfirmado);
        Assert.Single(database.Context.OrdenesServicio.Where(o => o.CotizacionId == fixture.CotizacionId));
    }

    [Fact]
    public async Task CreateAsync_WithEvaluatedQuotation_DoesNotCreateOrder()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddFixtureAsync(database.Context, "Evaluada", "Completo", false, null);
        var service = new OrdenServicioService(database.Context);

        var created = await service.CreateAsync(CreateDto(fixture, confirmed: false));

        Assert.Equal(0, created.IdOrden);
        Assert.False(await database.Context.OrdenesServicio.AnyAsync(o => o.CotizacionId == fixture.CotizacionId));
    }

    [Fact]
    public async Task CreateAsync_WhenOrderAlreadyExists_DoesNotCreateDuplicate()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddApprovedFixtureAsync(database.Context, "Completo", false, null);
        var service = new OrdenServicioService(database.Context);

        var first = await service.CreateAsync(CreateDto(fixture, confirmed: false));
        var duplicate = await service.CreateAsync(CreateDto(fixture, confirmed: false));

        Assert.True(first.IdOrden > 0);
        Assert.Equal(0, duplicate.IdOrden);
        Assert.Single(database.Context.OrdenesServicio.Where(o => o.CotizacionId == fixture.CotizacionId));
    }

    [Fact]
    public async Task UpdateAsync_WhenPaymentIsMissing_DoesNotConfirmCommitment()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddApprovedFixtureAsync(database.Context, null, false, null);
        var service = new OrdenServicioService(database.Context);

        var created = await service.CreateAsync(CreateDto(fixture, confirmed: false));
        var updateDto = CreateDto(fixture, confirmed: true);
        updateDto.IdOrden = created.IdOrden;
        var updated = await service.UpdateAsync(updateDto);

        Assert.False(updated);
        var persisted = await database.Context.OrdenesServicio.FindAsync(created.IdOrden);
        Assert.NotNull(persisted);
        Assert.False(persisted.CompromisoConfirmado);
    }

    [Fact]
    public async Task UpdateAsync_WhenDownPaymentModeDoesNotMatchQuotation_DoesNotConfirmCommitment()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddApprovedFixtureAsync(database.Context, "AdelantoSaldo", false, null);
        var service = new OrdenServicioService(database.Context);

        var created = await service.CreateAsync(CreateDto(fixture, confirmed: false));
        var updateDto = CreateDto(fixture, confirmed: true);
        updateDto.IdOrden = created.IdOrden;
        var updated = await service.UpdateAsync(updateDto);

        Assert.False(updated);
        var persisted = await database.Context.OrdenesServicio.FindAsync(created.IdOrden);
        Assert.NotNull(persisted);
        Assert.False(persisted.CompromisoConfirmado);
    }

    [Fact]
    public async Task UpdateAsync_WithApprovedQuotationAndMatchingDownPayment_ConfirmsCommitment()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddApprovedFixtureAsync(database.Context, "AdelantoSaldo", true, 30);
        var service = new OrdenServicioService(database.Context);

        var created = await service.CreateAsync(CreateDto(fixture, confirmed: false));
        var updateDto = CreateDto(fixture, confirmed: true);
        updateDto.IdOrden = created.IdOrden;
        updateDto.FechaCompromiso = new DateTime(2026, 12, 1, 9, 0, 0);
        var updated = await service.UpdateAsync(updateDto);

        Assert.True(updated);
        var persisted = await database.Context.OrdenesServicio.FindAsync(created.IdOrden);
        Assert.NotNull(persisted);
        Assert.True(persisted.CompromisoConfirmado);
        Assert.Equal(new DateTime(2026, 12, 1, 9, 0, 0), persisted.FechaCompromiso);
    }

    [Fact]
    public async Task RejectionState_DoesNotCreateOrder()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var fixture = await AddFixtureAsync(database.Context, "Rechazada", null, false, null);
        var service = new OrdenServicioService(database.Context);

        var created = await service.CreateAsync(CreateDto(fixture, confirmed: false));

        Assert.Equal(0, created.IdOrden);
        Assert.False(await database.Context.OrdenesServicio.AnyAsync(o => o.CotizacionId == fixture.CotizacionId));
    }

    private static OrdenServicioDTO CreateDto(FlowFixture fixture, bool confirmed)
    {
        return new OrdenServicioDTO
        {
            CotizacionId = fixture.CotizacionId,
            ClienteId = fixture.ClienteId,
            EstadoOrdenId = fixture.EstadoOrdenPendienteId,
            FechaCompromiso = new DateTime(2026, 12, 1, 9, 0, 0),
            CompromisoConfirmado = confirmed,
            UsarDireccionPerfil = true,
            RequiereFotosObligatorias = false
        };
    }

    private static async Task<FlowFixture> AddApprovedFixtureAsync(
        ApplicationDbContext context,
        string? payment,
        bool requiresDownPayment,
        int? downPaymentPercentage)
    {
        return await AddFixtureAsync(context, "Aprobada", payment, requiresDownPayment, downPaymentPercentage);
    }

    private static async Task<FlowFixture> AddFixtureAsync(
        ApplicationDbContext context,
        string quotationState,
        string? payment,
        bool requiresDownPayment,
        int? downPaymentPercentage)
    {
        var approvedState = await context.EstadosCotizacion.FirstOrDefaultAsync(e => e.Nombre == quotationState);
        if (approvedState == null)
        {
            approvedState = new EstadoCotizacion { Nombre = quotationState };
            context.EstadosCotizacion.Add(approvedState);
        }

        var pendingState = await context.EstadosOrden.FirstOrDefaultAsync(e => e.Nombre == "Pendiente");
        if (pendingState == null)
        {
            pendingState = new EstadoOrden { Nombre = "Pendiente" };
            context.EstadosOrden.Add(pendingState);
        }

        var serviceType = new TipoServicio
        {
            Nombre = $"Servicio {Guid.NewGuid():N}",
            Estado = "Activo",
            RequiereVisita = true
        };
        context.TiposServicio.Add(serviceType);

        var client = new Cliente
        {
            Identificacion = $"{Guid.NewGuid():N}"[..20],
            Nombre = "Cliente Prueba",
            Apellidos = "Flujo",
            Correo = $"{Guid.NewGuid():N}@example.test",
            Estado = "Activo"
        };
        context.Clientes.Add(client);

        await context.SaveChangesAsync();

        var quotation = new Cotizacion
        {
            ClienteId = client.IdCliente,
            TipoServicioId = serviceType.Id,
            EstadoCotizacionId = approvedState.Id,
            Descripcion = "Prueba del flujo de cotización",
            MontoPresupuesto = 100000m,
            RequiereAdelanto = requiresDownPayment,
            PorcentajeAdelanto = downPaymentPercentage,
            FormaPagoAceptada = payment,
            AprobadaPorCliente = quotationState == "Aprobada",
            FechaSolicitud = DateTime.UtcNow
        };
        context.Cotizaciones.Add(quotation);
        await context.SaveChangesAsync();

        return new FlowFixture
        {
            CotizacionId = quotation.IdCotizacion,
            ClienteId = client.IdCliente,
            EstadoOrdenPendienteId = pendingState.Id
        };
    }

    private sealed record FlowFixture
    {
        public int CotizacionId { get; init; }
        public int ClienteId { get; init; }
        public int EstadoOrdenPendienteId { get; init; }
    }

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
