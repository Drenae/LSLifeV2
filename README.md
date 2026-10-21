# LS Life V2

Remise en état et maintenance du mod **LS Life** pour **GTA V Legacy (mode histoire / solo)**.

## Objectif

Redonner vie à un mod qui n'était plus maintenu : le faire fonctionner, corriger les bugs, traduire les textes en français et apporter quelques améliorations utiles **sans le réécrire intégralement**.

## État du projet

- Base historique restaurée et fonctionnelle en jeu sur `main` (retour utilisateur).
- Référence stable : `v1.0.0-legacy-stable` / commit `957c680`.
- Conversion vers le système métrique engagée ; vérifier précisément son avancement dans le code avant toute nouvelle modification.
- La traduction française et les corrections de bugs restent des chantiers prioritaires.

## Environnement

- Jeu : GTA V **Legacy**, mode histoire.
- Langage : C# ; projet `LSlife.csproj`.
- Cible : **.NET Framework 4.8**, **x64**.
- API de script : **ScriptHookVDotNet2**.
- Menus : **NativeUI 1.9.1** d'origine.
- Autre dépendance : **iFruitAddon2**.
- Projet local habituel : `E:\LSLife`.
- Jeu local habituel : `D:\Program Files\Rockstar Games\Grand Theft Auto V Legacy`.

Les DLL externes sont référencées depuis l'installation locale du jeu ; elles ne sont pas fournies par ce dépôt.

## Compiler

```powershell
cd E:\LSLife
dotnet build .\LSlife.csproj -c Release
```

La compilation exige que les dépendances indiquées dans `LSlife.csproj` soient disponibles. Au besoin, passer `-p:GtaVPath="CHEMIN_DU_JEU"`.

## Organisation

- `LSlife/` : logique principale, menus et gameplay.
- `LSlife.Inventory/` : inventaire.
- `LSlife.Gangs/` : gangs.
- `LSlife.HeatBars/` : indicateurs de chaleur.
- `LSlife.Shots/` : tirs.
- `LSlife.OldCustomers/` : anciens clients.
- `Build-Baseline.ps1`, `Fix-ILSpy.ps1` : scripts historiques de maintenance.

## Documents

- [ROADMAP.md](ROADMAP.md) : priorités et suivi.
- [REGLES.md](REGLES.md) : règles impératives de maintenance.

## Principe

**Conserver ce qui marche, corriger ce qui ne marche pas et personnaliser seulement ce qui apporte une valeur réelle.**
