# Guía de Ajustes de Base de Datos - Sprints 2 y 3
## Proyecto Multiservicios B

### Resumen de Cambios Implementados

Se han creado **6 nuevas entidades** y se han **actualizado 5 entidades existentes** para soportar las historias de usuario de los módulos:
- **RT (Técnico/Campo)**: 10 historias
- **RM (Materiales)**: 10 historias  
- **RE (Equipos)**: 10 historias
- **RF (Fabricación a Medida)**: 10 historias

---

## 🆕 NUEVAS TABLAS CREADAS

### 1. **FotoOrdenServicio** (RT-004)
Almacena evidencia fotográfica obligatoria de órdenes de servicio.

**Campos:**
- `IdFotoOrden` (PK)
- `OrdenId` (FK a OrdenServicio)
- `Ruta` (string 260)
- `NombreOriginal` (string 150)
- `TipoContenido` (string 50)
- `TipoFoto` (string 20) - "Inicial" o "Final"
- `FechaCarga` (DateTime)
- `Descripcion` (string 500, nullable)

**Índices:** FK en `OrdenId` con `ON DELETE CASCADE`

---

### 2. **EventoOrdenServicio** (RT-001, RT-002, RT-003, RT-006, RT-007, RT-008)
Registra todos los eventos de una orden (llegada, inicio, observaciones, finalización, aceptación).

**Campos:**
- `IdEvento` (PK)
- `OrdenId` (FK a OrdenServicio)
- `TipoEvento` (string 50) - "LlegadaSitio", "InicioServicio", "ObservacionTecnica", "FinalizacionServicio", "ComentarioFinal", "AceptacionCliente"
- `FechaEvento` (DateTime)
- `Descripcion` (string 1000, nullable)
- `Latitud` (decimal 10,7, nullable) - GPS
- `Longitud` (decimal 10,7, nullable) - GPS
- `UsuarioId` (string 450, nullable)

**Índices:** FK en `OrdenId` con `ON DELETE CASCADE`

---

### 3. **SolicitudMaterial** (RM-007)
Gestiona solicitudes de materiales faltantes desde campo.

**Campos:**
- `IdSolicitud` (PK)
- `OrdenId` (FK a OrdenServicio)
- `MaterialId` (FK a Material)
- `EmpleadoId` (FK a Empleado)
- `CantidadSolicitada` (decimal 10,2)
- `FechaSolicitud` (DateTime)
- `Estado` (string 20) - "Pendiente", "Aprobada", "Rechazada", "Entregada"
- `Justificacion` (string 500, nullable)
- `RespuestaAdmin` (string 500, nullable)
- `FechaRespuesta` (DateTime, nullable)

**Índices:** FKs con `ON DELETE RESTRICT`

---

### 4. **AlertaMantenimiento** (RE-009, RE-010)
Alertas de mantenimiento preventivo programado.

**Campos:**
- `IdAlerta` (PK)
- `EquipoId` (FK a Equipo)
- `FechaMantenimiento` (DateTime)
- `TipoMantenimiento` (string 50) - "Preventivo", "Correctivo", "Calibración"
- `Descripcion` (string 500, nullable)
- `Estado` (string 20) - "Pendiente", "Notificada", "Realizada", "Cancelada"
- `FechaCreacion` (DateTime)
- `FechaNotificacion` (DateTime, nullable)
- `FechaRealizacion` (DateTime, nullable)

**Índices:** FK en `EquipoId` con `ON DELETE CASCADE`

---

### 5. **DocumentoFabricacion** (RF-002, RF-008)
Documentos de diseño y especificaciones de proyectos de fabricación.

**Campos:**
- `IdDocumento` (PK)
- `ProyectoId` (FK a ProyectoFabricacion)
- `NombreDocumento` (string 100)
- `TipoDocumento` (string 50) - "Diseño", "Especificaciones", "Plano", "Otro"
- `Ruta` (string 260)
- `Descripcion` (string 500, nullable)
- `FechaCarga` (DateTime)
- `CargadoPorUsuarioId` (string 450, nullable)

