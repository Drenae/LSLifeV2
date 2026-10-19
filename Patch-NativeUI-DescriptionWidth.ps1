# LSL V2 FR - Patch specifique a Guad/NativeUI 1.9.1.
# Modifie uniquement FormatDescription() dans une copie locale des sources.
param([Parameter(Mandatory=$true)][string]$NativeUISource)
$ErrorActionPreference = 'Stop'
$source = Join-Path $NativeUISource 'NativeUI\UIMenu.cs'
if (-not (Test-Path -LiteralPath $source)) { throw "Source introuvable : $source" }
$original = [IO.File]::ReadAllText($source)
$start = $original.IndexOf('        private string FormatDescription(string input)')
$end = $original.IndexOf('        #endregion', $start)
if ($start -lt 0 -or $end -lt 0) { throw 'FormatDescription introuvable : version de NativeUI incompatible.' }
$oldMethod = $original.Substring($start, $end - $start)
if ($oldMethod.Contains('LSLV2_DESCRIPTION_PATCH')) {
    Write-Host 'Correctif deja applique.' -ForegroundColor Yellow
    exit 0
}
if (-not $oldMethod.Contains('425 + WidthOffset') -or -not $oldMethod.Contains("input.Split(' ')")) {
    throw 'La methode ne correspond pas a NativeUI 1.9.1. Aucun changement.'
}
$newMethod = @'
        // LSLV2_DESCRIPTION_PATCH : respecte les sauts de ligne GTA et la largeur reelle.
        private string FormatDescription(string input)
        {
            if (String.IsNullOrEmpty(input)) return input;
            int maxPixelsPerLine = 425 + WidthOffset;
            var output = new System.Text.StringBuilder();
            // ~n~ est le saut de ligne GTA ; il ne doit jamais etre mesure comme un mot.
            string normalized = input.Replace("~n~", "\n").Replace("\r\n", "\n");
            string[] lines = normalized.Split('\n');
            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                if (lineIndex > 0) output.Append("\n");
                int pixels = 0;
                string[] words = lines[lineIndex].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string word in words)
                {
                    // Les codes de couleur GTA (~g~, ~r~, ~s~...) ne sont pas visibles.
                    string visible = System.Text.RegularExpressions.Regex.Replace(word, @"~[^~]*~", "");
                    int wordPixels = (int)StringMeasurer.MeasureString(visible, (Font)0, 0.35f);
                    int spacePixels = (int)StringMeasurer.MeasureString(" ");
                    if (pixels > 0 && pixels + spacePixels + wordPixels > maxPixelsPerLine)
                    {
                        output.Append("\n");
                        pixels = 0;
                    }
                    else if (pixels > 0)
                    {
                        output.Append(' ');
                        pixels += spacePixels;
                    }
                    output.Append(word);
                    pixels += wordPixels;
                }
            }
            return output.ToString();
        }

'@
$backup = "$source.lslv2.bak"
if (-not (Test-Path $backup)) { [IO.File]::Copy($source, $backup) }
$updated = $original.Substring(0, $start) + $newMethod + $original.Substring($end)
[IO.File]::WriteAllText($source, $updated, (New-Object Text.UTF8Encoding($true)))
Write-Host 'Correctif NativeUI 1.9.1 applique.' -ForegroundColor Green
