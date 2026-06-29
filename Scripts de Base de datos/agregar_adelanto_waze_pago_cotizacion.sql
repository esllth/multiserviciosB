IF COL_LENGTH('dbo.Cotizaciones', 'RequiereAdelanto') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones
    ADD RequiereAdelanto bit NOT NULL
        CONSTRAINT DF_Cotizaciones_RequiereAdelanto DEFAULT (0);
END;

IF COL_LENGTH('dbo.Cotizaciones', 'PorcentajeAdelanto') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones ADD PorcentajeAdelanto int NULL;
END;

IF COL_LENGTH('dbo.Cotizaciones', 'EnlaceWaze') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones ADD EnlaceWaze nvarchar(500) NULL;
END;

IF COL_LENGTH('dbo.Cotizaciones', 'FormaPagoAceptada') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones ADD FormaPagoAceptada nvarchar(40) NULL;
END;

IF COL_LENGTH('dbo.OrdenesServicio', 'EnlaceWaze') IS NULL
BEGIN
    ALTER TABLE dbo.OrdenesServicio ADD EnlaceWaze nvarchar(500) NULL;
END;