**Índices:** FK en `ProyectoId` con `ON DELETE CASCADE`

---

### 6. **MaterialProyectoFabricacion** (RF-004)
Materiales requeridos por proyecto de fabricación.

**Campos:**
- `IdMaterialProyecto` (PK)
- `ProyectoId` (FK a ProyectoFabricacion)
- `MaterialId` (FK a Material)
- `CantidadRequerida` (decimal 10,2)
- `CantidadUsada` (decimal 10,2, nullable)
- `Observaciones` (string 500, nullable)

**Índices:** 
- FK `ProyectoId` con `ON DELETE CASCADE`
- FK `MaterialId` con `ON DELETE RESTRICT`


-- ============================================
-- Tabla: FotoOrden
-- ============================================
CREATE TABLE FotoOrden (
    IdFotoOrden INT IDENTITY(1,1) PRIMARY KEY,
    OrdenId INT NOT NULL,
    Ruta NVARCHAR(260) NOT NULL,
    NombreOriginal NVARCHAR(150) NOT NULL,
    TipoContenido NVARCHAR(50) NOT NULL,
    TipoFoto NVARCHAR(20) NOT NULL CHECK (TipoFoto IN ('Inicial', 'Final')),
    FechaCarga DATETIME NOT NULL DEFAULT GETDATE(),
    Descripcion NVARCHAR(500) NULL
);

CREATE INDEX IX_FotoOrden_OrdenId ON FotoOrden(OrdenId);

ALTER TABLE FotoOrden ADD CONSTRAINT FK_FotoOrden_OrdenServicio 
    FOREIGN KEY (OrdenId) REFERENCES OrdenServicio(IdOrden) ON DELETE CASCADE;

-- ============================================
-- Tabla: EventoOrdenServicio
-- ============================================
CREATE TABLE EventoOrdenServicio (
    IdEvento INT IDENTITY(1,1) PRIMARY KEY,
    OrdenId INT NOT NULL,
    TipoEvento NVARCHAR(50) NOT NULL CHECK (TipoEvento IN ('LlegadaSitio', 'InicioServicio', 'ObservacionTecnica', 'FinalizacionServicio', 'ComentarioFinal', 'AceptacionCliente')),
    FechaEvento DATETIME NOT NULL DEFAULT GETDATE(),
    Descripcion NVARCHAR(1000) NULL,
    Latitud DECIMAL(10,7) NULL,
    Longitud DECIMAL(10,7) NULL,
    UsuarioId NVARCHAR(450) NULL
);

CREATE INDEX IX_EventoOrdenServicio_OrdenId ON EventoOrdenServicio(OrdenId);

ALTER TABLE EventoOrdenServicio ADD CONSTRAINT FK_EventoOrdenServicio_OrdenServicio 
    FOREIGN KEY (OrdenId) REFERENCES OrdenServicio(IdOrden) ON DELETE CASCADE;

-- ============================================
-- Tabla: SolicitudMaterial
-- ============================================
CREATE TABLE SolicitudMaterial (
    IdSolicitud INT IDENTITY(1,1) PRIMARY KEY,
    OrdenId INT NOT NULL,
    MaterialId INT NOT NULL,
    EmpleadoId INT NOT NULL,
    CantidadSolicitada DECIMAL(10,2) NOT NULL CHECK (CantidadSolicitada > 0),
    FechaSolicitud DATETIME NOT NULL DEFAULT GETDATE(),
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente' CHECK (Estado IN ('Pendiente', 'Aprobada', 'Rechazada', 'Entregada')),
    Justificacion NVARCHAR(500) NULL,
    RespuestaAdmin NVARCHAR(500) NULL,
    FechaRespuesta DATETIME NULL
);

