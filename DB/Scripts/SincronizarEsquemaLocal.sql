/*
    Sincroniza de forma segura las extensiones recientes del esquema.
    No elimina tablas ni registros existentes.
*/

IF COL_LENGTH(N'dbo.ConsumoMaterial', N'FechaRegistro') IS NULL
BEGIN
    ALTER TABLE [dbo].[ConsumoMaterial]
        ADD [FechaRegistro] DATETIME2 NOT NULL
            CONSTRAINT [DF_ConsumoMaterial_FechaRegistro] DEFAULT (GETDATE());
END;
GO

IF COL_LENGTH(N'dbo.Empleados', N'FotoPerfil') IS NULL
BEGIN
    ALTER TABLE [dbo].[Empleados]
        ADD [FotoPerfil] NVARCHAR(300) NULL;
END;
GO

IF COL_LENGTH(N'dbo.Zonas', N'CodigoDTA') IS NULL
BEGIN
    ALTER TABLE [dbo].[Zonas]
        ADD [CodigoDTA] NVARCHAR(20) NOT NULL
            CONSTRAINT [DF_Zonas_CodigoDTA] DEFAULT (N'');
END;
GO

IF OBJECT_ID(N'dbo.RevistaPublicaciones', N'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[RevistaPublicaciones]
    (
        [IdPublicacion] INT IDENTITY(1, 1) NOT NULL,
        [Titulo] NVARCHAR(80) NOT NULL,
        [Descripcion] NVARCHAR(250) NOT NULL,
        [Imagen] NVARCHAR(300) NOT NULL,
        [TextoEnlace] NVARCHAR(50) NOT NULL,
        [Orden] INT NOT NULL,
        [Activo] BIT NOT NULL
            CONSTRAINT [DF_RevistaPublicaciones_Activo] DEFAULT ((1)),
        CONSTRAINT [PK_RevistaPublicaciones]
            PRIMARY KEY CLUSTERED ([IdPublicacion] ASC)
    );
END;
GO

IF OBJECT_ID(N'dbo.RevistaPublicaciones', N'U') IS NOT NULL
   AND NOT EXISTS
   (
       SELECT 1
       FROM sys.key_constraints
       WHERE [parent_object_id] = OBJECT_ID(N'dbo.RevistaPublicaciones')
         AND [type] = N'PK'
         AND [name] = N'PK_RevistaPublicaciones'
   )
BEGIN
    DECLARE @LlaveActual sysname;
    DECLARE @NombreCompletoLlave nvarchar(517);

    SELECT @LlaveActual = [name]
    FROM sys.key_constraints
    WHERE [parent_object_id] = OBJECT_ID(N'dbo.RevistaPublicaciones')
      AND [type] = N'PK';

    IF @LlaveActual IS NOT NULL
    BEGIN
        SET @NombreCompletoLlave = N'dbo.' + QUOTENAME(@LlaveActual);

        EXEC sys.sp_rename
            @objname = @NombreCompletoLlave,
            @newname = N'PK_RevistaPublicaciones',
            @objtype = N'OBJECT';
    END;
END;
GO
