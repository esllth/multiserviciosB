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
BEGIN
    CREATE TABLE [Empleados] (
        [IdEmpleado] int NOT NULL IDENTITY,
        [IdentificacionEmpleado] nvarchar(max) NOT NULL,
        [NombreEmpleado] nvarchar(max) NOT NULL,
        [ApellidosEmpleado] nvarchar(max) NOT NULL,
        [CorreoElectronicoEmpleado] nvarchar(max) NOT NULL,
        [TelefonoEmpleado] nvarchar(max) NOT NULL,
        [DireccionId] int NOT NULL,
        [EstadoEmpleado] nvarchar(max) NOT NULL,
        [TieneUsuario] bit NOT NULL,
        [SalarioBase] decimal(10,2) NOT NULL,
        [FechaInicioEmpleado] datetime2 NOT NULL,
        [FechaFinalizacionEmpleado] datetime2 NULL,
        [UserId] nvarchar(max) NULL,
        CONSTRAINT [PK_Empleados] PRIMARY KEY ([IdEmpleado])
    );
END;

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
    VALUES (N'20260406034846_InitialClean', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Empleados]') AND [c].[name] = N'UserId');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [Empleados] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [Empleados] ALTER COLUMN [UserId] nvarchar(450) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [EstadosCotizacion] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_EstadosCotizacion] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [EstadosOrden] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(50) NOT NULL,
        CONSTRAINT [PK_EstadosOrden] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [Materiales] (
        [IdMaterial] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(255) NULL,
        [UnidadMedida] nvarchar(50) NULL,
        [StockActual] int NULL,
        [StockMinimo] int NULL,
        [PrecioUnitario] decimal(10,2) NULL,
        CONSTRAINT [PK_Materiales] PRIMARY KEY ([IdMaterial])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [TiposServicio] (
        [Id] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NOT NULL,
        [Estado] nvarchar(20) NULL,
        CONSTRAINT [PK_TiposServicio] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [UbicacionDTA] (
        [Id] int NOT NULL IDENTITY,
        [IdProvincia] int NOT NULL,
        [Provincia] nvarchar(100) NOT NULL,
        [IdCanton] int NOT NULL,
        [Canton] nvarchar(100) NOT NULL,
        [IdDistrito] int NOT NULL,
        [Distrito] nvarchar(100) NOT NULL,
        [CodigoDTA] nvarchar(20) NOT NULL,
        CONSTRAINT [PK_UbicacionDTA] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [Direcciones] (
        [Id] int NOT NULL IDENTITY,
        [UbicacionDTAId] int NOT NULL,
        [OtrasSenas] nvarchar(255) NULL,
        CONSTRAINT [PK_Direcciones] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Direcciones_UbicacionDTA_UbicacionDTAId] FOREIGN KEY ([UbicacionDTAId]) REFERENCES [UbicacionDTA] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [Clientes] (
        [IdCliente] int NOT NULL IDENTITY,
        [Identificacion] nvarchar(50) NOT NULL,
        [Nombre] nvarchar(100) NOT NULL,
        [Apellidos] nvarchar(100) NULL,
        [Correo] nvarchar(150) NULL,
        [Telefono] nvarchar(20) NULL,
        [DireccionId] int NULL,
        [Estado] nvarchar(20) NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([IdCliente]),
        CONSTRAINT [FK_Clientes_Direcciones_DireccionId] FOREIGN KEY ([DireccionId]) REFERENCES [Direcciones] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [Cotizaciones] (
        [IdCotizacion] int NOT NULL IDENTITY,
        [ClienteId] int NOT NULL,
        [TipoServicioId] int NOT NULL,
        [EstadoCotizacionId] int NOT NULL,
        [Descripcion] nvarchar(255) NULL,
        [MontoPresupuesto] decimal(12,2) NULL,
        [FechaSolicitud] datetime2 NULL,
        [AprobadaPorCliente] bit NOT NULL,
        CONSTRAINT [PK_Cotizaciones] PRIMARY KEY ([IdCotizacion]),
        CONSTRAINT [FK_Cotizaciones_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([IdCliente]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Cotizaciones_EstadosCotizacion_EstadoCotizacionId] FOREIGN KEY ([EstadoCotizacionId]) REFERENCES [EstadosCotizacion] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Cotizaciones_TiposServicio_TipoServicioId] FOREIGN KEY ([TipoServicioId]) REFERENCES [TiposServicio] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [Equipos] (
        [IdEquipo] int NOT NULL IDENTITY,
        [Nombre] nvarchar(100) NULL,
        [Categoria] nvarchar(100) NULL,
        [Especificaciones] nvarchar(255) NULL,
        [Estado] nvarchar(20) NULL,
        [ClienteId] int NULL,
        CONSTRAINT [PK_Equipos] PRIMARY KEY ([IdEquipo]),
        CONSTRAINT [FK_Equipos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([IdCliente]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [ProyectosFabricacion] (
        [IdProyecto] int NOT NULL IDENTITY,
        [ClienteId] int NOT NULL,
        [Descripcion] nvarchar(255) NULL,
        [FechaInicio] datetime2 NULL,
        [FechaFin] datetime2 NULL,
        [Estado] nvarchar(20) NULL,
        CONSTRAINT [PK_ProyectosFabricacion] PRIMARY KEY ([IdProyecto]),
        CONSTRAINT [FK_ProyectosFabricacion_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([IdCliente]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [OrdenesServicio] (
        [IdOrden] int NOT NULL IDENTITY,
        [CotizacionId] int NOT NULL,
        [ClienteId] int NOT NULL,
        [EmpleadoId] int NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaInicio] datetime2 NULL,
        [FechaFin] datetime2 NULL,
        [EstadoOrdenId] int NOT NULL,
        CONSTRAINT [PK_OrdenesServicio] PRIMARY KEY ([IdOrden]),
        CONSTRAINT [FK_OrdenesServicio_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([IdCliente]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesServicio_Cotizaciones_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [Cotizaciones] ([IdCotizacion]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesServicio_Empleados_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleados] ([IdEmpleado]) ON DELETE NO ACTION,
        CONSTRAINT [FK_OrdenesServicio_EstadosOrden_EstadoOrdenId] FOREIGN KEY ([EstadoOrdenId]) REFERENCES [EstadosOrden] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [ConsumosMaterial] (
        [IdConsumo] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [MaterialId] int NOT NULL,
        [CantidadUsada] decimal(10,2) NULL,
        CONSTRAINT [PK_ConsumosMaterial] PRIMARY KEY ([IdConsumo]),
        CONSTRAINT [FK_ConsumosMaterial_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ConsumosMaterial_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE TABLE [HistorialEquipos] (
        [IdHistorial] int NOT NULL IDENTITY,
        [EquipoId] int NOT NULL,
        [OrdenId] int NOT NULL,
        [FechaServicio] datetime2 NOT NULL,
        [Descripcion] nvarchar(255) NULL,
        CONSTRAINT [PK_HistorialEquipos] PRIMARY KEY ([IdHistorial]),
        CONSTRAINT [FK_HistorialEquipos_Equipos_EquipoId] FOREIGN KEY ([EquipoId]) REFERENCES [Equipos] ([IdEquipo]) ON DELETE NO ACTION,
        CONSTRAINT [FK_HistorialEquipos_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Empleados_UserId] ON [Empleados] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Clientes_DireccionId] ON [Clientes] ([DireccionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_ConsumosMaterial_MaterialId] ON [ConsumosMaterial] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_ConsumosMaterial_OrdenId] ON [ConsumosMaterial] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Cotizaciones_ClienteId] ON [Cotizaciones] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Cotizaciones_EstadoCotizacionId] ON [Cotizaciones] ([EstadoCotizacionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Cotizaciones_TipoServicioId] ON [Cotizaciones] ([TipoServicioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Direcciones_UbicacionDTAId] ON [Direcciones] ([UbicacionDTAId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_Equipos_ClienteId] ON [Equipos] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_HistorialEquipos_EquipoId] ON [HistorialEquipos] ([EquipoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_HistorialEquipos_OrdenId] ON [HistorialEquipos] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_OrdenesServicio_ClienteId] ON [OrdenesServicio] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_OrdenesServicio_CotizacionId] ON [OrdenesServicio] ([CotizacionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_OrdenesServicio_EmpleadoId] ON [OrdenesServicio] ([EmpleadoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_OrdenesServicio_EstadoOrdenId] ON [OrdenesServicio] ([EstadoOrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    CREATE INDEX [IX_ProyectosFabricacion_ClienteId] ON [ProyectosFabricacion] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    ALTER TABLE [Empleados] ADD CONSTRAINT [FK_Empleados_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260531201803_CFG001_Roles'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260531201803_CFG001_Roles', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Cotizaciones] DROP CONSTRAINT [FK_Cotizaciones_EstadosCotizacion_EstadoCotizacionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Cotizaciones] DROP CONSTRAINT [FK_Cotizaciones_TiposServicio_TipoServicioId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [OrdenesServicio] DROP CONSTRAINT [FK_OrdenesServicio_EstadosOrden_EstadoOrdenId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [TiposServicio] DROP CONSTRAINT [PK_TiposServicio];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [EstadosOrden] DROP CONSTRAINT [PK_EstadosOrden];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [EstadosCotizacion] DROP CONSTRAINT [PK_EstadosCotizacion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[TiposServicio]', N'TipoServicio', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[EstadosOrden]', N'EstadoOrden', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[EstadosCotizacion]', N'EstadoCotizacion', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [TipoServicio] ADD CONSTRAINT [PK_TipoServicio] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [EstadoOrden] ADD CONSTRAINT [PK_EstadoOrden] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [EstadoCotizacion] ADD CONSTRAINT [PK_EstadoCotizacion] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    CREATE TABLE [ConfiguracionSistema] (
        [Id] int NOT NULL IDENTITY,
        [Clave] nvarchar(100) NOT NULL,
        [Valor] nvarchar(255) NOT NULL,
        [Descripcion] nvarchar(255) NULL,
        CONSTRAINT [PK_ConfiguracionSistema] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    CREATE TABLE [Horarios] (
        [Id] int NOT NULL IDENTITY,
        [DiaSemana] nvarchar(20) NOT NULL,
        [HoraInicio] time NOT NULL,
        [HoraFin] time NOT NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Horarios] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    CREATE TABLE [Zonas] (
        [Id] int NOT NULL IDENTITY,
        [Provincia] nvarchar(100) NOT NULL,
        [Canton] nvarchar(100) NOT NULL,
        [Distrito] nvarchar(100) NOT NULL,
        [Descripcion] nvarchar(255) NULL,
        [Activo] bit NOT NULL,
        CONSTRAINT [PK_Zonas] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD CONSTRAINT [FK_Cotizaciones_EstadoCotizacion_EstadoCotizacionId] FOREIGN KEY ([EstadoCotizacionId]) REFERENCES [EstadoCotizacion] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD CONSTRAINT [FK_Cotizaciones_TipoServicio_TipoServicioId] FOREIGN KEY ([TipoServicioId]) REFERENCES [TipoServicio] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD CONSTRAINT [FK_OrdenesServicio_EstadoOrden_EstadoOrdenId] FOREIGN KEY ([EstadoOrdenId]) REFERENCES [EstadoOrden] ([Id]) ON DELETE CASCADE;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260602023719_AgregarModulosCampoMaterialesEquiposFabricacion', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602041648_AddDireccionIdToEmpleados'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Empleados]') AND [c].[name] = N'EstadoEmpleado');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Empleados] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [Empleados] ALTER COLUMN [EstadoEmpleado] bit NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602041648_AddDireccionIdToEmpleados'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260602041648_AddDireccionIdToEmpleados', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    DROP INDEX [IX_OrdenesServicio_CotizacionId] ON [OrdenesServicio];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OrdenesServicio]') AND [c].[name] = N'EmpleadoId');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [OrdenesServicio] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [OrdenesServicio] ALTER COLUMN [EmpleadoId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Empleados]') AND [c].[name] = N'CorreoElectronicoEmpleado');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Empleados] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [Empleados] ALTER COLUMN [CorreoElectronicoEmpleado] nvarchar(450) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    ALTER TABLE [Empleados] ADD [EstadoAcceso] nvarchar(30) NOT NULL DEFAULT N'PendienteRegistro';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    UPDATE [Empleados] SET [EstadoAcceso] = CASE WHEN [TieneUsuario] = 1 THEN N'Aprobado' ELSE N'PendienteRegistro' END
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    IF EXISTS (SELECT [CotizacionId] FROM [OrdenesServicio] GROUP BY [CotizacionId] HAVING COUNT(*) > 1) THROW 50001, 'Existen cotizaciones con más de una orden de servicio. Corrija los duplicados antes de aplicar la migración.', 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    IF EXISTS (SELECT [CorreoElectronicoEmpleado] FROM [Empleados] GROUP BY [CorreoElectronicoEmpleado] HAVING COUNT(*) > 1) THROW 50002, 'Existen empleados con correo duplicado. Corrija los duplicados antes de aplicar la migración.', 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    CREATE UNIQUE INDEX [IX_OrdenesServicio_CotizacionId] ON [OrdenesServicio] ([CotizacionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Empleados_CorreoElectronicoEmpleado] ON [Empleados] ([CorreoElectronicoEmpleado]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260602065214_CerrarSprintAccesoTecnicoOrdenAutomatica', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614210206_AgregarNombreNegocioYFotosCotizacion'
)
BEGIN
    ALTER TABLE [Clientes] ADD [NombreNegocio] nvarchar(150) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614210206_AgregarNombreNegocioYFotosCotizacion'
)
BEGIN
    CREATE TABLE [FotosCotizacion] (
        [IdFotoCotizacion] int NOT NULL IDENTITY,
        [CotizacionId] int NOT NULL,
        [Ruta] nvarchar(260) NOT NULL,
        [NombreOriginal] nvarchar(150) NOT NULL,
        [TipoContenido] nvarchar(50) NOT NULL,
        [FechaCarga] datetime2 NOT NULL,
        CONSTRAINT [PK_FotosCotizacion] PRIMARY KEY ([IdFotoCotizacion]),
        CONSTRAINT [FK_FotosCotizacion_Cotizaciones_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [Cotizaciones] ([IdCotizacion]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614210206_AgregarNombreNegocioYFotosCotizacion'
)
BEGIN
    CREATE INDEX [IX_FotosCotizacion_CotizacionId] ON [FotosCotizacion] ([CotizacionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260614210206_AgregarNombreNegocioYFotosCotizacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260614210206_AgregarNombreNegocioYFotosCotizacion', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Clientes] DROP CONSTRAINT [FK_Clientes_Direcciones_DireccionId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [ConsumosMaterial] DROP CONSTRAINT [FK_ConsumosMaterial_Materiales_MaterialId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [ConsumosMaterial] DROP CONSTRAINT [FK_ConsumosMaterial_OrdenesServicio_OrdenId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Direcciones] DROP CONSTRAINT [FK_Direcciones_UbicacionDTA_UbicacionDTAId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Direcciones] DROP CONSTRAINT [PK_Direcciones];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [ConsumosMaterial] DROP CONSTRAINT [PK_ConsumosMaterial];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[Direcciones]', N'Direccion', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[ConsumosMaterial]', N'ConsumoMaterial', 'OBJECT';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[Direccion].[IX_Direcciones_UbicacionDTAId]', N'IX_Direccion_UbicacionDTAId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[ConsumoMaterial].[IX_ConsumosMaterial_OrdenId]', N'IX_ConsumoMaterial_OrdenId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    EXEC sp_rename N'[ConsumoMaterial].[IX_ConsumosMaterial_MaterialId]', N'IX_ConsumoMaterial_MaterialId', 'INDEX';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Direccion] ADD CONSTRAINT [PK_Direccion] PRIMARY KEY ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [ConsumoMaterial] ADD CONSTRAINT [PK_ConsumoMaterial] PRIMARY KEY ([IdConsumo]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Clientes] ADD CONSTRAINT [FK_Clientes_Direccion_DireccionId] FOREIGN KEY ([DireccionId]) REFERENCES [Direccion] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [ConsumoMaterial] ADD CONSTRAINT [FK_ConsumoMaterial_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [ConsumoMaterial] ADD CONSTRAINT [FK_ConsumoMaterial_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    ALTER TABLE [Direccion] ADD CONSTRAINT [FK_Direccion_UbicacionDTA_UbicacionDTAId] FOREIGN KEY ([UbicacionDTAId]) REFERENCES [UbicacionDTA] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260619010817_ImplementacionSprints2y3_ModulosTecnicoMaterialesEquiposFabricacion', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico'
)
BEGIN
    IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NOT NULL
        ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Direccion];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Empleados]') AND [c].[name] = N'DireccionId');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Empleados] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [Empleados] ALTER COLUMN [DireccionId] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico'
)
BEGIN
    UPDATE [Empleados]
    SET [DireccionId] = NULL
    WHERE [DireccionId] = 0;

    IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NULL
       AND OBJECT_ID(N'[dbo].[Direccion]', N'U') IS NOT NULL
        ALTER TABLE [dbo].[Empleados] WITH CHECK
        ADD CONSTRAINT [FK_Empleados_Direccion]
        FOREIGN KEY ([DireccionId]) REFERENCES [dbo].[Direccion] ([Id]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    EXEC sp_rename N'[ProyectosFabricacion].[FechaInicio]', N'FechaInicioReal', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    EXEC sp_rename N'[ProyectosFabricacion].[FechaFin]', N'FechaInicioEstimada', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [TipoServicio] ADD [RequiereVisita] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProyectosFabricacion]') AND [c].[name] = N'Estado');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [ProyectosFabricacion] DROP CONSTRAINT ' + @var5 + ';');
    EXEC(N'UPDATE [ProyectosFabricacion] SET [Estado] = N'''' WHERE [Estado] IS NULL');
    ALTER TABLE [ProyectosFabricacion] ALTER COLUMN [Estado] nvarchar(30) NOT NULL;
    ALTER TABLE [ProyectosFabricacion] ADD DEFAULT N'' FOR [Estado];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProyectosFabricacion]') AND [c].[name] = N'Descripcion');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [ProyectosFabricacion] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [ProyectosFabricacion] ALTER COLUMN [Descripcion] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [CostoEstimado] decimal(12,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [CostoReal] decimal(12,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [DiseñoAprobado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaAprobacionDiseño] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaFinEstimada] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaFinReal] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaSolicitud] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [NombreProyecto] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [ObservacionesCliente] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [ObservacionesInternas] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [ComentariosFinales] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [CompromisoConfirmado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [EnlaceWaze] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [FechaAceptacionCliente] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [FechaCompromiso] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [FechaLlegadaSitio] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [LlegadaConfirmada] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [ObservacionesTecnicas] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [RequiereFotosObligatorias] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [UsarDireccionPerfil] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Materiales]') AND [c].[name] = N'Descripcion');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [Materiales] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [Materiales] ALTER COLUMN [Descripcion] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Materiales] ADD [AlertaStockActiva] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Materiales] ADD [Categoria] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Materiales] ADD [Codigo] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Materiales] ADD [Estado] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistorialEquipos]') AND [c].[name] = N'Descripcion');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [HistorialEquipos] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [HistorialEquipos] ALTER COLUMN [Descripcion] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [EstadoAnterior] nvarchar(30) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [EstadoPosterior] nvarchar(30) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [ObservacionesTecnico] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [TipoServicio] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Equipos]') AND [c].[name] = N'Nombre');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Equipos] DROP CONSTRAINT ' + @var9 + ';');
    EXEC(N'UPDATE [Equipos] SET [Nombre] = N'''' WHERE [Nombre] IS NULL');
    ALTER TABLE [Equipos] ALTER COLUMN [Nombre] nvarchar(100) NOT NULL;
    ALTER TABLE [Equipos] ADD DEFAULT N'' FOR [Nombre];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Equipos]') AND [c].[name] = N'Estado');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [Equipos] DROP CONSTRAINT ' + @var10 + ';');
    EXEC(N'UPDATE [Equipos] SET [Estado] = N'''' WHERE [Estado] IS NULL');
    ALTER TABLE [Equipos] ALTER COLUMN [Estado] nvarchar(30) NOT NULL;
    ALTER TABLE [Equipos] ADD DEFAULT N'' FOR [Estado];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Equipos]') AND [c].[name] = N'Especificaciones');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [Equipos] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [Equipos] ALTER COLUMN [Especificaciones] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Codigo] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [FechaAdquisicion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [FrecuenciaMantenimientoDias] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Marca] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Modelo] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [NumeroSerie] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Observaciones] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [ProximoMantenimiento] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [TipoEquipo] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Equipos] ADD [UltimoMantenimiento] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [EnlaceWaze] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [FechaVisitaSolicitada] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [FormaPagoAceptada] nvarchar(40) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [PorcentajeAdelanto] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [RequiereAdelanto] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [UsarDireccionPerfil] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    ALTER TABLE [ConsumoMaterial] ADD [FechaRegistro] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [AlertasMantenimiento] (
        [IdAlerta] int NOT NULL IDENTITY,
        [EquipoId] int NOT NULL,
        [FechaMantenimiento] datetime2 NOT NULL,
        [TipoMantenimiento] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Estado] nvarchar(20) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaNotificacion] datetime2 NULL,
        [FechaRealizacion] datetime2 NULL,
        CONSTRAINT [PK_AlertasMantenimiento] PRIMARY KEY ([IdAlerta]),
        CONSTRAINT [FK_AlertasMantenimiento_Equipos_EquipoId] FOREIGN KEY ([EquipoId]) REFERENCES [Equipos] ([IdEquipo]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [DocumentosFabricacion] (
        [IdDocumento] int NOT NULL IDENTITY,
        [ProyectoId] int NOT NULL,
        [NombreDocumento] nvarchar(100) NOT NULL,
        [TipoDocumento] nvarchar(50) NOT NULL,
        [Ruta] nvarchar(260) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [FechaCarga] datetime2 NOT NULL,
        [CargadoPorUsuarioId] nvarchar(450) NULL,
        CONSTRAINT [PK_DocumentosFabricacion] PRIMARY KEY ([IdDocumento]),
        CONSTRAINT [FK_DocumentosFabricacion_ProyectosFabricacion_ProyectoId] FOREIGN KEY ([ProyectoId]) REFERENCES [ProyectosFabricacion] ([IdProyecto]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [EventoOrdenServicio] (
        [IdEvento] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [TipoEvento] nvarchar(50) NOT NULL,
        [FechaEvento] datetime2 NOT NULL,
        [Descripcion] nvarchar(1000) NULL,
        [Latitud] decimal(10,7) NULL,
        [Longitud] decimal(10,7) NULL,
        [UsuarioId] nvarchar(450) NULL,
        CONSTRAINT [PK_EventoOrdenServicio] PRIMARY KEY ([IdEvento]),
        CONSTRAINT [FK_EventoOrdenServicio_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [FotoOrden] (
        [IdFotoOrden] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [Ruta] nvarchar(260) NOT NULL,
        [NombreOriginal] nvarchar(150) NOT NULL,
        [TipoContenido] nvarchar(50) NOT NULL,
        [TipoFoto] nvarchar(20) NOT NULL,
        [FechaCarga] datetime2 NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        CONSTRAINT [PK_FotoOrden] PRIMARY KEY ([IdFotoOrden]),
        CONSTRAINT [FK_FotoOrden_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [MaterialesProyectoFabricacion] (
        [IdMaterialProyecto] int NOT NULL IDENTITY,
        [ProyectoId] int NOT NULL,
        [MaterialId] int NOT NULL,
        [CantidadRequerida] decimal(10,2) NOT NULL,
        [CantidadUsada] decimal(10,2) NULL,
        [Observaciones] nvarchar(500) NULL,
        CONSTRAINT [PK_MaterialesProyectoFabricacion] PRIMARY KEY ([IdMaterialProyecto]),
        CONSTRAINT [FK_MaterialesProyectoFabricacion_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MaterialesProyectoFabricacion_ProyectosFabricacion_ProyectoId] FOREIGN KEY ([ProyectoId]) REFERENCES [ProyectosFabricacion] ([IdProyecto]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [Notificaciones] (
        [IdNotificacion] int NOT NULL IDENTITY,
        [OrdenId] int NULL,
        [ClienteId] int NULL,
        [MaterialId] int NULL,
        [Titulo] nvarchar(100) NULL,
        [Mensaje] nvarchar(255) NULL,
        [Fecha] datetime2 NULL,
        [Leida] bit NULL,
        CONSTRAINT [PK_Notificaciones] PRIMARY KEY ([IdNotificacion]),
        CONSTRAINT [FK_Notificaciones_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([IdCliente]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notificaciones_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notificaciones_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE TABLE [SolicitudesMaterial] (
        [IdSolicitud] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [MaterialId] int NOT NULL,
        [EmpleadoId] int NOT NULL,
        [CantidadSolicitada] decimal(10,2) NOT NULL,
        [FechaSolicitud] datetime2 NOT NULL,
        [Estado] nvarchar(20) NOT NULL,
        [Justificacion] nvarchar(500) NULL,
        [RespuestaAdmin] nvarchar(500) NULL,
        [FechaRespuesta] datetime2 NULL,
        CONSTRAINT [PK_SolicitudesMaterial] PRIMARY KEY ([IdSolicitud]),
        CONSTRAINT [FK_SolicitudesMaterial_Empleados_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleados] ([IdEmpleado]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SolicitudesMaterial_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SolicitudesMaterial_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_AlertasMantenimiento_EquipoId] ON [AlertasMantenimiento] ([EquipoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_DocumentosFabricacion_ProyectoId] ON [DocumentosFabricacion] ([ProyectoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_EventoOrdenServicio_OrdenId] ON [EventoOrdenServicio] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_FotoOrden_OrdenId] ON [FotoOrden] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_MaterialesProyectoFabricacion_MaterialId] ON [MaterialesProyectoFabricacion] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_MaterialesProyectoFabricacion_ProyectoId] ON [MaterialesProyectoFabricacion] ([ProyectoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_ClienteId] ON [Notificaciones] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_MaterialId] ON [Notificaciones] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_OrdenId] ON [Notificaciones] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_SolicitudesMaterial_EmpleadoId] ON [SolicitudesMaterial] ([EmpleadoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_SolicitudesMaterial_MaterialId] ON [SolicitudesMaterial] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    CREATE INDEX [IX_SolicitudesMaterial_OrdenId] ON [SolicitudesMaterial] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162106_REI002_DocumentosOrdenServicio'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260714162106_REI002_DocumentosOrdenServicio', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    EXEC sp_rename N'[ProyectosFabricacion].[FechaInicio]', N'FechaInicioReal', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    EXEC sp_rename N'[ProyectosFabricacion].[FechaFin]', N'FechaInicioEstimada', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [TipoServicio] ADD [RequiereVisita] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProyectosFabricacion]') AND [c].[name] = N'Estado');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ProyectosFabricacion] DROP CONSTRAINT ' + @var12 + ';');
    EXEC(N'UPDATE [ProyectosFabricacion] SET [Estado] = N'''' WHERE [Estado] IS NULL');
    ALTER TABLE [ProyectosFabricacion] ALTER COLUMN [Estado] nvarchar(30) NOT NULL;
    ALTER TABLE [ProyectosFabricacion] ADD DEFAULT N'' FOR [Estado];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ProyectosFabricacion]') AND [c].[name] = N'Descripcion');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ProyectosFabricacion] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [ProyectosFabricacion] ALTER COLUMN [Descripcion] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [CostoEstimado] decimal(12,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [CostoReal] decimal(12,2) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [DiseñoAprobado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaAprobacionDiseño] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaFinEstimada] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaFinReal] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [FechaSolicitud] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [NombreProyecto] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [ObservacionesCliente] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ProyectosFabricacion] ADD [ObservacionesInternas] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [ComentariosFinales] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [CompromisoConfirmado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [EnlaceWaze] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [FechaAceptacionCliente] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [FechaCompromiso] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [FechaLlegadaSitio] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [LlegadaConfirmada] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [ObservacionesTecnicas] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [RequiereFotosObligatorias] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [OrdenesServicio] ADD [UsarDireccionPerfil] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Materiales]') AND [c].[name] = N'Descripcion');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Materiales] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [Materiales] ALTER COLUMN [Descripcion] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Materiales] ADD [AlertaStockActiva] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Materiales] ADD [Categoria] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Materiales] ADD [Codigo] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Materiales] ADD [Estado] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[HistorialEquipos]') AND [c].[name] = N'Descripcion');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [HistorialEquipos] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [HistorialEquipos] ALTER COLUMN [Descripcion] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [EstadoAnterior] nvarchar(30) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [EstadoPosterior] nvarchar(30) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [ObservacionesTecnico] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [HistorialEquipos] ADD [TipoServicio] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var16 nvarchar(max);
    SELECT @var16 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Equipos]') AND [c].[name] = N'Nombre');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Equipos] DROP CONSTRAINT ' + @var16 + ';');
    EXEC(N'UPDATE [Equipos] SET [Nombre] = N'''' WHERE [Nombre] IS NULL');
    ALTER TABLE [Equipos] ALTER COLUMN [Nombre] nvarchar(100) NOT NULL;
    ALTER TABLE [Equipos] ADD DEFAULT N'' FOR [Nombre];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var17 nvarchar(max);
    SELECT @var17 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Equipos]') AND [c].[name] = N'Estado');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Equipos] DROP CONSTRAINT ' + @var17 + ';');
    EXEC(N'UPDATE [Equipos] SET [Estado] = N'''' WHERE [Estado] IS NULL');
    ALTER TABLE [Equipos] ALTER COLUMN [Estado] nvarchar(30) NOT NULL;
    ALTER TABLE [Equipos] ADD DEFAULT N'' FOR [Estado];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    DECLARE @var18 nvarchar(max);
    SELECT @var18 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Equipos]') AND [c].[name] = N'Especificaciones');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Equipos] DROP CONSTRAINT ' + @var18 + ';');
    ALTER TABLE [Equipos] ALTER COLUMN [Especificaciones] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Codigo] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [FechaAdquisicion] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [FrecuenciaMantenimientoDias] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Marca] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Modelo] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [NumeroSerie] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [Observaciones] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [ProximoMantenimiento] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [TipoEquipo] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Equipos] ADD [UltimoMantenimiento] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [EnlaceWaze] nvarchar(500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [FechaVisitaSolicitada] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [FormaPagoAceptada] nvarchar(40) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [PorcentajeAdelanto] int NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [RequiereAdelanto] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [Cotizaciones] ADD [UsarDireccionPerfil] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    ALTER TABLE [ConsumoMaterial] ADD [FechaRegistro] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [AlertasMantenimiento] (
        [IdAlerta] int NOT NULL IDENTITY,
        [EquipoId] int NOT NULL,
        [FechaMantenimiento] datetime2 NOT NULL,
        [TipoMantenimiento] nvarchar(50) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [Estado] nvarchar(20) NOT NULL,
        [FechaCreacion] datetime2 NOT NULL,
        [FechaNotificacion] datetime2 NULL,
        [FechaRealizacion] datetime2 NULL,
        CONSTRAINT [PK_AlertasMantenimiento] PRIMARY KEY ([IdAlerta]),
        CONSTRAINT [FK_AlertasMantenimiento_Equipos_EquipoId] FOREIGN KEY ([EquipoId]) REFERENCES [Equipos] ([IdEquipo]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [DocumentosFabricacion] (
        [IdDocumento] int NOT NULL IDENTITY,
        [ProyectoId] int NOT NULL,
        [NombreDocumento] nvarchar(100) NOT NULL,
        [TipoDocumento] nvarchar(50) NOT NULL,
        [Ruta] nvarchar(260) NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        [FechaCarga] datetime2 NOT NULL,
        [CargadoPorUsuarioId] nvarchar(450) NULL,
        CONSTRAINT [PK_DocumentosFabricacion] PRIMARY KEY ([IdDocumento]),
        CONSTRAINT [FK_DocumentosFabricacion_ProyectosFabricacion_ProyectoId] FOREIGN KEY ([ProyectoId]) REFERENCES [ProyectosFabricacion] ([IdProyecto]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [EventoOrdenServicio] (
        [IdEvento] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [TipoEvento] nvarchar(50) NOT NULL,
        [FechaEvento] datetime2 NOT NULL,
        [Descripcion] nvarchar(1000) NULL,
        [Latitud] decimal(10,7) NULL,
        [Longitud] decimal(10,7) NULL,
        [UsuarioId] nvarchar(450) NULL,
        CONSTRAINT [PK_EventoOrdenServicio] PRIMARY KEY ([IdEvento]),
        CONSTRAINT [FK_EventoOrdenServicio_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [FotoOrden] (
        [IdFotoOrden] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [Ruta] nvarchar(260) NOT NULL,
        [NombreOriginal] nvarchar(150) NOT NULL,
        [TipoContenido] nvarchar(50) NOT NULL,
        [TipoFoto] nvarchar(20) NOT NULL,
        [FechaCarga] datetime2 NOT NULL,
        [Descripcion] nvarchar(500) NULL,
        CONSTRAINT [PK_FotoOrden] PRIMARY KEY ([IdFotoOrden]),
        CONSTRAINT [FK_FotoOrden_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [MaterialesProyectoFabricacion] (
        [IdMaterialProyecto] int NOT NULL IDENTITY,
        [ProyectoId] int NOT NULL,
        [MaterialId] int NOT NULL,
        [CantidadRequerida] decimal(10,2) NOT NULL,
        [CantidadUsada] decimal(10,2) NULL,
        [Observaciones] nvarchar(500) NULL,
        CONSTRAINT [PK_MaterialesProyectoFabricacion] PRIMARY KEY ([IdMaterialProyecto]),
        CONSTRAINT [FK_MaterialesProyectoFabricacion_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MaterialesProyectoFabricacion_ProyectosFabricacion_ProyectoId] FOREIGN KEY ([ProyectoId]) REFERENCES [ProyectosFabricacion] ([IdProyecto]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [Notificaciones] (
        [IdNotificacion] int NOT NULL IDENTITY,
        [OrdenId] int NULL,
        [ClienteId] int NULL,
        [MaterialId] int NULL,
        [Titulo] nvarchar(100) NULL,
        [Mensaje] nvarchar(255) NULL,
        [Fecha] datetime2 NULL,
        [Leida] bit NULL,
        CONSTRAINT [PK_Notificaciones] PRIMARY KEY ([IdNotificacion]),
        CONSTRAINT [FK_Notificaciones_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([IdCliente]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notificaciones_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Notificaciones_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE TABLE [SolicitudesMaterial] (
        [IdSolicitud] int NOT NULL IDENTITY,
        [OrdenId] int NOT NULL,
        [MaterialId] int NOT NULL,
        [EmpleadoId] int NOT NULL,
        [CantidadSolicitada] decimal(10,2) NOT NULL,
        [FechaSolicitud] datetime2 NOT NULL,
        [Estado] nvarchar(20) NOT NULL,
        [Justificacion] nvarchar(500) NULL,
        [RespuestaAdmin] nvarchar(500) NULL,
        [FechaRespuesta] datetime2 NULL,
        CONSTRAINT [PK_SolicitudesMaterial] PRIMARY KEY ([IdSolicitud]),
        CONSTRAINT [FK_SolicitudesMaterial_Empleados_EmpleadoId] FOREIGN KEY ([EmpleadoId]) REFERENCES [Empleados] ([IdEmpleado]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SolicitudesMaterial_Materiales_MaterialId] FOREIGN KEY ([MaterialId]) REFERENCES [Materiales] ([IdMaterial]) ON DELETE NO ACTION,
        CONSTRAINT [FK_SolicitudesMaterial_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [OrdenesServicio] ([IdOrden]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_AlertasMantenimiento_EquipoId] ON [AlertasMantenimiento] ([EquipoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_DocumentosFabricacion_ProyectoId] ON [DocumentosFabricacion] ([ProyectoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_EventoOrdenServicio_OrdenId] ON [EventoOrdenServicio] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_FotoOrden_OrdenId] ON [FotoOrden] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_MaterialesProyectoFabricacion_MaterialId] ON [MaterialesProyectoFabricacion] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_MaterialesProyectoFabricacion_ProyectoId] ON [MaterialesProyectoFabricacion] ([ProyectoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_ClienteId] ON [Notificaciones] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_MaterialId] ON [Notificaciones] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_Notificaciones_OrdenId] ON [Notificaciones] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_SolicitudesMaterial_EmpleadoId] ON [SolicitudesMaterial] ([EmpleadoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_SolicitudesMaterial_MaterialId] ON [SolicitudesMaterial] ([MaterialId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    CREATE INDEX [IX_SolicitudesMaterial_OrdenId] ON [SolicitudesMaterial] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260714162125_PendingModelFix'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260714162125_PendingModelFix', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819011424_AmpliarDescripcionCotizacionAsistente'
)
BEGIN
    DECLARE @var19 nvarchar(max);
    SELECT @var19 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cotizaciones]') AND [c].[name] = N'Descripcion');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Cotizaciones] DROP CONSTRAINT ' + @var19 + ';');
    ALTER TABLE [Cotizaciones] ALTER COLUMN [Descripcion] nvarchar(1000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819011424_AmpliarDescripcionCotizacionAsistente'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819011424_AmpliarDescripcionCotizacionAsistente', N'10.0.7');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819020000_EncuestaUnicaPorOrden'
)
BEGIN
    IF OBJECT_ID(N'dbo.Encuestas', N'U') IS NULL
    BEGIN
        CREATE TABLE [dbo].[Encuestas] (
            [IdEncuesta] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
            [OrdenId] INT NOT NULL,
            [ClienteId] INT NOT NULL,
            [CalificacionServicio] INT NULL,
            [CalificacionTecnico] INT NULL,
            [Comentarios] NVARCHAR(255) NULL,
            [Fecha] DATE NULL,
            CONSTRAINT [CK_Encuestas_CalificacionServicio] CHECK ([CalificacionServicio] BETWEEN 1 AND 5),
            CONSTRAINT [CK_Encuestas_CalificacionTecnico] CHECK ([CalificacionTecnico] BETWEEN 1 AND 5),
            CONSTRAINT [FK_Encuestas_OrdenesServicio_OrdenId] FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden]),
            CONSTRAINT [FK_Encuestas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente])
        );
    END;

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Encuestas_OrdenId' AND object_id = OBJECT_ID(N'dbo.Encuestas'))
        CREATE UNIQUE INDEX [IX_Encuestas_OrdenId] ON [dbo].[Encuestas] ([OrdenId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260819020000_EncuestaUnicaPorOrden'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260819020000_EncuestaUnicaPorOrden', N'10.0.7');
END;

COMMIT;
GO

