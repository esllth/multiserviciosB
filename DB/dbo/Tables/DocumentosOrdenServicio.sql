CREATE TABLE [dbo].[DocumentosOrdenServicio] (
    [IdDocumento]         INT            IDENTITY (1, 1) NOT NULL,
    [OrdenId]             INT            NOT NULL,
    [NombreOriginal]      NVARCHAR (150) NOT NULL,
    [Ruta]                NVARCHAR (260) NOT NULL,
    [TipoContenido]       NVARCHAR (100) NOT NULL,
    [TipoDocumento]       NVARCHAR (50)  DEFAULT ('Otro') NOT NULL,
    [Descripcion]         NVARCHAR (500) NULL,
    [FechaCarga]          DATETIME2 (7)  DEFAULT (getdate()) NOT NULL,
    [CargadoPorUsuarioId] NVARCHAR (450) NULL,
    PRIMARY KEY CLUSTERED ([IdDocumento] ASC),
    CONSTRAINT [FK_DocumentosOrdenServicio_OrdenesServicio] FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_DocumentosOrdenServicio_OrdenId]
    ON [dbo].[DocumentosOrdenServicio]([OrdenId] ASC);

