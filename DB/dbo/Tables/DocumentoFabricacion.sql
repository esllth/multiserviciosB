CREATE TABLE [dbo].[DocumentoFabricacion] (
    [IdDocumento]         INT            IDENTITY (1, 1) NOT NULL,
    [ProyectoId]          INT            NOT NULL,
    [NombreDocumento]     NVARCHAR (100) NOT NULL,
    [TipoDocumento]       NVARCHAR (50)  NOT NULL,
    [Ruta]                NVARCHAR (260) NOT NULL,
    [Descripcion]         NVARCHAR (500) NULL,
    [FechaCarga]          DATETIME       DEFAULT (getdate()) NOT NULL,
    [CargadoPorUsuarioId] NVARCHAR (450) NULL,
    PRIMARY KEY CLUSTERED ([IdDocumento] ASC),
    CHECK ([TipoDocumento]='Otro' OR [TipoDocumento]='Plano' OR [TipoDocumento]='Especificaciones' OR [TipoDocumento]='Diseño')
);


GO
CREATE NONCLUSTERED INDEX [IX_DocumentoFabricacion_ProyectoId]
    ON [dbo].[DocumentoFabricacion]([ProyectoId] ASC);

