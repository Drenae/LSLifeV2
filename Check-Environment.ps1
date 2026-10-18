param(
    [string]$GamePath = 'D:\Program Files\Rockstar Games\Grand Theft Auto V Legacy'
)
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'LSlife.csproj'
$scriptDir = Join-Path $GamePath 'scripts'
Write-Host 'LS Life V2 - verification de l environnement' -ForegroundColor Cyan
Write-Host "Projet : $project"
if (-not (Test-Path $project)) { throw "Projet introuvable : $project" }
$required = @(
    @{Name='ScriptHookVDotNet2';Path=(Join-Path $GamePath 'ScriptHookVDotNet2.dll')},
    @{Name='NativeUI';Path=(Join-Path $scriptDir 'NativeUI.dll')},
    @{Name='iFruitAddon2';Path=(Join-Path $scriptDir 'iFruitAddon2.dll')}
)
$missing = $false
foreach ($item in $required) {
    if (Test-Path $item.Path) {
        Write-Host "OK $($item.Name) : $($item.Path)" -ForegroundColor Green
    } else {
        Write-Warning "MANQUANT $($item.Name) : $($item.Path)"
        $missing = $true
    }
}
$dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
if ($dotnet) { Write-Host "OK dotnet : $($dotnet.Source)" }
else { Write-Warning 'dotnet introuvable'; $missing = $true }
if ($missing) { exit 1 }
Write-Host 'Environnement de compilation pret.' -ForegroundColor Green
