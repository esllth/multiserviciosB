CREATE TABLE [dbo].[MaterialProyectoFabricacion] (
    [IdMaterialProyecto] INT             IDENTITY (1, 1) NOT NULL,
    [ProyectoId]         INT             NOT NULL,
    [MaterialId]         INT             NOT NULL,
    [CantidadRequerida]  DECIMAL (10, 2) NOT NULL,
    [CantidadUsada]      DECIMAL (10, 2) NULL,
    [Observaciones]      NVARCHAR (500)  NULL,
    PRIMARY KEY CLUSTERED ([IdMaterialProyecto] ASC),
    CHECK ([CantidadRequerida]>(0)),
    CHECK ([CantidadUsada]>=(0))
);


GO
CREATE NONCLUSTERED INDEX [IX_MaterialProyectoFabricacion_MaterialId]
    ON [dbo].[MaterialProyectoFabricacion]([MaterialId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_MaterialProyectoFabricacion_ProyectoId]
    ON [dbo].[MaterialProyectoFabricacion]([ProyectoId] ASC);

