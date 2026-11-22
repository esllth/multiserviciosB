IF NOT EXISTS (SELECT 1 FROM [dbo].[EstadoCotizacion] WHERE [Nombre] = N'Cancelada')
BEGIN
    INSERT INTO [dbo].[EstadoCotizacion] ([Nombre]) VALUES (N'Cancelada');
END;
GO

UPDATE cotizacion
SET cotizacion.[EstadoCotizacionId] = estadoCancelada.[Id]
FROM [dbo].[Cotizaciones] AS cotizacion
INNER JOIN [dbo].[Clientes] AS cliente
    ON cliente.[IdCliente] = cotizacion.[ClienteId]
CROSS JOIN [dbo].[EstadoCotizacion] AS estadoCancelada
WHERE cliente.[Estado] = N'Inactivo'
  AND estadoCancelada.[Nombre] = N'Cancelada'
  AND cotizacion.[EstadoCotizacionId] <> estadoCancelada.[Id];
GO

CREATE OR ALTER TRIGGER [dbo].[TR_Cotizaciones_CancelarClienteInactivo]
ON [dbo].[Cotizaciones]
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE cotizacion
    SET cotizacion.[EstadoCotizacionId] = estadoCancelada.[Id]
    FROM [dbo].[Cotizaciones] AS cotizacion
    INNER JOIN inserted AS nuevo
        ON nuevo.[IdCotizacion] = cotizacion.[IdCotizacion]
    INNER JOIN [dbo].[Clientes] AS cliente
        ON cliente.[IdCliente] = cotizacion.[ClienteId]
    CROSS JOIN [dbo].[EstadoCotizacion] AS estadoCancelada
    WHERE cliente.[Estado] = N'Inactivo'
      AND estadoCancelada.[Nombre] = N'Cancelada'
      AND cotizacion.[EstadoCotizacionId] <> estadoCancelada.[Id];
END;
GO

CREATE OR ALTER TRIGGER [dbo].[TR_Clientes_CancelarCotizacionesAlDesactivar]
ON [dbo].[Clientes]
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE cotizacion
    SET cotizacion.[EstadoCotizacionId] = estadoCancelada.[Id]
    FROM [dbo].[Cotizaciones] AS cotizacion
    INNER JOIN inserted AS nuevo
        ON nuevo.[IdCliente] = cotizacion.[ClienteId]
    INNER JOIN deleted AS anterior
        ON anterior.[IdCliente] = nuevo.[IdCliente]
    CROSS JOIN [dbo].[EstadoCotizacion] AS estadoCancelada
    WHERE (anterior.[Estado] IS NULL OR anterior.[Estado] <> N'Inactivo')
      AND nuevo.[Estado] = N'Inactivo'
      AND estadoCancelada.[Nombre] = N'Cancelada'
      AND cotizacion.[EstadoCotizacionId] <> estadoCancelada.[Id];
END;
GO
