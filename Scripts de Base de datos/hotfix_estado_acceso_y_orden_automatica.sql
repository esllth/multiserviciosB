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
