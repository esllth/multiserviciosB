CREATE TABLE [dbo].[UbicacionDTA] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [IdProvincia] INT            NOT NULL,
    [Provincia]   NVARCHAR (100) NOT NULL,
    [IdCanton]    INT            NOT NULL,
    [Canton]      NVARCHAR (100) NOT NULL,
    [IdDistrito]  INT            NOT NULL,
    [Distrito]    NVARCHAR (100) NOT NULL,
    [CodigoDTA]   NVARCHAR (20)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

