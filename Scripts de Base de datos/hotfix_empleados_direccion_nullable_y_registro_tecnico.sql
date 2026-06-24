USE [MultiservicioDB];
GO

IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] DROP CONSTRAINT [FK_Empleados_Direccion];
GO

IF EXISTS (
    SELECT 1
    FROM sys.columns
    WHERE object_id = OBJECT_ID(N'[dbo].[Empleados]')
      AND name = N'DireccionId'
      AND is_nullable = 0
)
    ALTER TABLE [dbo].[Empleados] ALTER COLUMN [DireccionId] INT NULL;
GO

UPDATE [dbo].[Empleados]
SET [DireccionId] = NULL
WHERE [DireccionId] = 0;
GO

IF OBJECT_ID(N'[dbo].[FK_Empleados_Direccion]', N'F') IS NULL
   AND OBJECT_ID(N'[dbo].[Direccion]', N'U') IS NOT NULL
    ALTER TABLE [dbo].[Empleados] WITH CHECK
    ADD CONSTRAINT [FK_Empleados_Direccion]
    FOREIGN KEY ([DireccionId]) REFERENCES [dbo].[Direccion] ([Id]);
GO

IF OBJECT_ID(N'[dbo].[__EFMigrationsHistory]', N'U') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1
        FROM [dbo].[__EFMigrationsHistory]
        WHERE [MigrationId] = N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico'
   )
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260624000000_PermitirEmpleadoSinDireccionYRegistroTecnico', N'10.0.7');
GO
