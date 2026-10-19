# LSL V2 FR - Patch et compilation NativeUI sans ouvrir Visual Studio.
# Usage : .\Build-NativeUI.ps1 -NativeUISource "E:\NativeUI"
param([string]$NativeUISource = "E:\NativeUI")
$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$root = [IO.Path]::GetFullPath($NativeUISource)
$project = Join-Path $root "NativeUI\NativeUI.csproj"
$config = Join-Path $root "NativeUI\packages.config"
# La version et le framework dependent du tag NativeUI (1.9.1 = 2.10.9/net40).
# Lire le chemin de reference directement dans le .csproj plutot que le coder en dur.
[xml]$projectXml = Get-Content -LiteralPath $project
$referenceNode = $projectXml.Project.ItemGroup.Reference | Where-Object { $_.Include -like "ScriptHookVDotNet2*" } | Select-Object -First 1
if (-not $referenceNode -or -not $referenceNode.HintPath) { throw "Reference ScriptHookVDotNet2 introuvable dans $project" }
$dependency = [IO.Path]::GetFullPath((Join-Path (Split-Path $project) ([string]$referenceNode.HintPath)))
$output = Join-Path $root "NativeUI\bin\x64\Release\NativeUI.dll"
$patch = Join-Path $PSScriptRoot "Patch-NativeUI-DescriptionWidth.ps1"
foreach ($file in @($project, $config, $patch)) {
    if (-not (Test-Path -LiteralPath $file -PathType Leaf)) { throw "Fichier introuvable : $file" }
}

Write-Host "[1/4] Detection de MSBuild..." -ForegroundColor Cyan
$vswhere = Join-Path ([Environment]::GetFolderPath("ProgramFilesX86")) "Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) { throw "vswhere.exe introuvable : MSBuild n'est pas installe." }
$msbuild = & $vswhere -latest -products * -requires Microsoft.Component.MSBuild -find "MSBuild\**\Bin\MSBuild.exe" | Select-Object -First 1
if (-not $msbuild -or -not (Test-Path $msbuild)) { throw "MSBuild introuvable." }

Write-Host "[2/4] Restauration de ScriptHookVDotNet2..." -ForegroundColor Cyan
if (-not (Test-Path $dependency)) {
    $nugetCommand = Get-Command nuget.exe -ErrorAction SilentlyContinue
    if ($nugetCommand) { $nuget = $nugetCommand.Source }
    else {
        $nuget = Join-Path $root ".tools\nuget.exe"
        if (-not (Test-Path $nuget)) {
            New-Item -ItemType Directory -Path (Split-Path $nuget) -Force | Out-Null
            [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
            Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile $nuget -UseBasicParsing
        }
    }
    & $nuget restore $config -PackagesDirectory (Join-Path $root "packages") -NonInteractive
    if ($LASTEXITCODE -ne 0) { throw "Echec de la restauration NuGet ($LASTEXITCODE)." }
}
if (-not (Test-Path $dependency)) { throw "Dependance manquante apres restauration : $dependency" }

Write-Host "[3/4] Correctif de largeur des descriptions..." -ForegroundColor Cyan
& $patch -NativeUISource $root
if (-not $?) { throw "Le correctif de largeur a echoue." }

Write-Host "[4/4] Compilation Release x64..." -ForegroundColor Cyan
& $msbuild $project /p:Configuration=Release /p:Platform=x64 /m /verbosity:minimal /nologo
if ($LASTEXITCODE -ne 0) { throw "Echec de compilation ($LASTEXITCODE)." }
if (-not (Test-Path $output)) { throw "DLL non generee : $output" }

Write-Host ""
Write-Host "Compilation reussie : $output" -ForegroundColor Green
Write-Host "Fermer GTA V et sauvegarder scripts\NativeUI.dll avant de remplacer la DLL du jeu." -ForegroundColor Yellow
