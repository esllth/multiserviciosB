CREATE TABLE [dbo].[Empleados] (
    [IdEmpleado]                INT             IDENTITY (1, 1) NOT NULL,
    [IdentificacionEmpleado]    NVARCHAR (50)   NOT NULL,
    [NombreEmpleado]            NVARCHAR (100)  NOT NULL,
    [ApellidosEmpleado]         NVARCHAR (100)  NOT NULL,
    [CorreoElectronicoEmpleado] NVARCHAR (150)  NOT NULL,
    [TelefonoEmpleado]          NVARCHAR (20)   NOT NULL,
    [DireccionId]               INT             NULL,
    [EstadoEmpleado]            BIT             NOT NULL,
    [TieneUsuario]              BIT             NOT NULL,
    [SalarioBase]               DECIMAL (10, 2) NOT NULL,
    [FechaInicioEmpleado]       DATE            NOT NULL,
    [FechaFinalizacionEmpleado] DATE            NULL,
    [UserId]                    NVARCHAR (450)  NULL,
    [EstadoAcceso]              NVARCHAR (30)   CONSTRAINT [DF_Empleados_EstadoAcceso] DEFAULT (N'PendienteRegistro') NOT NULL,
    [FotoPerfil]                NVARCHAR (300)  NULL,
    CONSTRAINT [PK_Empleados] PRIMARY KEY CLUSTERED ([IdEmpleado] ASC),
    CONSTRAINT [FK_Empleados_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Empleados_Direccion] FOREIGN KEY ([DireccionId]) REFERENCES [dbo].[Direccion] ([Id])
);

