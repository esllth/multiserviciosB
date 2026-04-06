using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1. CONFIGURAR BASE DE DATOS
// ============================

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ============================
// 2. CONFIGURAR IDENTITY
// ============================
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// ============================
// 3. MVC + RAZOR
// ============================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// ============================
// 4. CREAR ROLES + ASIGNAR ADMIN
// ============================
// ⚠️ TODO lo siguiente corre al iniciar la app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

    // ----------------------------
    // 4.1 CREAR ROLES
    // ----------------------------
    string[] roles = { "Administrador", "Empleado", "Cliente" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // ----------------------------
    // 4.2 VERIFICAR SI YA EXISTE ADMIN
    // ----------------------------
    var users = userManager.Users.ToList();
    bool adminExists = false;

    foreach (var u in users)
    {
        if (await userManager.IsInRoleAsync(u, "Administrador"))
        {
            adminExists = true;
            break;
        }
    }

    // ----------------------------
    // 4.3 SI NO HAY ADMIN:
    // BUSCAR USUARIO CON DOMINIO
    // ----------------------------
    if (!adminExists)
    {
        var candidate = users
            .Where(u => u.Email != null && u.Email.EndsWith("@multiserviciosb.com"))
            .FirstOrDefault();

        // ----------------------------
        // 4.4 ASIGNAR ROL ADMIN
        // ----------------------------
        if (candidate != null)
        {
            var rolesUser = await userManager.GetRolesAsync(candidate);

            if (!rolesUser.Contains("Administrador"))
            {
                await userManager.AddToRoleAsync(candidate, "Administrador");
            }
        }
    }
}

// ============================
// 5. MIDDLEWARE
// ============================
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

// ============================
// 6. RUTAS
// ============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();