CREATE TABLE [dbo].[RevistaPublicaciones] (
    [IdPublicacion] INT            IDENTITY (1, 1) NOT NULL,
    [Titulo]        NVARCHAR (80)  NOT NULL,
    [Descripcion]   NVARCHAR (250) NOT NULL,
    [Imagen]        NVARCHAR (300) NOT NULL,
    [TextoEnlace]   NVARCHAR (50)  NOT NULL,
    [Orden]         INT            NOT NULL,
    [Activo]        BIT            CONSTRAINT [DF_RevistaPublicaciones_Activo] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_RevistaPublicaciones] PRIMARY KEY CLUSTERED ([IdPublicacion] ASC)
);
