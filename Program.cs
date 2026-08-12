using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditoriaSaveChangesInterceptor>();
builder.Services.AddDbContext<ApplicationDbContext>((services, options) =>
    options.UseSqlServer(connectionString)
           .AddInterceptors(services.GetRequiredService<AuditoriaSaveChangesInterceptor>())
           .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

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
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();

// Servicios
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.PostConfigure<SmtpOptions>(options =>
{
    ConfigureSmtpFromEmailEnvironment(builder.Configuration, options);
});
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

// Aplicar migrations y seed inicial
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error al aplicar migrations");
    }
}

// Crear roles
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        string[] roles = { "Administrador", "Empleado", "Cliente", "Secretaria" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        var rolGerente = await roleManager.FindByNameAsync("Gerente");
        if (rolGerente != null)
        {
            var gerentes = await userManager.GetUsersInRoleAsync("Gerente");
            foreach (var gerente in gerentes)
            {
                if (!await userManager.IsInRoleAsync(gerente, "Secretaria"))
                {
                    await userManager.AddToRoleAsync(gerente, "Secretaria");
                }
            }
            await roleManager.DeleteAsync(rolGerente);
        }

        var administradorPrincipal = await userManager.FindByEmailAsync("admin@multiserviciosb.com");
        if (administradorPrincipal != null)
        {
            if (!await userManager.IsInRoleAsync(administradorPrincipal, "Administrador"))
                await userManager.AddToRoleAsync(administradorPrincipal, "Administrador");
            if (!await userManager.IsInRoleAsync(administradorPrincipal, "Empleado"))
                await userManager.AddToRoleAsync(administradorPrincipal, "Empleado");
            if (await userManager.IsInRoleAsync(administradorPrincipal, "Secretaria"))
                await userManager.RemoveFromRoleAsync(administradorPrincipal, "Secretaria");
            if (await userManager.IsInRoleAsync(administradorPrincipal, "Cliente"))
                await userManager.RemoveFromRoleAsync(administradorPrincipal, "Cliente");
        }

        var context = services.GetRequiredService<ApplicationDbContext>();

        var perfilAdministradorPrincipal = await context.Empleados
            .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado.ToLower() == "admin@multiserviciosb.com");
        if (perfilAdministradorPrincipal != null)
        {
            perfilAdministradorPrincipal.NombreEmpleado = "Administrador";
            perfilAdministradorPrincipal.ApellidosEmpleado = "de órdenes de servicio";
            perfilAdministradorPrincipal.TelefonoEmpleado = perfilAdministradorPrincipal.TelefonoEmpleado == "Pendiente"
                ? "Administración"
                : perfilAdministradorPrincipal.TelefonoEmpleado;
            EstadosEmpleado.Aplicar(perfilAdministradorPrincipal, EstadosEmpleado.Activo);
        }

        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID(N'[dbo].[ConsumoMaterial]', N'U') IS NOT NULL
               AND COL_LENGTH(N'dbo.ConsumoMaterial', N'FechaRegistro') IS NULL
            BEGIN
                ALTER TABLE [dbo].[ConsumoMaterial]
                ADD [FechaRegistro] datetime2 NOT NULL
                    CONSTRAINT [DF_ConsumoMaterial_FechaRegistro] DEFAULT (GETDATE());
            END");

        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID(N'[dbo].[Zonas]', N'U') IS NOT NULL
               AND COL_LENGTH(N'dbo.Zonas', N'CodigoDTA') IS NULL
            BEGIN
                ALTER TABLE [dbo].[Zonas]
                ADD [CodigoDTA] nvarchar(20) NOT NULL
                    CONSTRAINT [DF_Zonas_CodigoDTA] DEFAULT (N'');
            END");

        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID(N'[dbo].[Materiales]', N'U') IS NOT NULL
            BEGIN
                UPDATE [dbo].[Materiales]
                SET [Codigo] = UPPER(CASE
                        WHEN PATINDEX(N'%[A-Za-z]%', [Nombre]) > 0 THEN SUBSTRING([Nombre], PATINDEX(N'%[A-Za-z]%', [Nombre]), 1)
                        ELSE N'M'
                    END) + N'-' + RIGHT(N'0000' + CONVERT(nvarchar(10), [IdMaterial]), 4)
                WHERE ([Codigo] IS NULL OR LTRIM(RTRIM([Codigo])) = N'')
                  AND [Nombre] IS NOT NULL AND LTRIM(RTRIM([Nombre])) <> N'';
            END");

        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID(N'[dbo].[Empleados]', N'U') IS NOT NULL
               AND COL_LENGTH(N'dbo.Empleados', N'FotoPerfil') IS NULL
            BEGIN
                ALTER TABLE [dbo].[Empleados] ADD [FotoPerfil] nvarchar(300) NULL;
            END");

        await context.Database.ExecuteSqlRawAsync(@"
            IF OBJECT_ID(N'[dbo].[RevistaPublicaciones]', N'U') IS NULL
            BEGIN
                CREATE TABLE [dbo].[RevistaPublicaciones](
                    [IdPublicacion] int IDENTITY(1,1) NOT NULL PRIMARY KEY,
                    [Titulo] nvarchar(80) NOT NULL,
                    [Descripcion] nvarchar(250) NOT NULL,
                    [Imagen] nvarchar(300) NOT NULL,
                    [TextoEnlace] nvarchar(50) NOT NULL,
                    [Orden] int NOT NULL,
                    [Activo] bit NOT NULL CONSTRAINT [DF_RevistaPublicaciones_Activo] DEFAULT(1)
                );
            END

            IF NOT EXISTS (SELECT 1 FROM [dbo].[RevistaPublicaciones])
            BEGIN
                INSERT INTO [dbo].[RevistaPublicaciones] ([Titulo],[Descripcion],[Imagen],[TextoEnlace],[Orden],[Activo]) VALUES
                (N'Fabricacion a medida',N'Componentes industriales con precision, orden y acabado profesional.',N'/images/Revista/Revista1.jpg',N'Solicitar cotizacion',1,1),
                (N'Instalacion tecnica',N'Montajes limpios para operacion continua y mantenimiento sencillo.',N'/images/Revista/Revista5.jpg',N'Ver servicio',2,1),
                (N'Acabado industrial',N'Detalles funcionales pensados para resistencia, limpieza y durabilidad.',N'/images/Revista/Revista8.jpg',N'Ver detalle',3,1),
                (N'Servicio especializado',N'Diagnostico y ejecucion con criterio tecnico en campo.',N'/images/Revista/Revista2.jpg',N'Coordinar visita',4,1),
                (N'Equipos instalados',N'Integracion sobria para espacios de trabajo exigentes.',N'/images/Revista/Revista10.png',N'Consultar',5,1),
                (N'Mantenimiento',N'Intervenciones ordenadas para conservar rendimiento y seguridad.',N'/images/Revista/Revista3.png',N'Programar',6,1);
            END");

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

static void ConfigureSmtpFromEmailEnvironment(IConfiguration configuration, SmtpOptions options)
{
    options.Host = GetConfiguredValue(configuration, "EMAIL_HOST", options.Host);
    options.FromEmail = GetConfiguredValue(
        configuration,
        "EMAIL_FROM",
        GetConfiguredValue(configuration, "EMAIL_USER", options.FromEmail));
    options.FromName = GetConfiguredValue(configuration, "EMAIL_FROM_NAME", options.FromName);
    options.UserName = GetConfiguredValue(configuration, "EMAIL_USER", options.UserName);
    options.Password = GetConfiguredValue(configuration, "EMAIL_PASSWORD", options.Password);

    var port = configuration["EMAIL_PORT"];
    if (int.TryParse(port, out var parsedPort))
    {
        options.Port = parsedPort;
    }

    var secure = configuration["EMAIL_SECURE"];
    if (bool.TryParse(secure, out var parsedSecure))
    {
        options.EnableSsl = parsedSecure;
    }
}

static string GetConfiguredValue(IConfiguration configuration, string key, string currentValue)
{
    var value = configuration[key];
    return string.IsNullOrWhiteSpace(value) ? currentValue : value;
}
