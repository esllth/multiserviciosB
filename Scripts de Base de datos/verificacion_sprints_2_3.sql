-- =====================================================
-- Script de Verificación de Base de Datos
-- Sprints 2 y 3 - Módulos RT, RM, RE, RF
-- =====================================================

USE [MultiserviciosBDB];
GO

PRINT '=== VERIFICACIÓN DE TABLAS EXISTENTES ===';
PRINT '';

-- Verificar tablas nuevas
PRINT 'Verificando tablas nuevas...';
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FotosOrdenServicio')
    PRINT '✓ FotosOrdenServicio - OK'
ELSE
    PRINT '✗ FotosOrdenServicio - FALTA';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EventosOrdenServicio')
    PRINT '✓ EventosOrdenServicio - OK'
ELSE
    PRINT '✗ EventosOrdenServicio - FALTA';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SolicitudesMaterial')
    PRINT '✓ SolicitudesMaterial - OK'
ELSE
    PRINT '✗ SolicitudesMaterial - FALTA';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AlertasMantenimiento')
    PRINT '✓ AlertasMantenimiento - OK'
ELSE
    PRINT '✗ AlertasMantenimiento - FALTA';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DocumentosFabricacion')
    PRINT '✓ DocumentosFabricacion - OK'
ELSE
    PRINT '✗ DocumentosFabricacion - FALTA';

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MaterialesProyectoFabricacion')
    PRINT '✓ MaterialesProyectoFabricacion - OK'
ELSE
    PRINT '✗ MaterialesProyectoFabricacion - FALTA';

PRINT '';
PRINT '=== VERIFICACIÓN DE COLUMNAS NUEVAS ===';
PRINT '';

-- Verificar columnas en OrdenServicio
PRINT 'Verificando columnas en OrdenServicio...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdenServicio') AND name = 'FechaLlegadaSitio')
    PRINT '✓ FechaLlegadaSitio - OK'
ELSE
    PRINT '✗ FechaLlegadaSitio - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdenServicio') AND name = 'FechaAceptacionCliente')
    PRINT '✓ FechaAceptacionCliente - OK'
ELSE
    PRINT '✗ FechaAceptacionCliente - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdenServicio') AND name = 'ObservacionesTecnicas')
    PRINT '✓ ObservacionesTecnicas - OK'
ELSE
    PRINT '✗ ObservacionesTecnicas - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdenServicio') AND name = 'ComentariosFinales')
    PRINT '✓ ComentariosFinales - OK'
ELSE
    PRINT '✗ ComentariosFinales - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdenServicio') AND name = 'RequiereFotosObligatorias')
    PRINT '✓ RequiereFotosObligatorias - OK'
ELSE
    PRINT '✗ RequiereFotosObligatorias - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('OrdenServicio') AND name = 'LlegadaConfirmada')
    PRINT '✓ LlegadaConfirmada - OK'
ELSE
    PRINT '✗ LlegadaConfirmada - FALTA';

PRINT '';
PRINT 'Verificando columnas en Material...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Material') AND name = 'Codigo')
    PRINT '✓ Codigo - OK'
ELSE
    PRINT '✗ Codigo - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Material') AND name = 'Categoria')
    PRINT '✓ Categoria - OK'
ELSE
    PRINT '✗ Categoria - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Material') AND name = 'AlertaStockActiva')
    PRINT '✓ AlertaStockActiva - OK'
ELSE
    PRINT '✗ AlertaStockActiva - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Material') AND name = 'Estado')
    PRINT '✓ Estado - OK'
ELSE
    PRINT '✗ Estado - FALTA';

PRINT '';
PRINT 'Verificando columnas en Equipo...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'Codigo')
    PRINT '✓ Codigo - OK'
ELSE
    PRINT '✗ Codigo - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'TipoEquipo')
    PRINT '✓ TipoEquipo - OK'
ELSE
    PRINT '✗ TipoEquipo - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'Marca')
    PRINT '✓ Marca - OK'
ELSE
    PRINT '✗ Marca - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'Modelo')
    PRINT '✓ Modelo - OK'
ELSE
    PRINT '✗ Modelo - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'NumeroSerie')
    PRINT '✓ NumeroSerie - OK'
ELSE
    PRINT '✗ NumeroSerie - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'FechaAdquisicion')
    PRINT '✓ FechaAdquisicion - OK'
ELSE
    PRINT '✗ FechaAdquisicion - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'FrecuenciaMantenimientoDias')
    PRINT '✓ FrecuenciaMantenimientoDias - OK'
ELSE
    PRINT '✗ FrecuenciaMantenimientoDias - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'UltimoMantenimiento')
    PRINT '✓ UltimoMantenimiento - OK'
ELSE
    PRINT '✗ UltimoMantenimiento - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'ProximoMantenimiento')
    PRINT '✓ ProximoMantenimiento - OK'
ELSE
    PRINT '✗ ProximoMantenimiento - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Equipo') AND name = 'Observaciones')
    PRINT '✓ Observaciones - OK'
ELSE
    PRINT '✗ Observaciones - FALTA';

PRINT '';
PRINT 'Verificando columnas en ProyectoFabricacion...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'NombreProyecto')
    PRINT '✓ NombreProyecto - OK'
ELSE
    PRINT '✗ NombreProyecto - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'FechaSolicitud')
    PRINT '✓ FechaSolicitud - OK'
ELSE
    PRINT '✗ FechaSolicitud - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'FechaInicioEstimada')
    PRINT '✓ FechaInicioEstimada - OK'
ELSE
    PRINT '✗ FechaInicioEstimada - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'FechaFinEstimada')
    PRINT '✓ FechaFinEstimada - OK'
