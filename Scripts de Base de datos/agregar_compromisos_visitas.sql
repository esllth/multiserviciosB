IF COL_LENGTH('dbo.TipoServicio', 'RequiereVisita') IS NULL
BEGIN
    ALTER TABLE dbo.TipoServicio
    ADD RequiereVisita bit NOT NULL
        CONSTRAINT DF_TipoServicio_RequiereVisita DEFAULT (0);
END;

IF COL_LENGTH('dbo.Cotizaciones', 'FechaVisitaSolicitada') IS NULL
BEGIN
    ALTER TABLE dbo.Cotizaciones
    ADD FechaVisitaSolicitada datetime2 NULL;
END;

IF COL_LENGTH('dbo.OrdenesServicio', 'FechaCompromiso') IS NULL
BEGIN
    ALTER TABLE dbo.OrdenesServicio
    ADD FechaCompromiso datetime2 NULL;
END;

IF COL_LENGTH('dbo.OrdenesServicio', 'CompromisoConfirmado') IS NULL
BEGIN
    ALTER TABLE dbo.OrdenesServicio
    ADD CompromisoConfirmado bit NOT NULL
        CONSTRAINT DF_OrdenesServicio_CompromisoConfirmado DEFAULT (0);
END;
