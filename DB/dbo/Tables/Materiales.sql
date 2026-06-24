CREATE TABLE [dbo].[Materiales] (
    [IdMaterial]        INT             IDENTITY (1, 1) NOT NULL,
    [Nombre]            NVARCHAR (100)  NOT NULL,
    [Descripcion]       NVARCHAR (255)  NULL,
    [UnidadMedida]      NVARCHAR (50)   NULL,
    [StockActual]       INT             NULL,
    [StockMinimo]       INT             NULL,
    [PrecioUnitario]    DECIMAL (10, 2) NULL,
    [Codigo]            NVARCHAR (50)   NULL,
    [Categoria]         NVARCHAR (100)  NULL,
    [AlertaStockActiva] BIT             DEFAULT ((1)) NOT NULL,
    [Estado]            NVARCHAR (20)   DEFAULT ('Activo') NOT NULL,
    PRIMARY KEY CLUSTERED ([IdMaterial] ASC)
);

