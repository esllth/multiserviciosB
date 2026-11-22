param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"
$repositoryRoot = Split-Path -Parent $PSScriptRoot
$testProject = Join-Path $repositoryRoot "Tests\MultiservicioB.Tests.csproj"
$resultsDirectory = Join-Path $repositoryRoot "TestResults"

dotnet test $testProject `
    --configuration $Configuration `
    --logger "console;verbosity=normal" `
    --collect "XPlat Code Coverage" `
    --results-directory $resultsDirectory

if ($LASTEXITCODE -ne 0) {
    throw "Las pruebas automatizadas finalizaron con errores."
}
