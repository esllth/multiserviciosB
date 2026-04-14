# 🎯 Repositorio multiserviciosB
Repositorio del proyecto multiservicios B

# 📝 Diagrama entidad-relación db
## ER multiserviciosB

```mermaid
erDiagram
    Usuario {
        int id PK
        string nombre
        int rol_id FK
        string estado
    }
    
    Rol {
        int id PK
        string tipo_rol
        string estado
    }
    
    Cliente {
        int id PK
        string identificacion
        string nombre
        string apellidos
        string correo
        string telefono
        int direccion_id FK
        int rol_id FK
        string estado
    }
    
    Empleado {
        int id PK
        string identificacion
        string nombre
        string apellidos
        string correo
        string telefono
        int direccion_id FK
        int rol_id FK
        string estado
        float salario_base
        date fecha_inicio
        date fecha_finalizacion
    }
    
    Direccion {
        int id PK
        int pais_id FK
        int provincia_id FK
        int canton_id FK
        int distrito_id FK
        int localidad_id FK
        string otras_senas
        string estado
    }
    
    OrdenServicio {
        int id PK
        int cliente_id FK
        int tecnico_id FK
        datetime fecha_creacion
        datetime fecha_inicio
        datetime fecha_fin
        string estado
        string direccion
    }
    
    Factura {
        int id PK
        int orden_servicio_id FK
        string estado
    }
    
    Cotizacion {
        int id PK
        int cliente_id FK
        int tipo_servicio_id FK
        int estado_cotizacion_id FK
        string descripcion
        string notas_adicionales
        date fecha_solicitud
        float monto_presupuesto
        boolean aprobada_por_cliente
    }
    
    TipoServicio {
        int id PK
        string nombre
        string ubicacion
    }
    
    EstadoCotizacion {
        int id PK
        string nombre
        boolean activo
    }
    
    EstadoOrden {
        int id PK
        string nombre
        boolean activo
    }
    
    Tecnico {
        int id PK
        string nombre
        string apellido
        string correo
        string telefono
        string estado
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
    
    Evidencia {
        int id PK
        int orden_id FK
        string tipo
        string url_archivo
    }
    
    ObservacionTecnica {
        int id PK
        int orden_id FK
        int tecnico_id FK
        string descripcion
    }
    
    Notificacion {
        int id PK
        int cliente_id FK
        string titulo
        string mensaje
        int tipo_notificacion_id FK
        datetime fecha
        boolean leida
    }
    
    ParametroSistema {
        int id PK
        string nombre
        string valor
    }
    
    TipoNotificacion {
        int id PK
        string nombre
        string descripcion
    }
    
    %% Relaciones
    Usuario ||--|| Rol : "tiene"
    Cliente ||--|| Direccion : "tiene"
    Empleado ||--|| Direccion : "tiene"
    OrdenServicio ||--|| Cliente : "pertenece a"
    OrdenServicio ||--|| Tecnico : "asignado a"
    Factura ||--|| OrdenServicio : "genera"
    Cotizacion ||--|| Cliente : "solicita"
    Cotizacion ||--|| TipoServicio : "incluye"
    Cotizacion ||--|| EstadoCotizacion : "tiene"
    ConsumoMaterial ||--|| OrdenServicio : "registra"
    ConsumoMaterial ||--|| Material : "usa"
    Evidencia ||--|| OrdenServicio : "documenta"
    ObservacionTecnica ||--|| OrdenServicio : "comenta"
    ObservacionTecnica ||--|| Tecnico : "escribe"
    Notificacion ||--|| Cliente : "recibe"
    Notificacion ||--|| TipoNotificacion : "es de tipo"
