# Guía Rápida - Próximos Pasos para Completar el Sistema

## 1. Migración de Base de Datos

### Crear y Aplicar Migración
```powershell
# En la consola del administrador de paquetes de Visual Studio
Add-Migration AgregarModulosCampoMaterialesEquiposFabricacion
Update-Database
```

### Datos Iniciales Requeridos

Después de ejecutar la migración, inserte datos en las siguientes tablas:

#### Estados de Cotización
```sql
INSERT INTO EstadosCotizacion (Nombre) VALUES ('Pendiente');
INSERT INTO EstadosCotizacion (Nombre) VALUES ('Aprobada');
INSERT INTO EstadosCotizacion (Nombre) VALUES ('Rechazada');
```

#### Estados de Orden
```sql
INSERT INTO EstadosOrden (Nombre) VALUES ('Pendiente');
INSERT INTO EstadosOrden (Nombre) VALUES ('En Progreso');
INSERT INTO EstadosOrden (Nombre) VALUES ('Completada');
INSERT INTO EstadosOrden (Nombre) VALUES ('Cancelada');
```

#### Tipos de Servicio
```sql
INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Mantenimiento Preventivo', 'Activo');
INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Mantenimiento Correctivo', 'Activo');
INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Instalación', 'Activo');
INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Reparación', 'Activo');
INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Fabricación a Medida', 'Activo');
```

---

## 2. Pruebas de Funcionalidad

### Como Administrador

#### Materiales
1. Navegar a `/Materiales`
2. Crear nuevo material
3. Verificar alerta de bajo stock en `/Materiales/BajoStock`

#### Equipos
1. Navegar a `/Equipos`
2. Registrar equipo de cliente
3. Editar especificaciones

#### Fabricación a Medida
1. Navegar a `/FabricacionAmedida`
2. Crear proyecto para cliente
3. Actualizar estado del proyecto

#### Órdenes de Servicio
1. Navegar a `/Tecnicos`
2. Crear nueva orden asignada a técnico
3. Ver dashboard de órdenes

### Como Técnico (Empleado)
1. Login como empleado
2. Navegar a `/Tecnicos`
3. Ver órdenes asignadas
4. Iniciar orden pendiente
5. Finalizar orden en progreso

---

## 3. Mejoras Inmediatas Recomendadas

### A. Selector de Clientes (Dropdown)

Actualizar formularios para usar dropdown en lugar de input manual:

```csharp
// En el Controller
public async Task<IActionResult> Crear()
{
    ViewBag.Clientes = await _context.Clientes
        .Select(c => new SelectListItem 
        { 
            Value = c.IdCliente.ToString(), 
            Text = c.Nombre 
        }).ToListAsync();
    return View();
}
```

```razor
<!-- En la Vista -->
<select asp-for="ClienteId" asp-items="@ViewBag.Clientes" class="form-select">
    <option value="">Seleccione un cliente...</option>
</select>
```

### B. Selector de Técnicos

Similar al selector de clientes:

```csharp
ViewBag.Tecnicos = await _context.Empleados
    .Where(e => e.EstadoEmpleado == "Activo")
    .Select(e => new SelectListItem 
    { 
        Value = e.IdEmpleado.ToString(), 
        Text = $"{e.NombreEmpleado} {e.ApellidosEmpleado}" 
    }).ToListAsync();
```

### C. Selector de Estados

```csharp
ViewBag.Estados = await _context.EstadosOrden
    .Select(e => new SelectListItem 
    { 
        Value = e.Id.ToString(), 
        Text = e.Nombre 
    }).ToListAsync();
```

---

## 4. Módulo de Consumo de Materiales

### Crear Servicio
```csharp
public interface IConsumoMaterialService
{
    Task<IEnumerable<ConsumoMaterialDTO>> GetByOrdenAsync(int ordenId);
    Task<ConsumoMaterialDTO> CreateAsync(ConsumoMaterialDTO consumoDto);
    Task<bool> DeleteAsync(int id);
}
```

### Agregar a Vista de Orden
En `Views/Tecnicos/Detalle.cshtml`, agregar sección para registrar materiales usados.

---

## 5. Dashboard de Administración

### Crear Controller
```csharp
[Authorize(Roles = "Administrador")]
public class DashboardController : BaseController
{
    public async Task<IActionResult> Index()
    {
        var viewModel = new DashboardViewModel
        {
            TotalMateriales = await _context.Materiales.CountAsync(),
            MaterialesBajoStock = await _context.Materiales
                .Where(m => m.StockActual < m.StockMinimo).CountAsync(),
            OrdenesActivas = await _context.OrdenesServicio
                .Where(o => o.EstadoOrden.Nombre == "En Progreso").CountAsync(),
            ProyectosActivos = await _context.ProyectosFabricacion
                .Where(p => p.Estado == "En Progreso").CountAsync()
        };
        return View(viewModel);
    }
}
```

---

