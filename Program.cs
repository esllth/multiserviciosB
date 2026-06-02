using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Services;
using MultiservicioB.Services.Interfaces;
using MultiservicioB.Models;

var builder = WebApplication.CreateBuilder(args);

// Base de datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders() // necesario para Identity completo
// Descripciones de error en español
.AddErrorDescriber<SpanishIdentityErrorDescriber>();

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
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Rutas MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 🔴 IMPORTANTE: esto habilita Identity (Areas)
app.MapRazorPages();

app.Run();
