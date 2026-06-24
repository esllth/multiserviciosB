CREATE TABLE [dbo].[Equipos] (
    [IdEquipo]                    INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]                      NVARCHAR (100)  NOT NULL,
    [Categoria]                   NVARCHAR (100)  NULL,
    [Especificaciones]            NVARCHAR (2000) NULL,
    [Estado]                      NVARCHAR (20)   NULL,
    [ClienteId]                   INT             NULL,
    [Codigo]                      NVARCHAR (50)   NULL,
    [TipoEquipo]                  NVARCHAR (100)  NULL,
    [Marca]                       NVARCHAR (100)  NULL,
    [Modelo]                      NVARCHAR (100)  NULL,
    [NumeroSerie]                 NVARCHAR (100)  NULL,
    [FechaAdquisicion]            DATETIME2 (7)   NULL,
    [FrecuenciaMantenimientoDias] INT             NULL,
    [UltimoMantenimiento]         DATETIME2 (7)   NULL,
    [ProximoMantenimiento]        DATETIME2 (7)   NULL,
    [Observaciones]               NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([IdEquipo] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente])
);

