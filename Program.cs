using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Services;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.AllowedForNewUsers = false;
    options.Password.RequiredLength = 12;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders() // necesario para Identity completo
// Descripciones de error en español
.AddErrorDescriber<SpanishIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = builder.Environment.IsDevelopment()
        ? "MultiservicioB.Auth"
        : "__Host-MultiservicioB.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = false;
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
    options.TokenLifespan = TimeSpan.FromMinutes(30));
builder.Services.Configure<SecurityStampValidatorOptions>(options =>
    options.ValidationInterval = TimeSpan.FromMinutes(1));

builder.Services.AddMemoryCache();
builder.Services.AddDataProtection();
builder.Services.AddSingleton<ILoginSecurityService, LoginSecurityService>();
builder.Services.Configure<AuthenticationSecurityOptions>(
    builder.Configuration.GetSection(AuthenticationSecurityOptions.SectionName));

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "text/plain; charset=utf-8";
        await context.HttpContext.Response.WriteAsync(
            "Demasiados intentos. Espere un minuto antes de volver a intentar.",
            cancellationToken);
    };
    options.AddPolicy("authentication", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

// MVC + Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Servicios
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.AddTransient<IEmailSender, SmtpEmailSender>();
builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IEquipoService, EquipoService>();
builder.Services.AddScoped<IProyectoFabricacionService, ProyectoFabricacionService>();
builder.Services.AddScoped<IOrdenServicioService, OrdenServicioService>();
builder.Services.AddScoped<IRolService, RolService>();
builder.Services.AddScoped<IConfiguracionService, ConfiguracionService>();
builder.Services.AddScoped<ITipoServicioService, TipoServicioService>();
builder.Services.AddScoped<IEstadoOrdenService, EstadoOrdenService>();

var app = builder.Build();

// Crear roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Administrador", "Empleado", "Cliente" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var context = services.GetRequiredService<ApplicationDbContext>();

        string[] estadosCotizacion = { "Pendiente", "Evaluada", "Aprobada", "Rechazada" };
        foreach (var nombre in estadosCotizacion)
        {
            if (!await context.EstadosCotizacion.AnyAsync(e => e.Nombre == nombre))
                context.EstadosCotizacion.Add(new EstadoCotizacion { Nombre = nombre });
        }

        string[] estadosOrden = { "Pendiente", "En Progreso", "Completada", "Cancelada" };
        foreach (var nombre in estadosOrden)
        {
            if (!await context.EstadosOrden.AnyAsync(e => e.Nombre == nombre))
                context.EstadosOrden.Add(new EstadoOrden { Nombre = nombre });
        }

        string[] tiposServicio = { "Mantenimiento", "Reparación", "Instalación", "Inspección", "Consultoría" };
        foreach (var nombre in tiposServicio)
        {
            if (!await context.TiposServicio.AnyAsync(t => t.Nombre == nombre))
                context.TiposServicio.Add(new TipoServicio { Nombre = nombre, Estado = "Activo" });
        }

        if (!await context.Empleados.AnyAsync(e => e.NombreEmpleado == "Bolivar" && e.ApellidosEmpleado == "Alpizar"))
        {
            context.Empleados.Add(new Empleado
            {
                IdentificacionEmpleado = "BOLIVAR-ALPIZAR",
                NombreEmpleado = "Bolivar",
                ApellidosEmpleado = "Alpizar",
                CorreoElectronicoEmpleado = "bolivar.alpizar@multiserviciosb.com",
                TelefonoEmpleado = "0000-0000",
                EstadoEmpleado = true,
                TieneUsuario = false,
                EstadoAcceso = EstadosEmpleado.Activo,
                SalarioBase = 0,
                FechaInicioEmpleado = DateTime.UtcNow
            });
        }

        await context.SaveChangesAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al crear roles");
        throw;
    }
}

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    context.Response.Headers.XContentTypeOptions = "nosniff";
    context.Response.Headers.XFrameOptions = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔴 IMPORTANTE: esto habilita Identity (Areas)
app.MapRazorPages();

app.Run();
