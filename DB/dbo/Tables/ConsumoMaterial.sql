CREATE TABLE [dbo].[ConsumoMaterial] (
    [IdConsumo]     INT             IDENTITY (1, 1) NOT NULL,
    [OrdenId]       INT             NOT NULL,
    [MaterialId]    INT             NOT NULL,
    [CantidadUsada] DECIMAL (10, 2) NULL,
    [FechaRegistro] DATETIME2       CONSTRAINT [DF_ConsumoMaterial_FechaRegistro] DEFAULT (GETDATE()) NOT NULL,
    PRIMARY KEY CLUSTERED ([IdConsumo] ASC),
    FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Materiales] ([IdMaterial]),
    FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
);

