CREATE TABLE [dbo].[TipoServicio] (
    [Id]     INT            IDENTITY (1, 1) NOT NULL,
    [Nombre] NVARCHAR (100) NOT NULL,
    [Estado] NVARCHAR (20)  NULL,
    [RequiereVisita] BIT DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

