# Gang du joueur — audit et architecture du lot 1

Branche : `feature/player-gang`. Audit de la base `b92023c` ; aucune ancienne branche de gangs reprise.

## Reutilisation

| Systeme | Constat verifie | Choix |
|---|---|---|
| `Area`, `AreaLogic` | Quartier identifie par `Area.Name`, issu de `zoneData`. Chargement de `aRep` dans `LSLifeAreas.xml`. | Garder les objets et identifiants existants. Un territoire sera une reference de zone. |
| `LsFunctions.CurrentRepLvl` | Niveau = environ `0,04 * sqrt(points)` ; niveaux 10/8 proches de 62 500/40 000 points. | Source unique du niveau. Seuils distincts configurables au lot 2. |
| `DealerFunctions.MakeStreetSale` | Ventes joueur a pied / en voiture : reputation += 1 % du prix reel ; chemin commun pour les clients/livraisons. | Aucune seconde reputation. |
| `PlayerDealer.SimulateSales` | Ventes simulees avec stock disponible : reputation += 1 % du montant vendu. Le stock est retire. | Reutiliser les ventes reelles, ne pas accorder de maintien par simple presence. |
| `PlayerDealerHandler.OnSlowTick` | Simulation selon etat (hors mort/suivi) et heures ecoulees. Le filtre ne se limite pas a `Working`. | Auditer les etats et les chemins en presence du joueur au lot 2/3 avant de garantir le maintien. |
| `Shot.OnTick` | Une livraison manquee retire 10 % de la reputation locale positive. | Comportement conserve. Pas de nouvelle decroissance periodique. |
| `Gang`, `Area.Gang` | Gangs de quartiers lies aux menaces, dealers rivaux et drive-by. | Ne pas les remplacer ni en deriver le gang du joueur. Aucun spawn modifie. |
| Recrutement | `HandlePotentialWorker` / `AddDealer` pour client ; `DealerRecruitmentActions` / `ChangeAdealerToPdealer` pour rival. | Aucun nouveau recrutement. Rattachement par ID au lot 3, sans toucher aux donnees du dealer. L'offre monetaire du debauchage existant est conservee. |
| `PlayerDealer.ID`, `PlayerDealerHandler` | IDs entiers persistants, allocation et suppression existantes. | References d'IDs uniquement ; verifier reutilisation d'ID et nettoyage des references au lot 3. |
| `CustomMenu`, `CustomMenuLayout`, `CustomMenuShared` | Navigation et rendu compact reutilisables. F10 affiche l'inventaire ; le menu Zee vient du contact iFruit. | Ajouter « Mon gang » chez Zee et acces autonome F9 configurable. Protection commune contre le chevauchement des menus. |
| `XmlSaveFile` | Ecriture temporaire, flush, remplacement atomique avec `.bak`, refus de certains ecrasements dangereux. | Reutiliser sans modifier les sauvegardes existantes. |

## Architecture livree

- `PlayerGangProfile` : identite validee, GUID stable, chef logique `player`, date UTC, cle de couleur stable. Portee commune aux personnages, comme les fichiers LS Life actuels ; aucun handle de PNJ persiste.
- `PlayerGangStore` : XML versionne distinct `scripts/LSLife/LSLife_PlayerGang.xml`. Absence normale = aucun gang et aucune creation automatique de fichier. Chargement invalide = modifications bloquees avec possibilite de relecture apres restauration manuelle. Pas de restauration automatique d'un `.bak`.
- Creation et edition publiees en memoire uniquement apres sauvegarde reussie. Detection des changements externes depuis la lecture ; pas de promesse de verrouillage entre plusieurs processus.
- Edition d'identite preserve GUID, date, references et extensions XML inconnues. Aucun ecrasement des stocks, armes, recettes, reputation ou argent existants.
- `members/dealer/id` et `territories/territory/zone` preparent les references futures. Collections vides au lot 1 ; aucun dealer ou territoire attribue maintenant.
- `CustomPlayerGangMenu` : creation explicite, nom (2–24 caracteres), huit couleurs, annulation, modification de l'identite, consultation du chef et de l'ID. Les futurs lots sont affiches comme indisponibles, sans faux soldes ni revenus.
- Les modules suivants utiliseront la reputation vivante d'`Area`, les dealers vivants du handler et ces references. Le profil ne possede aucune copie de reputation, stock ou recettes.
- Tresorerie et transferts reserves au lot 4 ; aucune operation sur `Player.Money` dans ce lot.

## Limites et validations

Tests de persistance executables sans GTA : `dotnet run --project tests/PlayerGangPersistence` (SDK .NET 10). Ils utilisent les vrais fichiers du modele, du stockage et d'`XmlSaveFile`, sans remplacement de leur implementation.

Validation executee : 35 assertions de persistance reussies sur .NET 10. Compilation complete Release .NET Framework 4.8 x64 reussie avec SDK 10.0.403, DLL publiques SHVDN2 (distribution 3.6.0) et iFruitAddon2 3.1.1 : 0 erreur, 16 avertissements dans le code existant. Les DLL de reference proviennent des releases officielles des projets ; elles ne sont pas ajoutees au depot et ne sont pas a remplacer dans le jeu. Aucun test en jeu effectue.