CREATE INDEX IX_SolicitudMaterial_OrdenId ON SolicitudMaterial(OrdenId);
CREATE INDEX IX_SolicitudMaterial_MaterialId ON SolicitudMaterial(MaterialId);
CREATE INDEX IX_SolicitudMaterial_EmpleadoId ON SolicitudMaterial(EmpleadoId);

ALTER TABLE SolicitudMaterial ADD CONSTRAINT FK_SolicitudMaterial_OrdenServicio 
    FOREIGN KEY (OrdenId) REFERENCES OrdenServicio(IdOrden) ON DELETE RESTRICT;

ALTER TABLE SolicitudMaterial ADD CONSTRAINT FK_SolicitudMaterial_Material 
    FOREIGN KEY (MaterialId) REFERENCES Material(IdMaterial) ON DELETE RESTRICT;

ALTER TABLE SolicitudMaterial ADD CONSTRAINT FK_SolicitudMaterial_Empleado 
    FOREIGN KEY (EmpleadoId) REFERENCES Empleado(IdEmpleado) ON DELETE RESTRICT;

-- ============================================
-- Tabla: AlertaMantenimiento
-- ============================================
CREATE TABLE AlertaMantenimiento (
    IdAlerta INT IDENTITY(1,1) PRIMARY KEY,
    EquipoId INT NOT NULL,
    FechaMantenimiento DATETIME NOT NULL,
    TipoMantenimiento NVARCHAR(50) NOT NULL CHECK (TipoMantenimiento IN ('Preventivo', 'Correctivo', 'Calibración')),
    Descripcion NVARCHAR(500) NULL,
    Estado NVARCHAR(20) NOT NULL DEFAULT 'Pendiente' CHECK (Estado IN ('Pendiente', 'Notificada', 'Realizada', 'Cancelada')),
    FechaCreacion DATETIME NOT NULL DEFAULT GETDATE(),
    FechaNotificacion DATETIME NULL,
    FechaRealizacion DATETIME NULL
);

CREATE INDEX IX_AlertaMantenimiento_EquipoId ON AlertaMantenimiento(EquipoId);

ALTER TABLE AlertaMantenimiento ADD CONSTRAINT FK_AlertaMantenimiento_Equipo 
    FOREIGN KEY (EquipoId) REFERENCES Equipo(IdEquipo) ON DELETE CASCADE;

-- ============================================
-- Tabla: DocumentoFabricacion
-- ============================================
CREATE TABLE DocumentoFabricacion (
    IdDocumento INT IDENTITY(1,1) PRIMARY KEY,
    ProyectoId INT NOT NULL,
    NombreDocumento NVARCHAR(100) NOT NULL,
    TipoDocumento NVARCHAR(50) NOT NULL CHECK (TipoDocumento IN ('Diseño', 'Especificaciones', 'Plano', 'Otro')),
    Ruta NVARCHAR(260) NOT NULL,
    Descripcion NVARCHAR(500) NULL,
    FechaCarga DATETIME NOT NULL DEFAULT GETDATE(),
    CargadoPorUsuarioId NVARCHAR(450) NULL
);

CREATE INDEX IX_DocumentoFabricacion_ProyectoId ON DocumentoFabricacion(ProyectoId);

ALTER TABLE DocumentoFabricacion ADD CONSTRAINT FK_DocumentoFabricacion_ProyectoFabricacion 
    FOREIGN KEY (ProyectoId) REFERENCES ProyectoFabricacion(IdProyecto) ON DELETE CASCADE;

-- ============================================
-- Tabla: MaterialProyectoFabricacion
-- ============================================
CREATE TABLE MaterialProyectoFabricacion (
    IdMaterialProyecto INT IDENTITY(1,1) PRIMARY KEY,
    ProyectoId INT NOT NULL,
    MaterialId INT NOT NULL,
    CantidadRequerida DECIMAL(10,2) NOT NULL CHECK (CantidadRequerida > 0),
    CantidadUsada DECIMAL(10,2) NULL CHECK (CantidadUsada >= 0),
    Observaciones NVARCHAR(500) NULL
);

