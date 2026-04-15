#💡 Diagrama de Infraestructura
```mermaid
graph TD
    subgraph "Zona Externa - Usuarios"
        User(Usuarios Finales: Clientes, Tecnicos, Admin)
    end

    subgraph "Perimetro de Seguridad"
        CDN(CDN / SSL TLS)
        WAF(WAF - Web App Firewall)
        FW(Firewall)
    end

    subgraph "Microsoft Azure - Nube Principal"
        
        subgraph "VPC Productiva"
            
            subgraph "Capa de Balanceo y Escalado"
                LB(Balanceador de Carga)
                ASG(Autoescalado)
            end

            subgraph "Capa de Aplicacion"
                App1(App Service - ASP.NET Core)
                App2(App Service - ASP.NET Core)
                App3(App Service - ASP.NET Core)
            end

            subgraph "Servicios Internos"
                Identity(Identity - Auth and Roles)
                LogicApp(Logica de Negocio)
                Notifications(Notificaciones)
            end

            subgraph "Capa de Datos"
                DB[(SQL Server - Base de Datos)]
                Storage[(Storage - Evidencias)]
                Backup[(Backup)]
            end

            subgraph "Monitoreo y Seguridad"
                Monitor(Azure Monitor)
                KMS(Key Vault)
            end
        end
        
        subgraph "VPC Staging - Desarrollo"
            Staging_App(App Service - Staging)
            Staging_DB[(SQL Server - Staging)]
            Repo(GitHub - Repositorio)
            CI(GitHub Actions - CI CD)
        end
    end

    subgraph "Servicios Externos"
        Geo(Google Maps API)
        Waze(Waze API)
        SMTP(Servidor SMTP)
    end

    User --> CDN
    CDN --> WAF --> FW
    FW --> LB
    LB --> ASG --> App1
    LB --> ASG --> App2
    LB --> ASG --> App3

    App1 --> Identity
    App2 --> Identity
    App3 --> Identity
    
    App1 --> LogicApp
    App2 --> LogicApp
    App3 --> LogicApp
    
    LogicApp --> Notifications
    LogicApp --> DB
    LogicApp --> Storage

    Monitor --> App1
    Monitor --> App2
    Monitor --> App3
    Monitor --> DB
    Monitor --> Storage
    
    KMS --> DB
    KMS --> Storage
    KMS --> Identity

    DB --> Backup
    Storage --> Backup

    Repo --> CI --> Staging_App
    CI --> App1

    LogicApp --> Geo
    LogicApp --> Waze
    Notifications --> SMTP
