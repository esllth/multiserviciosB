CREATE TABLE [dbo].[HistorialEquipos] (
    [IdHistorial]          INT             IDENTITY (1, 1) NOT NULL,
    [EquipoId]             INT             NOT NULL,
    [OrdenId]              INT             NOT NULL,
    [FechaServicio]        DATE            NOT NULL,
    [Descripcion]          NVARCHAR (2000) NULL,
    [TipoServicio]         NVARCHAR (50)   NULL,
    [EstadoAnterior]       NVARCHAR (30)   NULL,
    [EstadoPosterior]      NVARCHAR (30)   NULL,
    [ObservacionesTecnico] NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([IdHistorial] ASC),
    FOREIGN KEY ([EquipoId]) REFERENCES [dbo].[Equipos] ([IdEquipo]),
    FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
);

