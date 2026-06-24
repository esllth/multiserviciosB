CREATE TABLE [dbo].[Notificaciones] (
    [IdNotificacion] INT            IDENTITY (1, 1) NOT NULL,
    [OrdenId]        INT            NULL,
    [ClienteId]      INT            NULL,
    [MaterialId]     INT            NULL,
    [Titulo]         NVARCHAR (100) NULL,
    [Mensaje]        NVARCHAR (255) NULL,
    [Fecha]          DATETIME       DEFAULT (getdate()) NULL,
    [Leida]          BIT            DEFAULT ((0)) NULL,
    PRIMARY KEY CLUSTERED ([IdNotificacion] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente]),
    FOREIGN KEY ([MaterialId]) REFERENCES [dbo].[Materiales] ([IdMaterial]),
    FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
);

