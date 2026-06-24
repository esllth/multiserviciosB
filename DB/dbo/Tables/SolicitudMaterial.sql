CREATE TABLE [dbo].[SolicitudMaterial] (
    [IdSolicitud]        INT             IDENTITY (1, 1) NOT NULL,
    [OrdenId]            INT             NOT NULL,
    [MaterialId]         INT             NOT NULL,
    [EmpleadoId]         INT             NOT NULL,
    [CantidadSolicitada] DECIMAL (10, 2) NOT NULL,
    [FechaSolicitud]     DATETIME        DEFAULT (getdate()) NOT NULL,
    [Estado]             NVARCHAR (20)   DEFAULT ('Pendiente') NOT NULL,
    [Justificacion]      NVARCHAR (500)  NULL,
    [RespuestaAdmin]     NVARCHAR (500)  NULL,
    [FechaRespuesta]     DATETIME        NULL,
    PRIMARY KEY CLUSTERED ([IdSolicitud] ASC),
    CHECK ([CantidadSolicitada]>(0)),
    CHECK ([Estado]='Entregada' OR [Estado]='Rechazada' OR [Estado]='Aprobada' OR [Estado]='Pendiente')
);


GO
CREATE NONCLUSTERED INDEX [IX_SolicitudMaterial_EmpleadoId]
    ON [dbo].[SolicitudMaterial]([EmpleadoId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolicitudMaterial_MaterialId]
    ON [dbo].[SolicitudMaterial]([MaterialId] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_SolicitudMaterial_OrdenId]
    ON [dbo].[SolicitudMaterial]([OrdenId] ASC);

