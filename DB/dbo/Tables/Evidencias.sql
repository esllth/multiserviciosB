CREATE TABLE [dbo].[Evidencias] (
    [IdEvidencia] INT            IDENTITY (1, 1) NOT NULL,
    [OrdenId]     INT            NOT NULL,
    [Tipo]        NVARCHAR (50)  NULL,
    [UrlArchivo]  NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([IdEvidencia] ASC),
    FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
);

