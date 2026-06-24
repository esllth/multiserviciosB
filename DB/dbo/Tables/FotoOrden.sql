CREATE TABLE [dbo].[FotoOrden] (
    [IdFotoOrden]    INT            IDENTITY (1, 1) NOT NULL,
    [OrdenId]        INT            NOT NULL,
    [Ruta]           NVARCHAR (260) NOT NULL,
    [NombreOriginal] NVARCHAR (150) NOT NULL,
    [TipoContenido]  NVARCHAR (50)  NOT NULL,
    [TipoFoto]       NVARCHAR (20)  NOT NULL,
    [FechaCarga]     DATETIME       DEFAULT (getdate()) NOT NULL,
    [Descripcion]    NVARCHAR (500) NULL,
    PRIMARY KEY CLUSTERED ([IdFotoOrden] ASC),
    CHECK ([TipoFoto]='Final' OR [TipoFoto]='Inicial')
);


GO
CREATE NONCLUSTERED INDEX [IX_FotoOrden_OrdenId]
    ON [dbo].[FotoOrden]([OrdenId] ASC);

