IF NOT EXISTS (
    SELECT 1
    FROM dbo.Empleados
    WHERE NombreEmpleado = N'Bolívar'
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
        N'Bolívar-ALPIZAR',
        N'Bolívar',
        N'Alpizar',
        N'Bolívar.alpizar@multiserviciosb.com',
        N'0000-0000',
        1,
        0,
        N'Activo',
        0,
        SYSUTCDATETIME()
    );
END;
