CREATE TABLE [dbo].[Clientes] (
    [IdCliente]      INT            IDENTITY (1, 1) NOT NULL,
    [Identificacion] NVARCHAR (50)  NOT NULL,
    [Nombre]         NVARCHAR (100) NOT NULL,
    [Apellidos]      NVARCHAR (100) NULL,
    [Correo]         NVARCHAR (150) NULL,
    [Telefono]       NVARCHAR (20)  NULL,
    [DireccionId]    INT            NULL,
    [Estado]         NVARCHAR (20)  NULL,
    [NombreNegocio]  NVARCHAR (150) NULL,
    PRIMARY KEY CLUSTERED ([IdCliente] ASC),
    FOREIGN KEY ([DireccionId]) REFERENCES [dbo].[Direccion] ([Id]),
    UNIQUE NONCLUSTERED ([Correo] ASC)
);

