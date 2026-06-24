CREATE TABLE [dbo].[Zonas] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Provincia]   NVARCHAR (100) NOT NULL,
    [Canton]      NVARCHAR (100) NOT NULL,
    [Distrito]    NVARCHAR (100) NOT NULL,
    [Descripcion] NVARCHAR (255) NULL,
    [Activo]      BIT            DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Zonas] PRIMARY KEY CLUSTERED ([Id] ASC)
);