## 6. API REST para Móvil (Opcional)

### Crear API Controller
```csharp
[Route("api/[controller]")]
[ApiController]
public class OrdenesApiController : ControllerBase
{
    private readonly IOrdenServicioService _ordenService;

    [HttpGet("tecnico/{empleadoId}")]
    public async Task<IActionResult> GetOrdenesTecnico(int empleadoId)
    {
        var ordenes = await _ordenService.GetByTecnicoAsync(empleadoId);
        return Ok(ordenes);
    }

    [HttpPost("{id}/iniciar")]
    public async Task<IActionResult> IniciarOrden(int id)
    {
        var result = await _ordenService.IniciarOrdenAsync(id);
        return result ? Ok() : NotFound();
    }
}
```

---

## 7. Reportes en PDF

### Instalar Paquete
```powershell
Install-Package QuestPDF
```

### Servicio de Reportes
```csharp
public interface IReporteService
{
    Task<byte[]> GenerarReporteOrden(int ordenId);
    Task<byte[]> GenerarReporteMateriales();
}
```

---

## 8. Sistema de Notificaciones

### Tabla de Notificaciones (Ya existe en DB)
Implementar servicio para crear notificaciones:

```csharp
public async Task NotificarBajoStock(int materialId)
{
    var notificacion = new Notificacion
    {
        MaterialId = materialId,
        Titulo = "Material Bajo Stock",
        Mensaje = "El material ha alcanzado el stock mínimo",
        Fecha = DateTime.Now,
        Leida = false
    };
    await _context.Notificaciones.AddAsync(notificacion);
    await _context.SaveChangesAsync();
}
```

---

## 9. Validaciones de Negocio Adicionales

### En MaterialService
```csharp
public async Task<MaterialDTO> CreateAsync(MaterialDTO materialDto)
{
    // Validar nombre único
    var existe = await _context.Materiales
        .AnyAsync(m => m.Nombre == materialDto.Nombre);
    if (existe)
        throw new Exception("Ya existe un material con ese nombre");

    // ... resto del código
}
```

### En OrdenServicioService
```csharp
public async Task<bool> IniciarOrdenAsync(int id)
{
    var orden = await _context.OrdenesServicio.FindAsync(id);
    if (orden == null) return false;

    // Validar que esté en estado Pendiente
    var estadoPendiente = await _context.EstadosOrden
        .FirstOrDefaultAsync(e => e.Nombre == "Pendiente");
    if (orden.EstadoOrdenId != estadoPendiente?.Id)
        throw new Exception("Solo se pueden iniciar órdenes pendientes");

    // ... resto del código
}
```

---

## 10. Testing

### Crear Proyecto de Pruebas
```powershell
dotnet new xunit -n MultiservicioB.Tests
dotnet add reference ..\MultiservicioB\MultiservicioB.csproj
```

### Ejemplo de Prueba
```csharp
public class MaterialServiceTests
{
    [Fact]
    public async Task CreateMaterial_DebeCrearCorrectamente()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        using var context = new ApplicationDbContext(options);
        var service = new MaterialService(context);

        var materialDto = new MaterialDTO
        {
            Nombre = "Material Test",
            StockActual = 100,
            StockMinimo = 10
        };

        // Act
        var result = await service.CreateAsync(materialDto);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IdMaterial > 0);
    }
}
```

---

## 11. Checklist de Completitud

### Funcionalidades Core ✅
- [x] Materiales CRUD
- [x] Equipos CRUD
- [x] Fabricación a Medida CRUD
- [x] Órdenes de Servicio CRUD
- [x] Dashboard Técnico
- [x] Autorización por Roles

### Pendientes Recomendadas
- [ ] Selectores de Clientes/Técnicos (Dropdowns)
- [ ] Consumo de Materiales en Órdenes
- [ ] Historial de Equipos
- [ ] Cotizaciones Completas
- [ ] Dashboard Administrativo
- [ ] Reportes PDF
- [ ] Sistema de Notificaciones
- [ ] Paginación en Listados
- [ ] Filtros y Búsqueda
- [ ] API REST
- [ ] Pruebas Unitarias

---

## 12. Comandos Útiles

### Entity Framework
```powershell
# Ver migraciones pendientes
Get-Migration

# Revertir última migración
Update-Database -Migration <NombreMigrationAnterior>

# Generar script SQL
Script-Migration
```

### Limpiar y Reconstruir
```powershell
dotnet clean
dotnet build
```

### Ejecutar la Aplicación
```powershell
dotnet run
```

---

## Contacto y Soporte

Para preguntas sobre la implementación:
- Revisar el archivo `SPRINT1_IMPLEMENTACION_RESUMEN.md`
- Consultar el esquema de base de datos en `Scripts de Base de datos/script inicial para login y empleados.sql`
- Verificar la estructura del proyecto en el Solution Explorer

**¡El sistema está listo para pruebas y desarrollo adicional!** 🚀
