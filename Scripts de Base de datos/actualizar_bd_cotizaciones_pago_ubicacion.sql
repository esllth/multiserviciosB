USE [MultiservicioDB];
GO

IF COL_LENGTH('dbo.Cotizaciones', 'UsarDireccionPerfil') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones
    ADD UsarDireccionPerfil bit NOT NULL
        CONSTRAINT DF_Cotizaciones_UsarDireccionPerfil DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.OrdenesServicio', 'UsarDireccionPerfil') IS NULL
BEGIN
    ALTER TABLE dbo.OrdenesServicio
    ADD UsarDireccionPerfil bit NOT NULL
        CONSTRAINT DF_OrdenesServicio_UsarDireccionPerfil DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.Cotizaciones', 'RequiereAdelanto') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones
    ADD RequiereAdelanto bit NOT NULL
        CONSTRAINT DF_Cotizaciones_RequiereAdelanto DEFAULT (0);
END;
GO

IF COL_LENGTH('dbo.Cotizaciones', 'PorcentajeAdelanto') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones ADD PorcentajeAdelanto int NULL;
END;
GO

IF COL_LENGTH('dbo.Cotizaciones', 'EnlaceWaze') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones ADD EnlaceWaze nvarchar(500) NULL;
END;
GO

IF COL_LENGTH('dbo.Cotizaciones', 'FormaPagoAceptada') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones ADD FormaPagoAceptada nvarchar(40) NULL;
END;
GO

IF COL_LENGTH('dbo.OrdenesServicio', 'EnlaceWaze') IS NULL
BEGIN
    ALTER TABLE dbo.OrdenesServicio ADD EnlaceWaze nvarchar(500) NULL;
END;
GO

SELECT
    COL_LENGTH('dbo.Cotizaciones', 'UsarDireccionPerfil') AS Cotizaciones_UsarDireccionPerfil,
    COL_LENGTH('dbo.OrdenesServicio', 'UsarDireccionPerfil') AS OrdenesServicio_UsarDireccionPerfil,
    COL_LENGTH('dbo.Cotizaciones', 'RequiereAdelanto') AS Cotizaciones_RequiereAdelanto,
    COL_LENGTH('dbo.Cotizaciones', 'PorcentajeAdelanto') AS Cotizaciones_PorcentajeAdelanto,
    COL_LENGTH('dbo.Cotizaciones', 'EnlaceWaze') AS Cotizaciones_EnlaceWaze,
    COL_LENGTH('dbo.Cotizaciones', 'FormaPagoAceptada') AS Cotizaciones_FormaPagoAceptada,
    COL_LENGTH('dbo.OrdenesServicio', 'EnlaceWaze') AS OrdenesServicio_EnlaceWaze;
GO