ELSE
    PRINT '✗ FechaFinEstimada - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'FechaInicioReal')
    PRINT '✓ FechaInicioReal - OK'
ELSE
    PRINT '✗ FechaInicioReal - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'FechaFinReal')
    PRINT '✓ FechaFinReal - OK'
ELSE
    PRINT '✗ FechaFinReal - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'CostoEstimado')
    PRINT '✓ CostoEstimado - OK'
ELSE
    PRINT '✗ CostoEstimado - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'CostoReal')
    PRINT '✓ CostoReal - OK'
ELSE
    PRINT '✗ CostoReal - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'DiseñoAprobado')
    PRINT '✓ DiseñoAprobado - OK'
ELSE
    PRINT '✗ DiseñoAprobado - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'FechaAprobacionDiseño')
    PRINT '✓ FechaAprobacionDiseño - OK'
ELSE
    PRINT '✗ FechaAprobacionDiseño - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'ObservacionesCliente')
    PRINT '✓ ObservacionesCliente - OK'
ELSE
    PRINT '✗ ObservacionesCliente - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ProyectoFabricacion') AND name = 'ObservacionesInternas')
    PRINT '✓ ObservacionesInternas - OK'
ELSE
    PRINT '✗ ObservacionesInternas - FALTA';

PRINT '';
PRINT 'Verificando columnas en ConsumoMaterial...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ConsumoMaterial') AND name = 'FechaRegistro')
    PRINT '✓ FechaRegistro - OK'
ELSE
    PRINT '✗ FechaRegistro - FALTA';

PRINT '';
PRINT 'Verificando columnas en HistorialEquipo...';
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HistorialEquipo') AND name = 'TipoServicio')
    PRINT '✓ TipoServicio - OK'
ELSE
    PRINT '✗ TipoServicio - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HistorialEquipo') AND name = 'EstadoAnterior')
    PRINT '✓ EstadoAnterior - OK'
ELSE
    PRINT '✗ EstadoAnterior - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HistorialEquipo') AND name = 'EstadoPosterior')
    PRINT '✓ EstadoPosterior - OK'
ELSE
    PRINT '✗ EstadoPosterior - FALTA';

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('HistorialEquipo') AND name = 'ObservacionesTecnico')
    PRINT '✓ ObservacionesTecnico - OK'
ELSE
    PRINT '✗ ObservacionesTecnico - FALTA';

PRINT '';
PRINT '=== CONTEO DE REGISTROS ===';
PRINT '';

-- Contar registros en tablas principales
DECLARE @CountOrdenServicio INT, @CountMaterial INT, @CountEquipo INT, @CountProyectoFabricacion INT;
DECLARE @CountFotos INT, @CountEventos INT, @CountSolicitudes INT, @CountAlertas INT;
DECLARE @CountDocumentos INT, @CountMaterialesProyecto INT;

SELECT @CountOrdenServicio = COUNT(*) FROM OrdenServicio;
SELECT @CountMaterial = COUNT(*) FROM Material;
SELECT @CountEquipo = COUNT(*) FROM Equipo;
SELECT @CountProyectoFabricacion = COUNT(*) FROM ProyectoFabricacion;

PRINT 'Órdenes de Servicio: ' + CAST(@CountOrdenServicio AS VARCHAR(10));
PRINT 'Materiales: ' + CAST(@CountMaterial AS VARCHAR(10));
PRINT 'Equipos: ' + CAST(@CountEquipo AS VARCHAR(10));
PRINT 'Proyectos de Fabricación: ' + CAST(@CountProyectoFabricacion AS VARCHAR(10));

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'FotosOrdenServicio')
BEGIN
    SELECT @CountFotos = COUNT(*) FROM FotosOrdenServicio;
    PRINT 'Fotos de Órdenes: ' + CAST(@CountFotos AS VARCHAR(10));
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'EventosOrdenServicio')
BEGIN
    SELECT @CountEventos = COUNT(*) FROM EventosOrdenServicio;
    PRINT 'Eventos de Órdenes: ' + CAST(@CountEventos AS VARCHAR(10));
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'SolicitudesMaterial')
BEGIN
    SELECT @CountSolicitudes = COUNT(*) FROM SolicitudesMaterial;
    PRINT 'Solicitudes de Material: ' + CAST(@CountSolicitudes AS VARCHAR(10));
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'AlertasMantenimiento')
BEGIN
    SELECT @CountAlertas = COUNT(*) FROM AlertasMantenimiento;
    PRINT 'Alertas de Mantenimiento: ' + CAST(@CountAlertas AS VARCHAR(10));
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'DocumentosFabricacion')
BEGIN
    SELECT @CountDocumentos = COUNT(*) FROM DocumentosFabricacion;
    PRINT 'Documentos de Fabricación: ' + CAST(@CountDocumentos AS VARCHAR(10));
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'MaterialesProyectoFabricacion')
BEGIN
    SELECT @CountMaterialesProyecto = COUNT(*) FROM MaterialesProyectoFabricacion;
    PRINT 'Materiales de Proyectos: ' + CAST(@CountMaterialesProyecto AS VARCHAR(10));
END

PRINT '';
PRINT '=== VERIFICACIÓN COMPLETADA ===';
PRINT '';
PRINT 'Revise los resultados anteriores para verificar que todas las tablas y columnas estén presentes.';
PRINT 'Si hay elementos faltantes (marcados con ✗), ejecute la migración de Entity Framework:';
PRINT 'dotnet ef database update --project MultiservicioB.csproj';
GO
