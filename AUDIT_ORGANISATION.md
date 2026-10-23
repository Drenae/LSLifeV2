# Audit d'organisation — LS Life V2

Date : 23 octobre 2026. Base : branche main, commit 6347c93. Travail : refactor/organisation-code.

## Objectif et périmètre
Déplacer le code existant vers des fichiers thématiques, sans changer les signatures, les appels, les règles métier, les sauvegardes, les dépendances ni les coordonnées des interfaces. Ne pas réécrire le mod. Conversion métrique en dernier.

## Inventaire du dépôt
- LSlife/LSL.cs : 4 578 lignes avant déplacement ; classe principale, initialisation, boucle de jeu, événements, gestion du joueur, police, dealers, quartiers et quatre panneaux de diagnostic.
- LSlife/LsFunctions.cs : 2 089 lignes ; mélange utilitaires, calculs, inventaires, sauvegardes, monde, police et gameplay.
- LSlife/LsMenus.cs : 1 799 lignes ; menus et callbacks de domaines multiples.
- LSlife/InventoryText.cs : HUD argent/stocks/vente ; conserver le rendu natif et ses positions.
- LSlife/Area.cs, AreaDealer.cs, Dealer.cs, DrugDealer.cs, Customer.cs, CustomerPed.cs, PlayerDealer.cs, PlayerDealerHandler.cs : quartiers, dealers et clients.
- LSlife/StashHouse.cs, StashHouseHandler.cs, StashVehicle.cs, DrugOrder.cs, DropDrugs.cs, DroppedBag.cs : stockage et commandes.
- LSlife/Pig.cs, Prison.cs, EnemyHandler.cs, DriveBy.cs, DriveByHandler.cs : police, prison et affrontements.
- LSlife.Gangs/ : Gang, GangUtilities, Threat, ThreatHandler.
- LSlife.HeatBars/ : AreaHeatBar, BarHandler, PlayerHeatBar.
- LSlife.Inventory/ : objets d'inventaire et Cocaine.
- LSlife.OldCustomers/ : OldCustomer, OldCustomerHandler.
- LSlife.Shots/ : Shot, ShotHandler.
- LSlife/LsTimer.cs, Enums.cs, PedTeam.cs, PedDebugger.cs, PickupHandler.cs, HeadShot.cs, HeadShotHandler.cs, HirePed.cs, DefaultPed.cs, DealerSpawnPostion.cs, LsLPeds.cs : composants spécialisés existants.
- Build-Baseline.ps1 et Fix-ILSpy.ps1 : outils de maintenance ; ne pas déplacer sans vérifier leurs chemins.
- assets/hud_background.png : ressource historique toujours déclarée dans le csproj, non utilisée par le HUD natif actuel. Ne pas supprimer dans ce chantier.

## Organisation cible progressive
- LSlife/UI/ : interfaces, HUD de diagnostic et autres méthodes d'affichage actuellement imbriquées dans LSL.
- LSlife/Core/ : orchestration, initialisation, événements.
- LSlife/World/ : quartiers et logique spatiale.
- LSlife/Player/ : logique du joueur.
- LSlife/Dealing/ : clients, dealers, ventes et commandes.
- LSlife/Services/ : fonctions utilitaires et persistance.
- Conserver provisoirement les répertoires thématiques existants ; ne pas les déplacer uniquement pour harmoniser les noms.

## Règles de déplacement
1. Créer une branche de travail ; main et backup/avant-reorganisation-2026-10-23 restent intacts.
2. Préférer les classes partielles pour extraire les méthodes privées de LSL : cela évite de changer leur accès aux champs et aux appels.
3. Déplacer les corps des méthodes à l'identique ; aucun renommage ni changement de logique.
4. Vérifier les appels, usings, compilation, puis comportement en jeu avant de poursuivre.
5. Ne pas toucher aux fichiers de sauvegarde, aux signatures publiques, à NativeUI, SHVDN2 ni aux unités.

## Lot 1 — effectué sur la branche de travail
- Déplacement intégral des méthodes areaInfoUI, ZeeInfoUI, PlayerUI et GangUI dans LSlife/UI/DebugHud.cs.
- LSL devient une classe partielle ; les quatre méthodes restent privées, sur la même classe, avec les mêmes données et appels.
- Aucun changement prévu sur les menus, l'inventaire, les sauvegardes ou les coordonnées.
- Compilation Windows et tests en jeu : **non effectués** ; validation nécessaire avant tout transfert sur main.

## Lots suivants — non réalisés
- Identifier et isoler les autres fonctions d'affichage de LSL, si leur dépendance le permet.
- Cartographier précisément les méthodes de LsFunctions et LsMenus et les répartir par responsabilités.
- Réorganiser les autres composants seulement si un bénéfice clair justifie le déplacement.
- Compiler et tester chaque lot avant intégration.
