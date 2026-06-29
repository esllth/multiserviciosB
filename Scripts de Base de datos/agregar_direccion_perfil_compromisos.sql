IF COL_LENGTH('dbo.Cotizaciones', 'UsarDireccionPerfil') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones
    ADD UsarDireccionPerfil bit NOT NULL
        CONSTRAINT DF_Cotizaciones_UsarDireccionPerfil DEFAULT (0);
END;

IF COL_LENGTH('dbo.OrdenesServicio', 'UsarDireccionPerfil') IS NULL
BEGIN
    ALTER TABLE dbo.OrdenesServicio
    ADD UsarDireccionPerfil bit NOT NULL
        CONSTRAINT DF_OrdenesServicio_UsarDireccionPerfil DEFAULT (0);
END;
