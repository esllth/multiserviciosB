/*
    Crea la tabla usada por RT-004 para evidencia fotografica de ordenes.
    Es idempotente: si la tabla ya existe, no duplica nada.
*/

IF OBJECT_ID(N'dbo.FotoOrden', N'U') IS NULL
BEGIN
    IF OBJECT_ID(N'dbo.FotosOrdenServicio', N'U') IS NOT NULL
    BEGIN
        EXEC sp_rename N'dbo.FotosOrdenServicio', N'FotoOrden';
    END
    ELSE
    BEGIN
        CREATE TABLE [dbo].[FotoOrden] (
            [IdFotoOrden]    INT            IDENTITY (1, 1) NOT NULL,
            [OrdenId]        INT            NOT NULL,
            [Ruta]           NVARCHAR (260) NOT NULL,
            [NombreOriginal] NVARCHAR (150) NOT NULL,
            [TipoContenido]  NVARCHAR (50)  NOT NULL,
            [TipoFoto]       NVARCHAR (20)  NOT NULL,
            [FechaCarga]     DATETIME       CONSTRAINT [DF_FotoOrden_FechaCarga] DEFAULT (GETDATE()) NOT NULL,
            [Descripcion]    NVARCHAR (500) NULL,
            CONSTRAINT [PK_FotoOrden] PRIMARY KEY CLUSTERED ([IdFotoOrden] ASC),
            CONSTRAINT [CK_FotoOrden_TipoFoto] CHECK ([TipoFoto] = N'Final' OR [TipoFoto] = N'Inicial')
        );
    END
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = N'IX_FotoOrden_OrdenId'
      AND object_id = OBJECT_ID(N'dbo.FotoOrden')
)
BEGIN
    CREATE NONCLUSTERED INDEX [IX_FotoOrden_OrdenId]
        ON [dbo].[FotoOrden]([OrdenId] ASC);
END;

IF OBJECT_ID(N'dbo.OrdenesServicio', N'U') IS NOT NULL
   AND NOT EXISTS (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = N'FK_FotoOrden_OrdenesServicio_OrdenId'
   )
BEGIN
    ALTER TABLE [dbo].[FotoOrden] WITH CHECK
    ADD CONSTRAINT [FK_FotoOrden_OrdenesServicio_OrdenId]
        FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
        ON DELETE CASCADE;
END;
