CREATE TABLE [dbo].[ObservacionesTecnicas] (
    [IdObservacion] INT            IDENTITY (1, 1) NOT NULL,
    [OrdenId]       INT            NOT NULL,
    [EmpleadoId]    INT            NOT NULL,
    [Descripcion]   NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([IdObservacion] ASC),
    FOREIGN KEY ([EmpleadoId]) REFERENCES [dbo].[Empleados] ([IdEmpleado]),
    FOREIGN KEY ([OrdenId]) REFERENCES [dbo].[OrdenesServicio] ([IdOrden])
);

