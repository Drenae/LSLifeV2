param(
    [string]$GamePath = 'D:\Program Files\Rockstar Games\Grand Theft Auto V Legacy',
    [ValidateSet('Debug','Release')]
    [string]$Configuration = 'Release'
)
$ErrorActionPreference = 'Stop'
$dll = Join-Path $PSScriptRoot "bin\$Configuration\net48\LSlife.dll"
$log = Join-Path $PSScriptRoot 'build-baseline.log'
$requirements = @(
    @{Name='ScriptHookVDotNet2';Path=(Join-Path $GamePath 'ScriptHookVDotNet2.dll')},
    @{Name='NativeUI';Path=(Join-Path $GamePath 'scripts\NativeUI.dll')},
    @{Name='iFruitAddon2';Path=(Join-Path $GamePath 'scripts\iFruitAddon2.dll')}
)
$failed = $false
if (Test-Path $dll) {
    $info = Get-Item $dll
    Write-Host "DLL : $($info.FullName)"
    Write-Host "Taille : $($info.Length) octets"
    Write-Host "SHA256 : $((Get-FileHash $dll -Algorithm SHA256).Hash)"
} else { Write-Warning "DLL introuvable : $dll"; $failed = $true }
if (-not (Test-Path $log)) { Write-Warning "Journal introuvable : $log"; $failed = $true }
else {
    $last = Get-Content $log -Tail 20
    $last | Select-Object -Last 5
    if (($last -join "`n") -notmatch '(?im)^\s*(0 Erreur\(s\)|0 Error\(s\))\s*$') {
        Write-Warning 'Le journal ne confirme pas une compilation sans erreur.'
        $failed = $true
    }
}
foreach ($item in $requirements) {
    if (Test-Path $item.Path) { Write-Host "OK $($item.Name)" }
    else { Write-Warning "MANQUANT $($item.Name)"; $failed = $true }
}
if ($failed) { exit 1 }
Write-Host 'Verification statique OK. Le fonctionnement en jeu reste a tester.' -ForegroundColor Green
