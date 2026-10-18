param(
    [string]$GamePath = 'D:\Program Files\Rockstar Games\Grand Theft Auto V Legacy'
)
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$project = Join-Path $root 'LSlife.csproj'
$dependencies = @(
    @{ Name = 'ScriptHookVDotNet3'; Path = (Join-Path $GamePath 'ScriptHookVDotNet3.dll') },
    @{ Name = 'iFruitAddon2'; Path = (Join-Path $GamePath 'scripts\iFruitAddon2.dll') },
    @{ Name = 'LemonUI.SHVDN3'; Path = (Join-Path $GamePath 'scripts\LemonUI.SHVDN3.dll') }
)
$missing = $false
foreach ($dependency in $dependencies) {
    if (Test-Path $dependency.Path) {
        $file = Get-Item $dependency.Path
        Write-Host "OK $($dependency.Name) - $($file.VersionInfo.FileVersion)"
    } else {
        Write-Warning "MANQUANT : $($dependency.Path)"
        $missing = $true
    }
}
$cs = @(Get-ChildItem $root -Recurse -Filter '*.cs' -File | Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' })
$patterns = [ordered]@{
    'using NativeUI' = 'using\s+NativeUI\s*;'
    'using GTA' = 'using\s+GTA\s*;'
    'MenuPool' = '\bMenuPool\b'
    'UIMenu' = '\bUIMenu\b'
    'UIMenuItem' = '\bUIMenuItem\b'
    'UIMenuListItem' = '\bUIMenuListItem\b'
    'UIMenuSliderItem' = '\bUIMenuSliderItem\b'
    'UIMenuCheckboxItem' = '\bUIMenuCheckboxItem\b'
    'NativeUI events' = '\.On(ItemSelect|ListChanged|SliderChanged|CheckboxChange)\b'
    'UI.Notify' = '\bUI\.Notify\s*\('
    'UI.ShowSubtitle' = '\bUI\.ShowSubtitle\s*\('
}
Write-Host "Sources C# analysees : $($cs.Count)"
foreach ($entry in $patterns.GetEnumerator()) {
    $hits = 0
    $affected = 0
    foreach ($file in $cs) {
        $matches = [regex]::Matches([System.IO.File]::ReadAllText($file.FullName), $entry.Value)
        if ($matches.Count -gt 0) { $affected++ ; $hits += $matches.Count }
    }
    Write-Host ("{0,-23} {1,5} occurrences / {2,2} fichiers" -f $entry.Key, $hits, $affected)
}
Write-Host 'Audit uniquement : aucune source ni DLL modifiee.'
if ($missing) { exit 1 }
