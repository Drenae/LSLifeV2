param([switch]$Apply)
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$files = Get-ChildItem $root -Recurse -Filter '*.cs' -File | Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' }
$changes = @()
foreach ($file in $files) {
    $old = [IO.File]::ReadAllText($file.FullName)
    $new = $old
    # ILSpy emits explicit calls to implicit conversion operators; let C# resolve them.
    $new = [regex]::Replace($new, 'InputArgument\.op_Implicit\(', '(')
    # GTA enum values are unsigned at the metadata level; preserve their bit patterns.
    $new = [regex]::Replace($new, '\((Hash|PedHash|VehicleHash|FiringPattern|WeaponHash|BlipSprite|BlipColor|PickupType|VehicleClass|PedType|Relationship|Control|VehicleSeat)\)\((-\d+L?)\)', 'unchecked(($1)($2))')
    # Disambiguate GTA UI font from System.Drawing.Font.
    $new = [regex]::Replace($new, '(?<![\w.])\(Font\)', '(GTA.Font)')
    if ($file.Name -eq 'LSL.cs') {
        $new = $new.Replace('((Script)this).Interval = 3;', 'Interval = 3;')
    }
    if ($file.Name -eq 'AreaDealer.cs') {
        $new = [regex]::Replace($new, 'WeaponHash\[\] array = new WeaponHash\[4\];\s*RuntimeHelpers\.InitializeArray\(array, __ldtoken\([^;]+;\s*WeaponHash\[\] weaponHashes = array;', 'WeaponHash[] weaponHashes = { WeaponHash.Pistol, WeaponHash.MicroSMG, WeaponHash.SMG, WeaponHash.AssaultRifle };')
    }
    if ($new -ne $old) {
        $changes += $file.FullName.Substring($root.Length + 1)
        if ($Apply) { [IO.File]::WriteAllText($file.FullName, $new, [Text.UTF8Encoding]::new($false)) }
    }
}
$changes | ForEach-Object { Write-Host $_ }
Write-Host "$($changes.Count) fichiers concernés. $(if ($Apply) {'Corrections appliquées.'} else {'Prévisualisation uniquement : ajouter -Apply.'})"
