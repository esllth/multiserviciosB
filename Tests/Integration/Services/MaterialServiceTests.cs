using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using MultiservicioB.Data;
using MultiservicioB.DTOs;
using MultiservicioB.Models;
using MultiservicioB.Services;

namespace MultiservicioB.Tests.Integration.Services;

public sealed class MaterialServiceTests
{
    [Fact]
    public async Task CreateAsync_PersistsMaterialAndGeneratesItsInventoryCode()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var service = new MaterialService(database.Context);

        var created = await service.CreateAsync(new MaterialDTO
        {
            Nombre = "Acero inoxidable",
            Estado = "Activo",
            StockActual = 12,
            StockMinimo = 3,
            AlertaStockActiva = true
        });

        Assert.True(created.IdMaterial > 0);
        Assert.Equal($"A-{created.IdMaterial:D4}", created.Codigo);

        var persisted = await database.Context.Materiales.FindAsync(created.IdMaterial);
        Assert.NotNull(persisted);
        Assert.Equal("Acero inoxidable", persisted.Nombre);
        Assert.Equal(12, persisted.StockActual);
    }

    [Fact]
    public async Task DescontarStockAsync_ClampsAtZeroAndAvailabilityUsesPersistedStock()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        var material = await AddMaterialAsync(database.Context, "Tornillos", stock: 4);
        var service = new MaterialService(database.Context);

        var availableBeforeUse = await service.VerificarDisponibilidadAsync(material.IdMaterial, 4);
        var updated = await service.DescontarStockAsync(material.IdMaterial, 10);
        var availableAfterUse = await service.VerificarDisponibilidadAsync(material.IdMaterial, 1);

        Assert.True(availableBeforeUse);
        Assert.True(updated);
        Assert.False(availableAfterUse);
        Assert.Equal(0, material.StockActual);
    }

    [Fact]
    public async Task GetBajoStockAsync_ReturnsOnlyActiveMaterialsBelowMinimum()
    {
        await using var database = await SqliteApplicationDatabase.CreateAsync();
        database.Context.Materiales.AddRange(
            new Material { Nombre = "Cable bajo", Estado = "Activo", StockActual = 1, StockMinimo = 4 },
            new Material { Nombre = "Cable suficiente", Estado = "Activo", StockActual = 4, StockMinimo = 4 },
            new Material { Nombre = "Cable inactivo", Estado = "Inactivo", StockActual = 0, StockMinimo = 4 });
        await database.Context.SaveChangesAsync();
        var service = new MaterialService(database.Context);

        var lowStock = (await service.GetBajoStockAsync()).ToList();

        var material = Assert.Single(lowStock);
        Assert.Equal("Cable bajo", material.Nombre);
        Assert.Equal(1, material.StockActual);
    }

    private static async Task<Material> AddMaterialAsync(
        ApplicationDbContext context,
        string name,
        int stock)
    {
        var material = new Material
        {
            Nombre = name,
            Estado = "Activo",
            StockActual = stock,
            StockMinimo = 2
        };

        context.Materiales.Add(material);
        await context.SaveChangesAsync();
        return material;
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