CREATE INDEX IX_MaterialProyectoFabricacion_ProyectoId ON MaterialProyectoFabricacion(ProyectoId);
CREATE INDEX IX_MaterialProyectoFabricacion_MaterialId ON MaterialProyectoFabricacion(MaterialId);

ALTER TABLE MaterialProyectoFabricacion ADD CONSTRAINT FK_MaterialProyectoFabricacion_ProyectoFabricacion 
    FOREIGN KEY (ProyectoId) REFERENCES ProyectoFabricacion(IdProyecto) ON DELETE CASCADE;

ALTER TABLE MaterialProyectoFabricacion ADD CONSTRAINT FK_MaterialProyectoFabricacion_Material 
    FOREIGN KEY (MaterialId) REFERENCES Material(IdMaterial) ON DELETE RESTRICT;



---

## 🔄 TABLAS MODIFICADAS (ALTER TABLE)

### 1. **OrdenServicio** - 8 nuevos campos

```sql
ALTER TABLE OrdenesServicio ADD
    FechaLlegadaSitio datetime2 NULL,
    FechaAceptacionCliente datetime2 NULL,
    ObservacionesTecnicas nvarchar(2000) NULL,
    ComentariosFinales nvarchar(1000) NULL,
    RequiereFotosObligatorias bit NOT NULL DEFAULT 1,
    LlegadaConfirmada bit NOT NULL DEFAULT 0;
```

**Nota:** `FechaInicio` y `FechaFin` ya existen, se mantienen.

---

### 2. **Material** - 4 nuevos campos

```sql
ALTER TABLE Materiales ADD
    Codigo nvarchar(50) NULL,
    Categoria nvarchar(100) NULL,
    AlertaStockActiva bit NOT NULL DEFAULT 1,
    Estado nvarchar(20) NOT NULL DEFAULT 'Activo';
```

**Nota:** `Descripcion` cambia de 255 a 1000 caracteres.

---

### 3. **Equipo** - 10 nuevos campos

```sql
ALTER TABLE Equipos ADD
    Codigo nvarchar(50) NULL,
    TipoEquipo nvarchar(100) NULL,
    Marca nvarchar(100) NULL,
    Modelo nvarchar(100) NULL,
    NumeroSerie nvarchar(100) NULL,
    FechaAdquisicion datetime2 NULL,
    FrecuenciaMantenimientoDias int NULL,
    UltimoMantenimiento datetime2 NULL,
    ProximoMantenimiento datetime2 NULL,
    Observaciones nvarchar(1000) NULL;

ALTER TABLE Equipos ALTER COLUMN Nombre nvarchar(100) NOT NULL;
ALTER TABLE Equipos ALTER COLUMN Estado nvarchar(30) NOT NULL DEFAULT 'Operativo';
ALTER TABLE Equipos ALTER COLUMN Especificaciones nvarchar(2000) NULL;
```

---

### 4. **ProyectoFabricacion** - Rediseño completo (12 nuevos campos)

```sql
ALTER TABLE ProyectosFabricacion ADD
    NombreProyecto nvarchar(200) NOT NULL DEFAULT '',
    FechaSolicitud datetime2 NOT NULL DEFAULT GETDATE(),
    FechaInicioEstimada datetime2 NULL,
    FechaFinEstimada datetime2 NULL,
    FechaInicioReal datetime2 NULL,
    FechaFinReal datetime2 NULL,
    CostoEstimado decimal(12,2) NULL,
    CostoReal decimal(12,2) NULL,
    DiseñoAprobado bit NOT NULL DEFAULT 0,
    FechaAprobacionDiseño datetime2 NULL,
    ObservacionesCliente nvarchar(1000) NULL,
    ObservacionesInternas nvarchar(1000) NULL;

-- DEPRECADOS (eliminar si no tienen datos):
-- FechaInicio -> reemplazado por FechaInicioEstimada/FechaInicioReal
-- FechaFin -> reemplazado por FechaFinEstimada/FechaFinReal

ALTER TABLE ProyectosFabricacion ALTER COLUMN Estado nvarchar(30) NOT NULL;
ALTER TABLE ProyectosFabricacion ALTER COLUMN Descripcion nvarchar(1000) NULL;
```

