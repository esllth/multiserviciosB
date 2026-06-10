USE [MultiservicioDB];
GO

BEGIN TRANSACTION;

IF COL_LENGTH('dbo.Empleados', 'EstadoAcceso') IS NULL
BEGIN
    ALTER TABLE [dbo].[Empleados]
        ADD [EstadoAcceso] NVARCHAR(30) NOT NULL
            CONSTRAINT [DF_Empleados_EstadoAcceso] DEFAULT N'PendienteRegistro';
END;

EXEC sys.sp_executesql N'
    UPDATE [dbo].[Empleados]
    SET [EstadoAcceso] =
        CASE
            WHEN [TieneUsuario] = 1 AND [EstadoAcceso] = N''PendienteRegistro'' THEN N''Aprobado''
            ELSE [EstadoAcceso]
        END;';

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE [object_id] = OBJECT_ID(N'dbo.OrdenesServicio')
      AND [name] = N'EmpleadoId'
      AND [is_nullable] = 0
)
BEGIN
    ALTER TABLE [dbo].[OrdenesServicio]
        ALTER COLUMN [EmpleadoId] INT NULL;
END;

COMMIT TRANSACTION;
GO

SELECT
    COL_LENGTH('dbo.Empleados', 'EstadoAcceso') AS LongitudEstadoAcceso,
    COLUMNPROPERTY(OBJECT_ID('dbo.OrdenesServicio'), 'EmpleadoId', 'AllowsNull') AS EmpleadoIdPermiteNull;
GO



-- Fix EstadoEmpleado column type mismatch
-- Step 1: Normalize existing data to 0 or 1
UPDATE [Empleados] 
SET [EstadoEmpleado] = CASE 
    WHEN [EstadoEmpleado] IN (N'Activo', N'True', N'true', N'1', N'activo') THEN N'1'
    WHEN [EstadoEmpleado] IN (N'Inactivo', N'False', N'false', N'0', N'inactivo') THEN N'0'
    ELSE N'1' -- Default to active if unclear
END
WHERE [EstadoEmpleado] NOT IN (N'0', N'1');

-- Step 2: Change column type from nvarchar(max) to bit
ALTER TABLE [Empleados]
ALTER COLUMN [EstadoEmpleado] bit NOT NULL;



--ver tablas
SELECT name FROM sys.tables ORDER BY name;
 
SELECT * FROM [__EFMigrationsHistory] ORDER BY MigrationId;


-- Script para crear tablas de Configuración, Horarios y Zonas
-- SQL Server Management Studio (SSMS)

-- Tabla ConfiguracionSistema
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionSistema')
BEGIN
    CREATE TABLE [dbo].[ConfiguracionSistema] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Clave] NVARCHAR(100) NOT NULL,
        [Valor] NVARCHAR(255) NOT NULL,
        [Descripcion] NVARCHAR(255) NULL,
        CONSTRAINT [PK_ConfiguracionSistema] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla ConfiguracionSistema creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla ConfiguracionSistema ya existe.';
END
GO

-- Tabla Horarios
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Horarios')
BEGIN
    CREATE TABLE [dbo].[Horarios] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [DiaSemana] NVARCHAR(20) NOT NULL,
        [HoraInicio] TIME NOT NULL,
        [HoraFin] TIME NOT NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Horarios] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla Horarios creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Horarios ya existe.';
END
GO

-- Tabla Zonas
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Zonas')
BEGIN
    CREATE TABLE [dbo].[Zonas] (
        [Id] INT IDENTITY(1,1) NOT NULL,
        [Provincia] NVARCHAR(100) NOT NULL,
        [Canton] NVARCHAR(100) NOT NULL,
        [Distrito] NVARCHAR(100) NOT NULL,
        [Descripcion] NVARCHAR(255) NULL,
        [Activo] BIT NOT NULL DEFAULT 1,
        CONSTRAINT [PK_Zonas] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
    PRINT 'Tabla Zonas creada exitosamente.';
END
ELSE
BEGIN
    PRINT 'La tabla Zonas ya existe.';
END
GO

-- Verificar que todas las tablas se crearon correctamente
SELECT 
    'ConfiguracionSistema' AS Tabla,
    CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionSistema') 
         THEN 'Existe' 
         ELSE 'No existe' 
    END AS Estado
UNION ALL
SELECT 
    'Horarios',
    CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'Horarios') 
         THEN 'Existe' 
         ELSE 'No existe' 
    END
UNION ALL
SELECT 
    'Zonas',
    CASE WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'Zonas') 
         THEN 'Existe' 
         ELSE 'No existe' 
    END;
GO