# LS Life V2

Remise en état et maintenance du mod **LS Life** pour **GTA V Legacy (mode histoire / solo)**.

## Objectif

Redonner vie à un mod qui n'était plus maintenu : le faire fonctionner, corriger les bugs, traduire les textes en français et apporter quelques améliorations utiles **sans le réécrire intégralement**.

## État du projet

- Base historique restaurée et fonctionnelle en jeu sur `main` (retour utilisateur).
- Référence stable : `v1.0.0-legacy-stable` / commit `957c680`.
- Conversion vers le système métrique engagée ; **chantier à reprendre en tout dernier**, après traduction, corrections de bugs et personnalisation légère.
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


## Alertes des employés (nouveau — à tester en jeu)

Les employés peuvent envoyer une notification avec le son SMS GTA V lorsqu'un produit passe sous le seuil de stock ou que la recette nette à récolter atteint le montant choisi. L'alerte « tout le stock est vide » remplace les trois alertes individuelles lorsqu'elles surviennent ensemble. Les alertes sont déclenchées sur les changements, pas à chaque frame ; la collecte réarme l'alerte recette, et le réapprovisionnement réarme l'alerte de stock.

Paramètres facultatifs à ajouter dans `scripts\\LSlife.ini` (les valeurs par défaut s'appliquent si la section est absente) :

```ini
[WORKER_ALERTS]
ENABLED = true
STOCK_THRESHOLD = 0
REVENUE_THRESHOLD = 10000
COOLDOWN_SECONDS = 30
```

- `STOCK_THRESHOLD` : quantité en **grammes** ; `0` = alerter à l'épuisement.
- `REVENUE_THRESHOLD` : montant en **dollars nets à récolter** (après commission de 20 %).
- `COOLDOWN_SECONDS` : intervalle minimum entre deux notifications, tous employés confondus.
- L'état initial d'un employé est observé sans générer d'alerte ; une transition ultérieure est nécessaire.
- Les notifications utilisent le système historique `SendTextMsg` (son SMS + notification GTA), **pas un message persistant dans la boîte de réception du téléphone**.
