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
Smtp__FromName=Multiservicios Bolivar
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
EMAIL_FROM_NAME=Multiservicios Bolivar
```

Si `EMAIL_FROM` no se configura, la aplicacion usa `EMAIL_USER` como remitente.

Si `EMAIL_FROM` es diferente a `EMAIL_USER`, la aplicacion usa `EMAIL_USER` como remitente tecnico del sobre SMTP para evitar rechazos del servidor, pero conserva `EMAIL_FROM` como remitente visible del correo.

## Puertos soportados

- `465` usa SSL directo.
- `587` usa STARTTLS.

Ambos quedan soportados por el servicio SMTP de la aplicacion.

## Importante

No guardar la contrasena SMTP en `appsettings.json` ni en Git. Debe configurarse como variable de entorno o secreto del proveedor de hosting.
