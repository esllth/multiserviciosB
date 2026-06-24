CREATE TABLE [dbo].[ProyectosFabricacion] (
    [IdProyecto]            INT             IDENTITY (1, 1) NOT NULL,
    [ClienteId]             INT             NOT NULL,
    [Descripcion]           NVARCHAR (1000) NULL,
    [FechaInicio]           DATE            NULL,
    [FechaFin]              DATE            NULL,
    [Estado]                NVARCHAR (30)   NOT NULL,
    [NombreProyecto]        NVARCHAR (200)  DEFAULT ('') NOT NULL,
    [FechaSolicitud]        DATETIME2 (7)   DEFAULT (getdate()) NOT NULL,
    [FechaInicioEstimada]   DATETIME2 (7)   NULL,
    [FechaFinEstimada]      DATETIME2 (7)   NULL,
    [FechaInicioReal]       DATETIME2 (7)   NULL,
    [FechaFinReal]          DATETIME2 (7)   NULL,
    [CostoEstimado]         DECIMAL (12, 2) NULL,
    [CostoReal]             DECIMAL (12, 2) NULL,
    [DiseñoAprobado]        BIT             DEFAULT ((0)) NOT NULL,
    [FechaAprobacionDiseño] DATETIME2 (7)   NULL,
    [ObservacionesCliente]  NVARCHAR (1000) NULL,
    [ObservacionesInternas] NVARCHAR (1000) NULL,
    PRIMARY KEY CLUSTERED ([IdProyecto] ASC),
    FOREIGN KEY ([ClienteId]) REFERENCES [dbo].[Clientes] ([IdCliente])
);

