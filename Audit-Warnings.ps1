param(
    [string]$LogPath = (Join-Path $PSScriptRoot 'build-baseline.log')
)
$ErrorActionPreference = 'Stop'
if (-not (Test-Path $LogPath)) { throw "Journal introuvable : $LogPath. Lancez .\Build-Baseline.ps1" }
$lines = Get-Content $LogPath
$records = foreach ($line in $lines) {
    if ($line -match '^(?<file>.+?\.cs)\((?<line>\d+),(?<column>\d+)\):\s*warning\s+(?<code>[A-Z]+\d+):\s*(?<message>.*?)(?:\s+\[.+?\])?$') {
        [pscustomobject]@{File=$Matches.file;Line=[int]$Matches.line;Code=$Matches.code;Message=$Matches.message}
    }
}
$unique = @($records | Sort-Object File,Line,Code,Message -Unique)
Write-Host "Occurrences du journal : $(@($records).Count)"
Write-Host "Avertissements uniques : $($unique.Count)"
Write-Host 'Repartition par code :'
$unique | Group-Object Code | Sort-Object Count -Descending | Format-Table Count,Name -AutoSize
Write-Host 'Fichiers les plus touches :'
$unique | Group-Object File | Sort-Object Count -Descending | Select-Object -First 15 Count,Name | Format-Table -AutoSize
$csv = Join-Path $PSScriptRoot 'warnings-v2.csv'
$unique | Export-Csv -Path $csv -NoTypeInformation -Encoding UTF8 -Delimiter ';'
Write-Host "Rapport : $csv"
Write-Host 'Aucun avertissement n est masque ni supprime.'
