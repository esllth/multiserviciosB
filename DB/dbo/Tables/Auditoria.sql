CREATE TABLE [dbo].[Auditoria] (
    [IdAuditoria] INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioId]   NVARCHAR (450) NOT NULL,
    [Accion]      NVARCHAR (100) NULL,
    [Fecha]       DATETIME       DEFAULT (getdate()) NULL,
    [Detalle]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([IdAuditoria] ASC),
    FOREIGN KEY ([UsuarioId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

