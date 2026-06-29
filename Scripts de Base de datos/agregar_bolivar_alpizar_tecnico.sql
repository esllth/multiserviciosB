IF NOT EXISTS (
    SELECT 1
    FROM dbo.Empleados
    WHERE NombreEmpleado = N'Bolivar'
      AND ApellidosEmpleado = N'Alpizar'
)
BEGIN
    INSERT INTO dbo.Empleados (
        IdentificacionEmpleado,
        NombreEmpleado,
        ApellidosEmpleado,
        CorreoElectronicoEmpleado,
        TelefonoEmpleado,
        EstadoEmpleado,
        TieneUsuario,
        EstadoAcceso,
        SalarioBase,
        FechaInicioEmpleado
    )
    VALUES (
        N'BOLIVAR-ALPIZAR',
        N'Bolivar',
        N'Alpizar',
        N'bolivar.alpizar@multiserviciosb.com',
        N'0000-0000',
        1,
        0,
        N'Activo',
        0,
        SYSUTCDATETIME()
    );
END;
