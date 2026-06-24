CREATE TABLE [dbo].[EventoOrdenServicio] (
    [IdEvento]    INT             IDENTITY (1, 1) NOT NULL,
    [OrdenId]     INT             NOT NULL,
    [TipoEvento]  NVARCHAR (50)   NOT NULL,
    [FechaEvento] DATETIME        DEFAULT (getdate()) NOT NULL,
    [Descripcion] NVARCHAR (1000) NULL,
    [Latitud]     DECIMAL (10, 7) NULL,
    [Longitud]    DECIMAL (10, 7) NULL,
    [UsuarioId]   NVARCHAR (450)  NULL,
    PRIMARY KEY CLUSTERED ([IdEvento] ASC),
    CHECK ([TipoEvento]='AceptacionCliente' OR [TipoEvento]='ComentarioFinal' OR [TipoEvento]='FinalizacionServicio' OR [TipoEvento]='ObservacionTecnica' OR [TipoEvento]='InicioServicio' OR [TipoEvento]='LlegadaSitio')
);


GO
CREATE NONCLUSTERED INDEX [IX_EventoOrdenServicio_OrdenId]
    ON [dbo].[EventoOrdenServicio]([OrdenId] ASC);

