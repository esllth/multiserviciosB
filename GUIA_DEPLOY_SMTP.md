# Guia de deploy SMTP

La aplicacion acepta dos formatos de variables para SMTP en produccion.

## Dominio publico para enlaces de recuperacion

Configure esta variable con el dominio real de la aplicacion. Se usa para generar el enlace de `Restablecer contraseña` y la URL absoluta del logo en los correos.

```text
APP_BASE_URL=https://multiserviciosb.com
```

Tambien se acepta:

```text
PUBLIC_BASE_URL=https://multiserviciosb.com
App__PublicBaseUrl=https://multiserviciosb.com
```

## Formato recomendado para .NET

```text
Smtp__Host=smtp.hostinger.com
Smtp__Port=465
Smtp__EnableSsl=true
Smtp__FromEmail=noreply@multiserviciosb.com
Smtp__FromName=Multiservicios Bolívar
Smtp__UserName=noreply@multiserviciosb.com
Smtp__Password=CONTRASENA_SMTP
```

## Formato compatible con hostings

```text
EMAIL_HOST=smtp.hostinger.com
EMAIL_PORT=465
EMAIL_SECURE=true
EMAIL_USER=noreply@multiserviciosb.com
EMAIL_PASSWORD=CONTRASENA_SMTP
EMAIL_FROM=noreply@multiserviciosb.com
EMAIL_FROM_NAME=Multiservicios Bolívar
```

Si `EMAIL_FROM` no se configura, la aplicacion usa `EMAIL_USER` como remitente.

Si `EMAIL_FROM` es diferente a `EMAIL_USER`, la aplicacion usa `EMAIL_USER` como remitente tecnico del sobre SMTP para evitar rechazos del servidor, pero conserva `EMAIL_FROM` como remitente visible del correo.

## Puertos soportados

- `465` usa SSL directo.
- `587` usa STARTTLS.

Ambos quedan soportados por el servicio SMTP de la aplicacion.

## SMTP secundario de respaldo

La aplicacion usa Brevo automaticamente si el servidor SMTP principal falla por conexion, autenticacion, timeout o error de protocolo SMTP.

Datos configurados en el proyecto:

```text
SmtpFallback__Host=smtp-relay.brevo.com
SmtpFallback__Port=587
SmtpFallback__EnableSsl=true
SmtpFallback__FromEmail=notificacionesmultib@gmail.com
SmtpFallback__FromName=Multiservicios Bolívar
SmtpFallback__UserName=c4a322001@smtp-brevo.com
SmtpFallback__Password=CLAVE_SMTP_BREVO
```

En produccion, configure `SmtpFallback__Password` como variable de entorno secreta o mediante el gestor de secretos del hosting. No agregue la clave SMTP a `appsettings.json` ni al repositorio.

El remitente Gmail debe estar agregado y verificado como remitente en Brevo.

## Importante

No guardar la contrasena SMTP en `appsettings.json` ni en Git. Debe configurarse como variable de entorno o secreto del proveedor de hosting.
