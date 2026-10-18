param(
    [string]$GamePath = 'D:\Program Files\Rockstar Games\Grand Theft Auto V Legacy'
)
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$log = Join-Path $PSScriptRoot 'migration-build.log'
Write-Host 'Compilation de diagnostic SHVDN3 + LemonUI (sans NativeUI).'
Write-Host 'Le projet de reference LSlife.csproj et la DLL du jeu restent inchanges.'
& dotnet build (Join-Path $PSScriptRoot 'LSlife.SHVDN3.csproj') --configuration Release --property:GtaVPath="$GamePath" --nologo *> $log
$code = $LASTEXITCODE
$errors = @(Get-Content $log | Where-Object { $_ -match ': error (CS\d+|MSB\d+|NETSDK\d+)' } | Sort-Object -Unique)
Write-Host "Journal complet : $log"
Write-Host "Erreurs distinctes : $($errors.Count)"
$errors | Select-Object -First 50 | ForEach-Object { Write-Host $_ }
if ($errors.Count -gt 50) { Write-Host "... $($errors.Count - 50) erreurs supplementaires dans le journal." }
if ($code -eq 0) { Write-Host 'Compilation SHVDN3 reussie (diagnostic).' }
else { Write-Host 'Erreurs attendues tant que les sources ne sont pas migrees.' }
exit $code
