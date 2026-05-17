----- CREAR BASE DE DATOS -----
CREATE DATABASE MultiservicioDB
GO

----- USAR BASE DE DATOS -----
USE MultiservicioDB
GO

----- IDENTITY -----
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)

----- IDENTITY -----
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetRoleClaims] (
        [Id] int NOT NULL IDENTITY,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetUserClaims] (
        [Id] int NOT NULL IDENTITY,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(128) NOT NULL,
        [ProviderKey] nvarchar(128) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(128) NOT NULL,
        [Name] nvarchar(128) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260406034846_InitialClean'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260406034846_InitialClean', N'10.0.4');
END;

COMMIT;
GO

	---INSERTAR DATOS DE ROLES
	INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[Name])
     VALUES
           (NEWID()
           ,'Administrador')
GO

INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[Name])
     VALUES
           (NEWID()
           ,'Empleado')
GO

INSERT INTO [dbo].[AspNetRoles]
           ([Id]
           ,[Name])
     VALUES
           (NEWID()
           ,'Cliente')
GO

---===CREACION DE TABLAS===---
----- TABLAS DEL MODELO ER -----

-- Direcciones y Ubicación
CREATE TABLE UbicacionDTA (
    Id INT IDENTITY PRIMARY KEY,
    IdProvincia INT NOT NULL,
    Provincia NVARCHAR(100) NOT NULL,
    IdCanton INT NOT NULL,
    Canton NVARCHAR(100) NOT NULL,
    IdDistrito INT NOT NULL,
    Distrito NVARCHAR(100) NOT NULL,
    CodigoDTA NVARCHAR(20) NOT NULL
);

CREATE TABLE Direccion (
    Id INT IDENTITY PRIMARY KEY,
    UbicacionDTAId INT NOT NULL,
    OtrasSenas NVARCHAR(255),
    FOREIGN KEY (UbicacionDTAId) REFERENCES UbicacionDTA(Id)
);

-- Clientes
CREATE TABLE Clientes (
    IdCliente INT IDENTITY PRIMARY KEY,
    Identificacion NVARCHAR(50) NOT NULL,
    Nombre NVARCHAR(100) NOT NULL,
    Apellidos NVARCHAR(100),
    Correo NVARCHAR(150) UNIQUE,
    Telefono NVARCHAR(20),
    DireccionId INT,
    Estado NVARCHAR(20),
    FOREIGN KEY (DireccionId) REFERENCES Direccion(Id)
);

-- Empleados
CREATE TABLE Empleados (
    IdEmpleado INT IDENTITY PRIMARY KEY,
    IdentificacionEmpleado NVARCHAR(50) NOT NULL,
    NombreEmpleado NVARCHAR(100) NOT NULL,
    ApellidosEmpleado NVARCHAR(100) NOT NULL,
    CorreoElectronicoEmpleado NVARCHAR(150) NOT NULL,
    TelefonoEmpleado NVARCHAR(20) NOT NULL,
    DireccionId INT NOT NULL,
    EstadoEmpleado NVARCHAR(20) NOT NULL,
    TieneUsuario BIT NOT NULL DEFAULT 0,
    SalarioBase DECIMAL(10,2) NOT NULL,
    FechaInicioEmpleado DATE NOT NULL,
    FechaFinalizacionEmpleado DATE NULL,
    UserId NVARCHAR(450) NULL, -- FK hacia AspNetUsers
    FOREIGN KEY (DireccionId) REFERENCES Direccion(Id),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id)
);

