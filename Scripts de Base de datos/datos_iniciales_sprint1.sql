-- ====================================
-- Script de Datos Iniciales - Sprint 1
-- Módulos: Materiales, Equipos, Fabricación, Órdenes de Servicio
-- ====================================

USE MultiservicioDB
GO

-- ====================================
-- 1. ESTADOS DE COTIZACIÓN
-- ====================================
IF NOT EXISTS (SELECT 1 FROM EstadosCotizacion WHERE Nombre = 'Pendiente')
BEGIN
    INSERT INTO EstadosCotizacion (Nombre) VALUES ('Pendiente');
    PRINT 'Estado Cotización: Pendiente - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosCotizacion WHERE Nombre = 'Aprobada')
BEGIN
    INSERT INTO EstadosCotizacion (Nombre) VALUES ('Aprobada');
    PRINT 'Estado Cotización: Aprobada - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosCotizacion WHERE Nombre = 'Rechazada')
BEGIN
    INSERT INTO EstadosCotizacion (Nombre) VALUES ('Rechazada');
    PRINT 'Estado Cotización: Rechazada - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosCotizacion WHERE Nombre = 'En Revisión')
BEGIN
    INSERT INTO EstadosCotizacion (Nombre) VALUES ('En Revisión');
    PRINT 'Estado Cotización: En Revisión - Insertado';
END
GO

-- ====================================
-- 2. ESTADOS DE ORDEN DE SERVICIO
-- ====================================
IF NOT EXISTS (SELECT 1 FROM EstadosOrden WHERE Nombre = 'Pendiente')
BEGIN
    INSERT INTO EstadosOrden (Nombre) VALUES ('Pendiente');
    PRINT 'Estado Orden: Pendiente - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosOrden WHERE Nombre = 'En Progreso')
BEGIN
    INSERT INTO EstadosOrden (Nombre) VALUES ('En Progreso');
    PRINT 'Estado Orden: En Progreso - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosOrden WHERE Nombre = 'Completada')
BEGIN
    INSERT INTO EstadosOrden (Nombre) VALUES ('Completada');
    PRINT 'Estado Orden: Completada - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM EstadosOrden WHERE Nombre = 'Cancelada')
BEGIN
    INSERT INTO EstadosOrden (Nombre) VALUES ('Cancelada');
    PRINT 'Estado Orden: Cancelada - Insertado';
END
GO

-- ====================================
-- 3. TIPOS DE SERVICIO
-- ====================================
IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Mantenimiento Preventivo')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Mantenimiento Preventivo', 'Activo');
    PRINT 'Tipo Servicio: Mantenimiento Preventivo - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Mantenimiento Correctivo')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Mantenimiento Correctivo', 'Activo');
    PRINT 'Tipo Servicio: Mantenimiento Correctivo - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Instalación')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Instalación', 'Activo');
    PRINT 'Tipo Servicio: Instalación - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Reparación')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Reparación', 'Activo');
    PRINT 'Tipo Servicio: Reparación - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Fabricación a Medida')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Fabricación a Medida', 'Activo');
    PRINT 'Tipo Servicio: Fabricación a Medida - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Inspección')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Inspección', 'Activo');
    PRINT 'Tipo Servicio: Inspección - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM TiposServicio WHERE Nombre = 'Consultoría')
BEGIN
    INSERT INTO TiposServicio (Nombre, Estado) VALUES ('Consultoría', 'Activo');
    PRINT 'Tipo Servicio: Consultoría - Insertado';
END
GO

-- ====================================
-- 4. DATOS DE PRUEBA - UBICACIONES DTA (Ejemplo San José)
-- ====================================
IF NOT EXISTS (SELECT 1 FROM UbicacionDTA WHERE CodigoDTA = '1-01-01')
BEGIN
    INSERT INTO UbicacionDTA (IdProvincia, Provincia, IdCanton, Canton, IdDistrito, Distrito, CodigoDTA)
    VALUES (1, 'San José', 1, 'San José', 1, 'Carmen', '1-01-01');
    PRINT 'Ubicación DTA: San José, Carmen - Insertada';
END

IF NOT EXISTS (SELECT 1 FROM UbicacionDTA WHERE CodigoDTA = '1-01-02')
BEGIN
    INSERT INTO UbicacionDTA (IdProvincia, Provincia, IdCanton, Canton, IdDistrito, Distrito, CodigoDTA)
    VALUES (1, 'San José', 1, 'San José', 2, 'Merced', '1-01-02');
    PRINT 'Ubicación DTA: San José, Merced - Insertada';
END

IF NOT EXISTS (SELECT 1 FROM UbicacionDTA WHERE CodigoDTA = '1-01-03')
BEGIN
    INSERT INTO UbicacionDTA (IdProvincia, Provincia, IdCanton, Canton, IdDistrito, Distrito, CodigoDTA)
    VALUES (1, 'San José', 1, 'San José', 3, 'Hospital', '1-01-03');
    PRINT 'Ubicación DTA: San José, Hospital - Insertada';
END
GO

-- ====================================
-- 5. DATOS DE PRUEBA - DIRECCIONES
-- ====================================
DECLARE @UbicacionId1 INT = (SELECT TOP 1 Id FROM UbicacionDTA WHERE CodigoDTA = '1-01-01');

IF @UbicacionId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Direccion WHERE UbicacionDTAId = @UbicacionId1)
BEGIN
    INSERT INTO Direccion (UbicacionDTAId, OtrasSenas)
    VALUES (@UbicacionId1, '100 metros norte del parque central');
    PRINT 'Dirección de prueba 1 - Insertada';
END
GO

