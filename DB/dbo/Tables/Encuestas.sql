CREATE TABLE [dbo].[Encuestas] (
    [IdEncuesta]           INT            IDENTITY (1, 1) NOT NULL,
    [OrdenId]              INT            NOT NULL,
    [ClienteId]            INT            NOT NULL,
    [CalificacionServicio] INT            NULL,
    [CalificacionTecnico]  INT            NULL,
    [Comentarios]          NVARCHAR (255) NULL,
    [Fecha]                DATE           NULL,
    PRIMARY KEY CLUSTERED ([IdEncuesta] ASC),
    CHECK ([CalificacionServicio]>=(1) AND [CalificacionServicio]<=(5)),
    CHECK ([CalificacionTecnico]>=(1) AND [CalificacionTecnico]<=(5)),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente]),
    FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
);

