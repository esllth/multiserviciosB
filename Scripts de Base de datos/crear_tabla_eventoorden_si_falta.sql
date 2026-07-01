/*
    Crea la tabla usada por ordenes de servicio para eventos RT.
    Es idempotente: si ya existe, no duplica nada.
*/

IF OBJECT_ID(N'dbo.EventoOrdenServicio', N'U') IS NULL
BEGIN
    IF OBJECT_ID(N'dbo.EventosOrdenServicio', N'U') IS NOT NULL
    BEGIN
        EXEC sp_rename N'dbo.EventosOrdenServicio', N'EventoOrdenServicio';
    END
    ELSE
    BEGIN
        CREATE TABLE [dbo].[EventoOrdenServicio] (
            [IdEvento]    INT             IDENTITY (1, 1) NOT NULL,
            [OrdenId]     INT             NOT NULL,
            [TipoEvento]  NVARCHAR (50)   NOT NULL,
            [FechaEvento] DATETIME        CONSTRAINT [DF_EventoOrdenServicio_FechaEvento] DEFAULT (GETDATE()) NOT NULL,
            [Descripcion] NVARCHAR (1000) NULL,
            [Latitud]     DECIMAL (10, 7) NULL,
            [Longitud]    DECIMAL (10, 7) NULL,
            [UsuarioId]   NVARCHAR (450)  NULL,
            CONSTRAINT [PK_EventoOrdenServicio] PRIMARY KEY CLUSTERED ([IdEvento] ASC),
            CONSTRAINT [CK_EventoOrdenServicio_TipoEvento] CHECK (
                [TipoEvento] = N'AceptacionCliente' OR
                [TipoEvento] = N'ComentarioFinal' OR
                [TipoEvento] = N'FinalizacionServicio' OR
                [TipoEvento] = N'ObservacionTecnica' OR
                [TipoEvento] = N'InicioServicio' OR
                [TipoEvento] = N'LlegadaSitio'
            )
        );
    END
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_EventoOrdenServicio_OrdenId'
      AND object_id = OBJECT_ID(N'dbo.EventoOrdenServicio')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_EventoOrdenServicio_OrdenId]
        ON [dbo].[EventoOrdenServicio]([OrdenId] ASC);
END;

IF OBJECT_ID(N'dbo.OrdenesServicio', N'U') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = N'FK_EventoOrdenServicio_OrdenesServicio_OrdenId'
   )
BEGIN
    ALTER TABLE [dbo].[EventoOrdenServicio] WITH CHECK
    ADD CONSTRAINT [FK_EventoOrdenServicio_OrdenesServicio_OrdenId]
        FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
        ON DELETE CASCADE;
END;
