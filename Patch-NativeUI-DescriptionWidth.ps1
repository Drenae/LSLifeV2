# LSL V2 FR : correctif local de NativeUI (Guad/NativeUI, branche master).
# Ne modifie pas le jeu ni la DLL installee. A executer sur une copie du depot officiel.
param(
    [Parameter(Mandatory = $true)]
    [string]$NativeUISource
)

$ErrorActionPreference = 'Stop'
$source = Join-Path $NativeUISource 'NativeUI\UIMenu.cs'
if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
    throw "Fichier introuvable : $source. Cloner https://github.com/Guad/NativeUI puis transmettre son dossier racine."
}

$original = [System.IO.File]::ReadAllText($source)
# LSL V2 FR : les deux limites de texte doivent suivre l'offset du menu.
$oldWrap = '_descriptionText.Wrap = 400;'
$newWrap = '_descriptionText.Wrap = 400 + WidthOffset;'
$oldCount = 'Screen.GetLineCount(descCaption, _descriptionText.Position, _descriptionText.Font, _descriptionText.Scale, 400);'
$newCount = 'Screen.GetLineCount(descCaption, _descriptionText.Position, _descriptionText.Font, _descriptionText.Scale, 400 + WidthOffset);'

if ($original.Contains($newWrap) -and $original.Contains($newCount)) {
    Write-Host 'Correctif deja applique ; aucune modification.' -ForegroundColor Yellow
    exit 0
}
if (($original.Split(@($oldWrap), [System.StringSplitOptions]::None).Count -ne 2) -or
    ($original.Split(@($oldCount), [System.StringSplitOptions]::None).Count -ne 2)) {
    throw 'Version du code source differente : modification annulee pour eviter un patch incorrect.'
}

$patched = $original.Replace($oldWrap, $newWrap).Replace($oldCount, $newCount)
$backup = "$source.lslv2.bak"
if (-not (Test-Path -LiteralPath $backup)) {
    [System.IO.File]::Copy($source, $backup)
}
[System.IO.File]::WriteAllText($source, $patched, (New-Object System.Text.UTF8Encoding($true)))
Write-Host 'Correctif applique a NativeUI/UIMenu.cs (400 + WidthOffset).' -ForegroundColor Green
Write-Host "Sauvegarde du source : $backup"
Write-Host 'Compiler maintenant NativeUI en Release x64 (framework .NET 4.8) depuis Visual Studio.'
Write-Host 'Avant de tester la DLL reconstruite, sauvegarder scripts\NativeUI.dll et quitter GTA V.'
