# Pruebas automatizadas

La suite usa xUnit, ASP.NET Core `WebApplicationFactory` y EF Core SQLite en memoria. Incluye pruebas unitarias, pruebas de servicios con base de datos aislada y pruebas HTTP de rutas, autorización y encabezados de seguridad. Ninguna prueba requiere SQL Server, credenciales SMTP ni datos persistentes.

## Ejecutar

En Windows PowerShell:

```powershell
./scripts/test.ps1
```

En Linux o macOS:

```bash
./scripts/test.sh
```

También se puede ejecutar directamente:

```bash
dotnet test Tests/MultiservicioB.Tests.csproj --configuration Release --collect "XPlat Code Coverage"
```

Los resultados TRX y la cobertura Cobertura se guardan en `TestResults/`.

## Organización

- `Tests/Unit`: funciones y servicios comprobados sin una base de datos real.
- `Tests/Integration/Services`: servicios y persistencia ejecutados contra SQLite temporal.
- `Tests/Integration/Web`: solicitudes HTTP a la aplicación de prueba, con su inicialización de datos de producción desactivada.
- `.github/workflows/ci.yml`: restaura, compila, ejecuta pruebas y cobertura en cada push y pull request; además publica el artefacto de la aplicación en pushes.

Para añadir cobertura, crea pruebas junto al área correspondiente y usa bases SQLite independientes por prueba para evitar compartir estado. Los cambios a lógica de autenticación deben incluir casos de éxito, rechazo y expiración o consumo único cuando aplique.
