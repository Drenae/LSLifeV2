param(
    [string]$GamePath = 'D:\Program Files\Rockstar Games\Grand Theft Auto V Legacy',
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'LSlife.csproj'
$log = Join-Path $PSScriptRoot 'build-baseline.log'
if (-not (Test-Path $project)) { throw "Projet introuvable : $project" }
Write-Host "Compilation LS Life V2 ($Configuration)" -ForegroundColor Cyan
$oldPreference = $ErrorActionPreference
try {
    $ErrorActionPreference = 'Continue'
    & dotnet build $project --configuration $Configuration --property:GtaVPath="$GamePath" --nologo --verbosity minimal 2>&1 | Out-File -FilePath $log -Encoding utf8
    $code = $LASTEXITCODE
} finally {
    $ErrorActionPreference = $oldPreference
}
Get-Content $log | Select-Object -Last 25
Write-Host "Journal : $log"
if ($code -ne 0) { Write-Host "Compilation echouee ($code)" -ForegroundColor Red; exit $code }
$dll = Join-Path $PSScriptRoot "bin\$Configuration\net48\LSlife.dll"
if (-not (Test-Path $dll)) { throw "Compilation terminee, mais DLL introuvable : $dll" }
Write-Host "DLL generee : $dll" -ForegroundColor Green
