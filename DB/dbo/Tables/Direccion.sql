CREATE TABLE [dbo].[Direccion] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [UbicacionDTAId] INT            NOT NULL,
    [OtrasSenas]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    FOREIGN KEY ([UbicacionDTAId]) REFERENCES [dbo].[UbicacionDTA] ([Id])
);

