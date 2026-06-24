CREATE TABLE [dbo].[Cotizaciones] (
    [IdCotizacion]       INT             IDENTITY (1, 1) NOT NULL,
    [ClienteId]          INT             NOT NULL,
    [TipoServicioId]     INT             NOT NULL,
    [EstadoCotizacionId] INT             NOT NULL,
    [Descripcion]        NVARCHAR (255)  NULL,
    [MontoPresupuesto]   DECIMAL (12, 2) NULL,
    [FechaSolicitud]     DATE            NULL,
    [AprobadaPorCliente] BIT             DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([IdCotizacion] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente]),
    FOREIGN KEY ([EstadoCotizacionId]) REFERENCES [dbo].[EstadoCotizacion] ([Id]),
    FOREIGN KEY ([TipoServicioId]) REFERENCES [dbo].[TipoServicio] ([Id])
);