---

### 5. **HistorialEquipo** - 3 nuevos campos

```sql
ALTER TABLE HistorialEquipos ADD
    TipoServicio nvarchar(50) NULL,
    EstadoAnterior nvarchar(30) NULL,
    EstadoPosterior nvarchar(30) NULL,
    ObservacionesTecnico nvarchar(1000) NULL;

ALTER TABLE HistorialEquipos ALTER COLUMN Descripcion nvarchar(2000) NULL;
```

---

### 6. **ConsumoMaterial** - 1 nuevo campo

```sql
ALTER TABLE ConsumosMaterial ADD
    FechaRegistro datetime2 NOT NULL DEFAULT GETDATE();
```

---

## 📋 PASOS PARA APLICAR LOS CAMBIOS

### Opción 1: Aplicar migración automáticamente (Recomendado)

```powershell
# En la raíz del proyecto
dotnet ef database update --project MultiservicioB.csproj
```

Este comando:
- ✅ Crea todas las nuevas tablas
- ✅ Modifica las tablas existentes
- ✅ Crea todos los índices y relaciones
- ✅ Es reversible con `dotnet ef database update <MigracionAnterior>`

---

### Opción 2: Generar script SQL y aplicar manualmente

Una vez que las vistas estén corregidas y el proyecto compile:

```powershell
# Generar script SQL
dotnet ef migrations script --idempotent --output migration_script.sql

# Luego ejecutar el script en SQL Server Management Studio o sqlcmd
```

---

## ⚠️ CONSIDERACIONES IMPORTANTES

### 1. **Backup de Base de Datos**
Antes de aplicar la migración:

```sql
BACKUP DATABASE [MultiservicioDB] 
TO DISK = 'C:\Backups\MultiservicioDB_PreSprints2y3.bak'
WITH FORMAT, INIT, COMPRESSION;
```




---

### 2. **Datos Existentes**

#### Proyectos de Fabricación
Si tienes proyectos existentes, necesitarás script de migración de datos:

```sql
-- Migrar fechas antiguas a nuevas (si aplica)
UPDATE ProyectosFabricacion 
SET FechaInicioEstimada = FechaInicio,
    FechaFinEstimada = FechaFin,
    FechaSolicitud = ISNULL(FechaCreacion, GETDATE()),
    NombreProyecto = 'Proyecto ' + CAST(IdProyecto AS VARCHAR);
```

#### Órdenes de Servicio
Las órdenes existentes tendrán valores por defecto:
- `RequiereFotosObligatorias = true`
- `LlegadaConfirmada = false`

#### Materiales y Equipos
- Estado por defecto: "Activo" / "Operativo"
- AlertaStockActiva = true

---

### 3. **Índices de Rendimiento Adicionales (Opcional)**

Para optimizar consultas frecuentes:

```sql
-- Eventos de orden por tipo
CREATE NONCLUSTERED INDEX IX_EventoOrdenServicio_TipoEvento 
ON EventosOrdenServicio(TipoEvento, FechaEvento DESC);

-- Solicitudes pendientes
CREATE NONCLUSTERED INDEX IX_SolicitudMaterial_Estado 
ON SolicitudesMaterial(Estado, FechaSolicitud DESC);

-- Alertas próximas
CREATE NONCLUSTERED INDEX IX_AlertaMantenimiento_FechaEstado 
ON AlertasMantenimiento(FechaMantenimiento, Estado);

-- Materiales bajo stock
CREATE NONCLUSTERED INDEX IX_Material_Stock 
ON Materiales(Estado, StockActual, StockMinimo)
WHERE AlertaStockActiva = 1;
```

---

