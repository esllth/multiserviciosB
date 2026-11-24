using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MultiservicioB.Data;

namespace MultiservicioB.Tests.Integration.Web;

public sealed class ApplicationHttpTests : IClassFixture<MultiservicioBApplicationFactory>
{
    private readonly HttpClient _client;

    public ApplicationHttpTests(MultiservicioBApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
    }

    [Fact]
    public async Task HomeRoute_RedirectsToPublicMagazine()
    {
        using var response = await _client.GetAsync("/Home/Index");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Revista", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task AdminMagazineRoute_RequiresAuthentication()
    {
        using var response = await _client.GetAsync("/Revista/Editar");

        Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Identity/Account/Login", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Responses_IncludeSecurityHeaders()
    {
        using var response = await _client.GetAsync("/Home/Index");

        Assert.Equal("nosniff", Assert.Single(response.Headers.GetValues("X-Content-Type-Options")));
        Assert.Equal("DENY", Assert.Single(response.Headers.GetValues("X-Frame-Options")));
        Assert.Equal(
            "strict-origin-when-cross-origin",
            Assert.Single(response.Headers.GetValues("Referrer-Policy")));
    }
}

public sealed class MultiservicioBApplicationFactory : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:DefaultConnection"] = "Server=localhost;Database=MultiservicioBTests;User Id=test;Password=test;TrustServerCertificate=True",
                ["Testing:SkipDatabaseInitialization"] = "true",
                ["Logging:LogLevel:Default"] = "Warning"
            }));

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
            services.AddSingleton(_connection);
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _connection?.Dispose();
        }

        base.Dispose(disposing);
    }
}
