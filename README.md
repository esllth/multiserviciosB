# 🎯 Repositorio multiserviciosB
Repositorio del proyecto multiservicios B

# 📝 Diagrama entidad-relación db
## ER multiserviciosB

```mermaid
erDiagram
    Usuario {
        int id PK
        string nombre
        string correo
        string password_hash
        int rol_id FK
        int empleado_id FK
        string estado
    }

    Rol {
        int id PK
        string nombre
        string descripcion
    }

    Empleado {
        int id PK
        string identificacion
        string nombre
        string apellidos
        string correo
        string telefono
        int direccion_id FK
        string estado
        float salario_base
        date fecha_inicio
        date fecha_fin
    }

    Cliente {
        int id PK
        string identificacion
        string nombre
        string apellidos
        string correo
        string telefono
        int direccion_id FK
        string estado
    }

    Direccion {
        int id PK
        int ubicacion_dta_id FK
        string otras_senas
    }

    UbicacionDTA {
        int id PK
        string provincia
        string canton
        string distrito
        string codigo_dta
    }

    Cotizacion {
        int id PK
        int cliente_id FK
        int tipo_servicio_id FK
        int estado_cotizacion_id FK
        string descripcion
        float monto_presupuesto
        date fecha_solicitud
        boolean aprobada_por_cliente
    }

    OrdenServicio {
        int id PK
        int cotizacion_id FK
        int cliente_id FK
        int empleado_id FK
        datetime fecha_creacion
        datetime fecha_inicio
        datetime fecha_fin
        int estado_orden_id FK
    }

    EstadoOrden {
        int id PK
        string nombre
    }

    EstadoCotizacion {
        int id PK
        string nombre
    }

    Material {
        int id PK
        string nombre
        string descripcion
        string unidad_medida
        int stock_actual
        int stock_minimo
        float precio_unitario
    }

    ConsumoMaterial {
        int id PK
        int orden_id FK
        int material_id FK
        float cantidad_usada
    }

    Equipo {
        int id PK
        string nombre
        string categoria
        string especificaciones
        string estado
    }

    ProyectoFabricacion {
        int id PK
        int cliente_id FK
        string descripcion
        date fecha_inicio
        date fecha_fin
        string estado
    }

    Evidencia {
        int id PK
        int orden_id FK
        string tipo
        string url_archivo
    }

    ObservacionTecnica {
        int id PK
        int orden_id FK
        int empleado_id FK
        string descripcion
    }

    Encuesta {
        int id PK
        int orden_id FK
        int cliente_id FK
        int calificacion_servicio
        int calificacion_tecnico
        string comentarios
        date fecha
    }

    Notificacion {
        int id PK
        int orden_id FK
        int cliente_id FK
        int material_id FK
        string titulo
        string mensaje
        datetime fecha
        boolean leida
    }

    Auditoria {
        int id PK
        int usuario_id FK
        string accion
        datetime fecha
        string detalle
    }

    %% Relaciones principales
    Usuario ||--|| Rol : "tiene"
    Usuario ||--|| Empleado : "corresponde a"
    Cliente ||--|| Direccion : "vive en"
    Empleado ||--|| Direccion : "reside en"
    Direccion ||--|| UbicacionDTA : "usa código"
    Cotizacion ||--|| Cliente : "solicita"
    Cotizacion ||--|| TipoServicio : "define"
    Cotizacion ||--|| EstadoCotizacion : "estado"
    OrdenServicio ||--|| Cotizacion : "proviene de"
    OrdenServicio ||--|| Cliente : "pertenece a"
    OrdenServicio ||--|| Empleado : "asignado a"
    ConsumoMaterial ||--|| OrdenServicio : "registra"
    ConsumoMaterial ||--|| Material : "usa"
    Evidencia ||--|| OrdenServicio : "documenta"
    ObservacionTecnica ||--|| OrdenServicio : "comenta"
    Encuesta ||--|| OrdenServicio : "evalúa"
    Encuesta ||--|| Cliente : "responde"
    Notificacion ||--|| OrdenServicio : "relacionada"
    Notificacion ||--|| Material : "alerta stock"
    Auditoria ||--|| Usuario : "registra acción"
