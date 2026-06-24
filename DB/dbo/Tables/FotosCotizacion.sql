CREATE TABLE [dbo].[FotosCotizacion] (
    [IdFotoCotizacion] INT            IDENTITY (1, 1) NOT NULL,
    [CotizacionId]     INT            NOT NULL,
    [Ruta]             NVARCHAR (260) NOT NULL,
    [NombreOriginal]   NVARCHAR (150) NOT NULL,
    [TipoContenido]    NVARCHAR (50)  NOT NULL,
    [FechaCarga]       DATETIME2 (7)  NOT NULL,
    CONSTRAINT [PK_FotosCotizacion] PRIMARY KEY CLUSTERED ([IdFotoCotizacion] ASC),
    CONSTRAINT [FK_FotosCotizacion_Cotizaciones_CotizacionId] FOREIGN KEY ([CotizacionId]) REFERENCES [dbo].[Cotizaciones] ([IdCotizacion]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_FotosCotizacion_CotizacionId]
    ON [dbo].[FotosCotizacion]([CotizacionId] ASC);

