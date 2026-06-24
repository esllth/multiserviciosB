CREATE TABLE [dbo].[Horarios] (
    [Id]         INT           IDENTITY (1, 1) NOT NULL,
    [DiaSemana]  NVARCHAR (20) NOT NULL,
    [HoraInicio] TIME (7)      NOT NULL,
    [HoraFin]    TIME (7)      NOT NULL,
    [Activo]     BIT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Horarios] PRIMARY KEY CLUSTERED ([Id] ASC)
);