-- Estados y Tipos
CREATE TABLE EstadoCotizacion (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

CREATE TABLE EstadoOrden (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(50) NOT NULL
);

CREATE TABLE TipoServicio (
    Id INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Estado NVARCHAR(20)
);

-- Cotizaciones y Órdenes
CREATE TABLE Cotizaciones (
    IdCotizacion INT IDENTITY PRIMARY KEY,
    ClienteId INT NOT NULL,
    TipoServicioId INT NOT NULL,
    EstadoCotizacionId INT NOT NULL,
    Descripcion NVARCHAR(255),
    MontoPresupuesto DECIMAL(12,2),
    FechaSolicitud DATE,
    AprobadaPorCliente BIT DEFAULT 0,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(IdCliente),
    FOREIGN KEY (TipoServicioId) REFERENCES TipoServicio(Id),
    FOREIGN KEY (EstadoCotizacionId) REFERENCES EstadoCotizacion(Id)
);

CREATE TABLE OrdenesServicio (
    IdOrden INT IDENTITY PRIMARY KEY,
    CotizacionId INT NOT NULL,
    ClienteId INT NOT NULL,
    EmpleadoId INT NOT NULL,
    FechaCreacion DATETIME DEFAULT GETDATE(),
    FechaInicio DATETIME,
    FechaFin DATETIME,
    EstadoOrdenId INT NOT NULL,
    FOREIGN KEY (CotizacionId) REFERENCES Cotizaciones(IdCotizacion),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(IdCliente),
    FOREIGN KEY (EmpleadoId) REFERENCES Empleados(IdEmpleado),
    FOREIGN KEY (EstadoOrdenId) REFERENCES EstadoOrden(Id)
);

-- Materiales y Consumo
CREATE TABLE Materiales (
    IdMaterial INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    UnidadMedida NVARCHAR(50),
    StockActual INT,
    StockMinimo INT,
    PrecioUnitario DECIMAL(10,2)
);

CREATE TABLE ConsumoMaterial (
    IdConsumo INT IDENTITY PRIMARY KEY,
    OrdenId INT NOT NULL,
    MaterialId INT NOT NULL,
    CantidadUsada DECIMAL(10,2),
    FOREIGN KEY (OrdenId) REFERENCES OrdenesServicio(IdOrden),
    FOREIGN KEY (MaterialId) REFERENCES Materiales(IdMaterial)
);

-- Equipos y Fabricación
CREATE TABLE Equipos (
    IdEquipo INT IDENTITY PRIMARY KEY,
    Nombre NVARCHAR(100),
    Categoria NVARCHAR(100),
    Especificaciones NVARCHAR(255),
    Estado NVARCHAR(20)
);

CREATE TABLE ProyectosFabricacion (
    IdProyecto INT IDENTITY PRIMARY KEY,
    ClienteId INT NOT NULL,
    Descripcion NVARCHAR(255),
    FechaInicio DATE,
    FechaFin DATE,
    Estado NVARCHAR(20),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(IdCliente)
);

-- Evidencias y Observaciones
CREATE TABLE Evidencias (
    IdEvidencia INT IDENTITY PRIMARY KEY,
    OrdenId INT NOT NULL,
    Tipo NVARCHAR(50),
    UrlArchivo NVARCHAR(255),
    FOREIGN KEY (OrdenId) REFERENCES OrdenesServicio(IdOrden)
);

CREATE TABLE ObservacionesTecnicas (
    IdObservacion INT IDENTITY PRIMARY KEY,
    OrdenId INT NOT NULL,
    EmpleadoId INT NOT NULL,
    Descripcion NVARCHAR(255),
    FOREIGN KEY (OrdenId) REFERENCES OrdenesServicio(IdOrden),
    FOREIGN KEY (EmpleadoId) REFERENCES Empleados(IdEmpleado)
);

-- Encuestas
CREATE TABLE Encuestas (
    IdEncuesta INT IDENTITY PRIMARY KEY,
    OrdenId INT NOT NULL,
    ClienteId INT NOT NULL,
    CalificacionServicio INT CHECK (CalificacionServicio BETWEEN 1 AND 5),
    CalificacionTecnico INT CHECK (CalificacionTecnico BETWEEN 1 AND 5),
    Comentarios NVARCHAR(255),
    Fecha DATE,
    FOREIGN KEY (OrdenId) REFERENCES OrdenesServicio(IdOrden),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(IdCliente)
);

-- Notificaciones
CREATE TABLE Notificaciones (
    IdNotificacion INT IDENTITY PRIMARY KEY,
    OrdenId INT NULL,
    ClienteId INT NULL,
    MaterialId INT NULL,
    Titulo NVARCHAR(100),
    Mensaje NVARCHAR(255),
    Fecha DATETIME DEFAULT GETDATE(),
    Leida BIT DEFAULT 0,
    FOREIGN KEY (OrdenId) REFERENCES OrdenesServicio(IdOrden),
    FOREIGN KEY (ClienteId) REFERENCES Clientes(IdCliente),
    FOREIGN KEY (MaterialId) REFERENCES Materiales(IdMaterial)
);

-- Auditoría
CREATE TABLE Auditoria (
    IdAuditoria INT IDENTITY PRIMARY KEY,
    UsuarioId NVARCHAR(450) NOT NULL,
    Accion NVARCHAR(100),
    Fecha DATETIME DEFAULT GETDATE(),
    Detalle NVARCHAR(255),
    FOREIGN KEY (UsuarioId) REFERENCES AspNetUsers(Id)
);

-- Relacionar equipos con clientes (dueño del activo)
ALTER TABLE Equipos
ADD ClienteId INT NULL,
    FOREIGN KEY (ClienteId) REFERENCES Clientes(IdCliente);

-- Relacionar equipos con órdenes de servicio (mantenimientos)
CREATE TABLE HistorialEquipos (
    IdHistorial INT IDENTITY PRIMARY KEY,
    EquipoId INT NOT NULL,
    OrdenId INT NOT NULL,
    FechaServicio DATE NOT NULL,
    Descripcion NVARCHAR(255),
    FOREIGN KEY (EquipoId) REFERENCES Equipos(IdEquipo),
    FOREIGN KEY (OrdenId) REFERENCES OrdenesServicio(IdOrden)
);


