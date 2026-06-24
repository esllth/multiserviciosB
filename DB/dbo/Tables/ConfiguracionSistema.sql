CREATE TABLE [dbo].[ConfiguracionSistema] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Clave]       NVARCHAR (100) NOT NULL,
    [Valor]       NVARCHAR (255) NOT NULL,
    [Descripcion] NVARCHAR (255) NULL,
    CONSTRAINT [PK_ConfiguracionSistema] PRIMARY KEY CLUSTERED ([Id] ASC)
);

