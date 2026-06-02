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

