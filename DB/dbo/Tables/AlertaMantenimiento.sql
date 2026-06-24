CREATE TABLE [dbo].[AlertaMantenimiento] (
    [IdAlerta]           INT            IDENTITY (1, 1) NOT NULL,
    [EquipoId]           INT            NOT NULL,
    [FechaMantenimiento] DATETIME       NOT NULL,
    [TipoMantenimiento]  NVARCHAR (50)  NOT NULL,
    [Descripcion]        NVARCHAR (500) NULL,
    [Estado]             NVARCHAR (20)  DEFAULT ('Pendiente') NOT NULL,
    [FechaCreacion]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [FechaNotificacion]  DATETIME       NULL,
    [FechaRealizacion]   DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([IdAlerta] ASC),
    CHECK ([Estado]='Cancelada' OR [Estado]='Realizada' OR [Estado]='Notificada' OR [Estado]='Pendiente'),
    CHECK ([TipoMantenimiento]='Calibración' OR [TipoMantenimiento]='Correctivo' OR [TipoMantenimiento]='Preventivo')
);


GO
CREATE NONCLUSTERED INDEX [IX_AlertaMantenimiento_EquipoId]
    ON [dbo].[AlertaMantenimiento]([EquipoId] ASC);