## 🎯 VALIDACIÓN POST-MIGRACIÓN

Después de aplicar los cambios, ejecuta estas validaciones:

```sql
-- 1. Verificar nuevas tablas
SELECT name FROM sys.tables 
WHERE name IN (
    'FotosOrdenServicio',
    'EventosOrdenServicio',
    'SolicitudesMaterial',
    'AlertasMantenimiento',
    'DocumentosFabricacion',
    'MaterialesProyectoFabricacion'
);

-- 2. Verificar nuevas columnas en OrdenServicio
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'OrdenesServicio'
AND COLUMN_NAME IN (
    'FechaLlegadaSitio',
    'ObservacionesTecnicas',
    'LlegadaConfirmada'
);

-- 3. Verificar integridad referencial
SELECT 
    fk.name AS FK_Name,
    tp.name AS Parent_Table,
    tr.name AS Referenced_Table
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
WHERE tp.name LIKE '%Orden%' OR tp.name LIKE '%Material%';

-- 4. Contar registros (debe ser 0 en nuevas tablas)
SELECT 
    'FotosOrdenServicio' as Tabla, COUNT(*) as Total FROM FotosOrdenServicio
UNION ALL
SELECT 'EventosOrdenServicio', COUNT(*) FROM EventosOrdenServicio
UNION ALL
SELECT 'SolicitudesMaterial', COUNT(*) FROM SolicitudesMaterial
UNION ALL
SELECT 'AlertasMantenimiento', COUNT(*) FROM AlertasMantenimiento;
```

---

## 📊 ESTIMACIÓN DE TAMAÑO

Estimación de crecimiento de base de datos:

| Tabla | Registros/Mes | Tamaño Aprox/Registro | Crecimiento Anual |
|-------|---------------|----------------------|-------------------|
| FotoOrdenServicio | ~1,000 | 1 KB (metadatos) | 12 MB |
| EventoOrdenServicio | ~5,000 | 0.5 KB | 30 MB |
| SolicitudMaterial | ~200 | 1 KB | 2.4 MB |
| AlertaMantenimiento | ~300 | 0.5 KB | 1.8 MB |
| DocumentoFabricacion | ~100 | 1 KB | 1.2 MB |

**Total estimado:** ~50 MB/año (sin archivos adjuntos)

---

## 🔐 PERMISOS NECESARIOS

El usuario de base de datos requiere:

```sql
GRANT CREATE TABLE TO [UsuarioApp];
GRANT ALTER ON SCHEMA::dbo TO [UsuarioApp];
GRANT REFERENCES ON SCHEMA::dbo TO [UsuarioApp];
```

---

## 📞 SOPORTE

Si encuentras errores durante la migración:

1. **Revisa el log de Entity Framework:**
   ```powershell
   $env:ASPNETCORE_ENVIRONMENT="Development"
   dotnet ef database update --verbose
   ```

2. **Rollback si es necesario:**
   ```powershell
   dotnet ef database update <NombreMigracionAnterior>
   ```

3. **Recrea la migración si hay problemas:**
   ```powershell
   dotnet ef migrations remove
   # Corregir modelos
   dotnet ef migrations add NuevaMigracion
   ```

---

## ✅ CHECKLIST DE IMPLEMENTACIÓN

- [ ] Backup de base de datos realizado
- [ ] Código compilado exitosamente
- [ ] Migración creada: `ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion`
- [ ] Revisado script SQL generado
- [ ] Aplicada migración: `dotnet ef database update`
- [ ] Validaciones SQL ejecutadas
- [ ] Índices adicionales creados (opcional)
- [ ] Datos existentes migrados (si aplica)
- [ ] Pruebas básicas en cada módulo
- [ ] Documentación actualizada

---

**Fecha de creación:** ${new Date().toLocaleDateString('es-ES')}
**Versión:** Sprints 2 y 3 - Módulos RT, RM, RE, RF
**Estado:** Listo para aplicación en entorno de desarrollo
