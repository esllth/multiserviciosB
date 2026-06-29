# Recuperacion de contrasena por correo

El flujo ya esta conectado con ASP.NET Identity:

1. El usuario entra a `Olvide mi contrasena`.
2. Ingresa su correo.
3. El sistema genera un token de recuperacion con expiracion de 30 minutos.
4. Envia un enlace al correo configurado.
5. El usuario define una nueva contrasena desde el enlace.

## Configuracion requerida

Configure la seccion `Smtp` en `appsettings.json`, variables de entorno o User Secrets.

Ejemplo de estructura:

```json
"Smtp": {
  "Host": "smtp.tu-proveedor.com",
  "Port": 587,
  "EnableSsl": true,
  "FromEmail": "no-reply@multiserviciosb.com",
  "FromName": "Multiservicios Bolivar",
  "UserName": "no-reply@multiserviciosb.com",
  "Password": "CONTRASENA_O_APP_PASSWORD"
}
```

Para Gmail/Google Workspace normalmente se usa:

```json
"Host": "smtp.gmail.com",
"Port": 587,
"EnableSsl": true
```

Use una contrasena de aplicacion, no la contrasena normal de la cuenta.

## Prueba en desarrollo

Si SMTP no esta configurado y la app corre en Development, la pantalla de confirmacion muestra un enlace temporal de prueba.
Ese enlace no se muestra en produccion.