-- ====================================
-- 6. DATOS DE PRUEBA - CLIENTES
-- ====================================
DECLARE @DireccionId1 INT = (SELECT TOP 1 Id FROM Direccion);

IF @DireccionId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Clientes WHERE Identificacion = '1-1234-5678')
BEGIN
    INSERT INTO Clientes (Identificacion, Nombre, Apellidos, Correo, Telefono, DireccionId, Estado)
    VALUES ('1-1234-5678', 'Juan', 'Pérez González', 'juan.perez@example.com', '8888-8888', @DireccionId1, 'Activo');
    PRINT 'Cliente de prueba 1 - Insertado';
END

IF @DireccionId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Clientes WHERE Identificacion = '2-2345-6789')
BEGIN
    INSERT INTO Clientes (Identificacion, Nombre, Apellidos, Correo, Telefono, DireccionId, Estado)
    VALUES ('2-2345-6789', 'María', 'Rodríguez López', 'maria.rodriguez@example.com', '8777-7777', @DireccionId1, 'Activo');
    PRINT 'Cliente de prueba 2 - Insertado';
END
GO

-- ====================================
-- 7. DATOS DE PRUEBA - MATERIALES
-- ====================================
IF NOT EXISTS (SELECT 1 FROM Materiales WHERE Nombre = 'Tornillo 1/4"')
BEGIN
    INSERT INTO Materiales (Nombre, Descripcion, UnidadMedida, StockActual, StockMinimo, PrecioUnitario)
    VALUES ('Tornillo 1/4"', 'Tornillo hexagonal galvanizado', 'Unidad', 500, 100, 50.00);
    PRINT 'Material: Tornillo - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM Materiales WHERE Nombre = 'Cable Eléctrico #12')
BEGIN
    INSERT INTO Materiales (Nombre, Descripcion, UnidadMedida, StockActual, StockMinimo, PrecioUnitario)
    VALUES ('Cable Eléctrico #12', 'Cable eléctrico calibre 12 AWG', 'Metro', 1000, 200, 850.00);
    PRINT 'Material: Cable Eléctrico - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM Materiales WHERE Nombre = 'Tubo PVC 1"')
BEGIN
    INSERT INTO Materiales (Nombre, Descripcion, UnidadMedida, StockActual, StockMinimo, PrecioUnitario)
    VALUES ('Tubo PVC 1"', 'Tubo PVC presión 1 pulgada', 'Metro', 50, 30, 1200.00);
    PRINT 'Material: Tubo PVC - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM Materiales WHERE Nombre = 'Soldadura Estaño')
BEGIN
    INSERT INTO Materiales (Nombre, Descripcion, UnidadMedida, StockActual, StockMinimo, PrecioUnitario)
    VALUES ('Soldadura Estaño', 'Rollo soldadura estaño 60/40', 'Unidad', 15, 20, 4500.00);
    PRINT 'Material: Soldadura (BAJO STOCK) - Insertado';
END

IF NOT EXISTS (SELECT 1 FROM Materiales WHERE Nombre = 'Aceite Lubricante')
BEGIN
    INSERT INTO Materiales (Nombre, Descripcion, UnidadMedida, StockActual, StockMinimo, PrecioUnitario)
    VALUES ('Aceite Lubricante', 'Aceite lubricante industrial', 'Litro', 80, 50, 3200.00);
    PRINT 'Material: Aceite Lubricante - Insertado';
END
GO

-- ====================================
-- 8. DATOS DE PRUEBA - EQUIPOS
-- ====================================
DECLARE @ClienteId1 INT = (SELECT TOP 1 IdCliente FROM Clientes WHERE Identificacion = '1-1234-5678');

IF @ClienteId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Equipos WHERE Nombre = 'Compresor Industrial')
BEGIN
    INSERT INTO Equipos (Nombre, Categoria, Especificaciones, Estado, ClienteId)
    VALUES ('Compresor Industrial', 'Neumático', 'Compresor 10HP, 300L, 175PSI', 'Activo', @ClienteId1);
    PRINT 'Equipo: Compresor - Insertado';
END

IF @ClienteId1 IS NOT NULL AND NOT EXISTS (SELECT 1 FROM Equipos WHERE Nombre = 'Motor Eléctrico 5HP')
BEGIN
    INSERT INTO Equipos (Nombre, Categoria, Especificaciones, Estado, ClienteId)
    VALUES ('Motor Eléctrico 5HP', 'Eléctrico', 'Motor trifásico 5HP, 220V', 'Activo', @ClienteId1);
    PRINT 'Equipo: Motor Eléctrico - Insertado';
END
GO

-- ====================================
-- 9. VERIFICACIÓN DE DATOS
-- ====================================
PRINT '=================================================='
PRINT 'RESUMEN DE DATOS INSERTADOS'
PRINT '=================================================='

SELECT 'Estados Cotización' AS Tabla, COUNT(*) AS Total FROM EstadosCotizacion
UNION ALL
SELECT 'Estados Orden', COUNT(*) FROM EstadosOrden
UNION ALL
SELECT 'Tipos Servicio', COUNT(*) FROM TiposServicio
UNION ALL
SELECT 'Ubicaciones DTA', COUNT(*) FROM UbicacionDTA
UNION ALL
SELECT 'Direcciones', COUNT(*) FROM Direccion
UNION ALL
SELECT 'Clientes', COUNT(*) FROM Clientes
UNION ALL
SELECT 'Materiales', COUNT(*) FROM Materiales
UNION ALL
SELECT 'Equipos', COUNT(*) FROM Equipos;

PRINT '=================================================='
PRINT 'Script de inicialización completado exitosamente'
PRINT '=================================================='
GO
