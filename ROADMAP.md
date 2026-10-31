## Audit comparatif SHVDN2 / SHVDN3 - 31/10/2026

Reference : `feature/custom-zee-menu` (SHVDN2), cible : `feature/shvdn3-migration`.

- [x] Comparer les arbres : 134 fichiers dans chaque branche, 54 fichiers C# modifies dans la cible.
- [x] `LsLPeds.cs` strictement identique : rafraichissement PNJ toutes les 7 secondes.
- [x] `Area.CanSpawnDealer()` identique : blocage possible via DealerKilled/DemandBuff.
- [x] `Tick.cs` : ancien `World.GetZoneNameLabel` remplace par `LsFunctions.GetZoneLabel` pour les transitions, la selection de PNJ et les planques. Verifier l'equivalence exacte de ces identifiants.
- [x] `Tick.cs` : condition de spawn deja modifiee (`else if` -> `if`) sans resoudre le symptome en jeu ; ne pas la considerer comme cause confirmee.
- [ ] Instrumenter et comparer en jeu playerArea, newArea, areaIndex, zoneData.ContainsKey, GangPresance, CanSpawnDealer, nombre de PNJ et de candidats.
- [ ] Comparer la logique de DEBUG (PedDebugger, HUD, touches, affichage), puis les 54 fichiers C# modifies pour identifier les regressions fonctionnelles.
- [ ] Eviter les modifications de gameplay non etayees ; compiler et valider chaque correctif.

## Changement de quartier / spawn dealers - 31/10/2026

- [x] Analyse de Tick.cs : recherche de dealer dans `else if` de changement de zone, empechant le spawn lors du meme controle.
- [x] Separer le spawn de la transition de zone, avec protection de la liste de PNJ.
- [x] Verifier `LsLPeds.Ontick()` : actualise les PNJ proches toutes les 7 secondes et retire ceux trop eloignes.
- [ ] Compiler et tester en jeu les transitions successives de quartiers et la regeneration des dealers.
- [ ] Si le probleme persiste, instrumenter `CanSpawnDealer()`, `GangPresance`, nombre de PNJ eligibles et `areaIndex` dans le HUD DEBUG.

## DEBUG HUD - 31/10/2026

- [x] Overlay AreaDebug dissocie de showInventory.
- [x] Quatre panneaux disposes en deux colonnes, deplacement borne aux nouvelles dimensions.
- [x] Ajout des informations employes, dealer de quartier, temps de jeu et protections sur les compteurs de PNJ.
- [x] Sous-titre de confirmation pour le basculement du diagnostic.
- [ ] Compiler et tester en jeu; ajuster si necessaire l'affichage selon la resolution.
- [ ] Revoir ensuite les etiquettes 3D de PedDebugger.

## Crash employé/dealer — 31/10/2026 09:25:35

- [x] Log : `NullReferenceException` dans `PlayerDealer.Deal()` via `PlayerDealer.OnSlowTick()` et `PlayerDealerHandler.OnSlowTick()` ; distinct du DEBUG.
- [x] Correction du blip client supprimé sans vérification (`AttachedBlip.Delete()`), alors que le blip n'est créé que lorsque `LSL.DEBUG` est actif. Suppression protégée par `!= null && Exists()` et effectuée avant le retrait du PNJ.
- [x] Vérification de `LSL.lPeds` et de sa liste avant la recherche d'un client.
- [ ] Recompiler et tester la vente automatique des employés sur une session prolongée ; cause exacte non confirmée faute de numéro de ligne.
- [ ] Revoir séparément le HUD DEBUG et ses dépendances à `showInventory`.

## Compilation 31/10/2026 — log 02:49:48

- [x] Cinq erreurs C# à `Tick.cs:1052` : marqueur de diagnostic inséré entre `if` et `else if`. Marqueur déplacé dans le bloc `else if`.
- [x] Instrumentation renforcée dans `Tick.cs` : points de contrôle autour de scans d'armes, zones, nettoyage, dealer de quartier et début de tick. Capture de toutes les exceptions `Exception` (plus seulement `NullReferenceException`) dans `scripts/LSLife/LSLifeRuntimeErrors.log`, puis propagation à SHVDN.
- [ ] Compilation locale à relancer : non vérifiable ici. Ensuite reproduire crash automatique après `Reload()` et transmettre le nouveau journal.

## Diagnostic du crash 03:44:22 — après `wepDealer`

- [x] Dernier marqueur du log : `if (wepDealer != null)` ; localisation **non définitive** car de nombreuses instructions suivent avant le prochain marqueur.
- [x] `Tick.cs` : garde de blip sur nettoyage périodique (`||` → `&&`), contrôle de validité du ped `aDealer` avant accès à sa position, contrôle de la présence de planques avant `Houses[0]`.
- [x] Sept nouveaux points de contrôle autour de `ShotHandler`, entretien 10 secondes, actualisation des zones et création des dealers de quartier.
- [ ] Recompiler et reproduire ; exploiter le nouveau marqueur du log pour isoler l'instruction fautive. Pas encore de validation en jeu.

## Diagnostic 31/10/2026 03:36–03:40

- [x] Log reçu : deux `NullReferenceException` identiques, stack `OnTickCore → onTick`, sans PDB/numéro de ligne. Le précédent journal ne suffit pas à isoler la cause.
- [x] Ajout de 19 marqueurs d'étapes dans `Tick.cs`, journalisés à l'exception via `LSLifeRuntimeErrors.log` ; localisation ciblée du prochain crash sans deviner le correctif.
- [ ] Recompiler et reproduire ; lire `Derniere etape onTick`, corriger l'accès nul précisément.

## Crash quelques secondes après chargement — 31/10/2026 03:32:56

- [ ] `NullReferenceException` dans `LSL.onTick` après chargement, sans numéro de ligne dans SHVDN. Cause non encore identifiée.
- [x] Instrumentation `Tick.cs` : wrapper `onTick`/`OnTickCore`, enregistrement de l'exception et stack trace dans `scripts/LSLife/LSLifeRuntimeErrors.log`, puis relance de l'exception pour préserver le signal SHVDN. Aucun gameplay modifié volontairement.
- [ ] Compiler en Debug avec symboles PDB si possible, déployer DLL et PDB ensemble, reproduire et analyser `LSLifeRuntimeErrors.log`.
- [ ] Corriger la cause identifiée, puis valider le chargement et les menus. `main` inchangée.

## Tests en jeu 31/10/2026 — correctifs crashes SHVDN3

- [x] Menu Zee principal, commandes de drogue (composition, paiement, réception/stock), options HUD : test utilisateur positif avant fin de livraison.
- [x] Crash `InvalidCastException` (`OutputArgument` → `InputArgument`) sur **Acheter des armes** et **Demander à être déposé** : 11 conversions invalides retirées dans `SpawnLogic.cs` (6), `WorldFunctions.cs` (3) et `Tick.cs` (2). Recompilation requise.
- [x] Crash `AreaDealer.Deal()` : protections contre liste de peds absente et blip client absent ; correction également du test de blip `||` susceptible de déréférencer null dans `AreaDealer.CheckState()`. Recompiler et retester le sous-menu Dealer.
- [ ] **Crash après départ des livreurs** : `NullReferenceException` dans `LSL.onTick` sans ligne source ; cause précise non établie. Reproduire avec DLL PDB ou journal SHVDN3 plus détaillé, vérifier cycle de vie `ZeesDealers` après livraison.
- [ ] Compiler les correctifs, puis retester les 3 actions Zee et la livraison. `main` inchangée.

## Build 31/10/2026 01:48:28 — compilation SHVDN3 propre

- [x] **Build Release net48 réussi : 0 erreur / 0 avertissement** (`LSlife.dll` générée), confirmé par le journal utilisateur.
- [x] Migration des API obsolètes de contrôles, terrain et pickups : compilation validée.
- [ ] **Validation en jeu GTA V Legacy** : démarrage du script, menu Zee, menus employés, planques/coffres, interactions dealer/clients, commandes clavier/manette, pickups, spawns/hauteurs, HUD, téléphone natif et iFruitAddon2.
- [ ] Examiner les logs SHVDN3 en cas de crash ou anomalie ; corriger sur `feature/shvdn3-migration`.
- [ ] Ne fusionner vers `main` qu'après validation fonctionnelle explicite.

## Build 31/10/2026 01:46:48 — LOT final des 8 avertissements

- [x] Build utilisateur : **0 erreur / 8 avertissements**. Migration des 27 contrôles compilée.
- [x] **3 pickups** migrés vers `GTA.Pickup.Create` : `DroppedBag`, `StashHouse`, `Tick`. Types, positions, rotations, modèles personnalisés et montants (0 / 100 / 100) conservés ; signatures vérifiées dans le code source SHVDN3.
- [x] **5 requêtes de hauteur** migrées vers `World.GetGroundHeight(position, out float height)` dans `WorldFunctions` (4) et `CustomerLogic` (1), signature vérifiée dans SHVDN3. En cas d'absence de collision, conservation de la hauteur initiale dans les helpers de positionnement ; `CustomerLogic` utilise 0f comme ancien comportement de repli.
- [ ] **Compiler ce lot**, objectif 0 erreur / 0 avertissement ; ne pas annoncer le résultat avant retour utilisateur.
- [ ] Tests en jeu : pickups, altitude des spawns, client, commandes, coffres, interactions. `main` inchangée.

## Build 31/10/2026 01:45:11 — GROS LOT commandes GTA (8 fichiers)

- [x] Build utilisateur : **0 erreur / 35 avertissements** ; les 2 CS0162 du HUD sont éliminés.
- [x] **27 appels obsolètes** `Game.IsControlJustPressed/Released((Control)ID)` migrés dans `HirePed`, `DrugDealer`, `AreaDealer`, `StashHouse`, `Shot`, `StashVehicle`, `PlayerDealer`, `Tick`.
- [x] Utilisation directe des natives `IS_DISABLED_CONTROL_JUST_PRESSED/RELEASED` avec groupe `2` et ID numérique inchangé : évite les incompatibilités de types entre `GTA.Control` et `GTA.Input.ControlAction` ; les événements pressed/released restent distincts.
- [ ] **Recompiler ce lot de 27 modifications** et tester en jeu les interactions (menus, recrutement, coffre, tirs, dealer). Les signatures natives sont utilisées sans vérification locale de compilation.
- [ ] 5 avertissements `GetGroundHeight` et 3 `CreatePickup` à traiter après validation des signatures SHVDN3 ; tests gameplay, puis fusion. `main` inchangée.

## Build 31/10/2026 01:43:40

- [x] Build utilisateur **0 erreur / 37 avertissements**.
- [x] `Hud.cs` : correction complémentaire des deux branches `CS0162` : les drapeaux de visibilité ne sont plus `readonly` (la compilation peut toujours évaluer un `static readonly false` comme constant dans ce contexte). Valeurs par défaut conservées à `false`.
- [ ] Recompiler ce correctif ; avertissements contrôles, terrain et pickups à migrer avec signatures SHVDN3 vérifiées.
- [ ] Tests en jeu avant fusion ; `main` inchangée.

## Build 31/10/2026 01:41:57

- [x] Build utilisateur **0 erreur / 37 avertissements**.
- [x] `Hud.cs` : options `ShowReputation` et `ShowDealerCapacity` passées de constantes à champs statiques readonly ; conserve le comportement désactivé sans branches inaccessibles.
- [ ] Recompiler ce changement. Restent les API de contrôle, terrain et pickups ; éviter les substitutions aveugles de signatures SHVDN3.
- [ ] Tester les menus, interactions, police, commandes et spawn en jeu avant fusion. `main` inchangée.

## Build 31/10/2026 01:41:07 — correctif de compilation

- [x] Build utilisateur : **2 erreurs CS0103, 41 avertissements** ; `DrugOrder.cs` utilisait encore `payed` après suppression du champ.
- [x] `DrugOrder.cs` : retrait des deux affectations restantes à `payed` (59 et 248), variable jamais lue.
- [x] `Prison.cs` : suppression de trois champs privés jamais lus (`Gate.DEBUG`, `Gate.Locked`, `BailAmount`) ; le `Prison.DEBUG` réellement utilisé est conservé.
- [x] `Threat.cs` : initialisation explicite de `Responded` à `false`.
- [ ] Relancer le build ; correctif non encore compilé.
- [ ] Restent API contrôles, terrain, pickups, avertissements HUD ; tests en jeu. Branche `main` inchangée.

## Build 31/10/2026 01:39:15 — nettoyage transversal

- [x] Build utilisateur : **0 erreur / 46 avertissements** (52 auparavant).
- [x] `InputEvents.cs` : suppression de la condition constante `if (1 == 0)`.
- [x] `ItemSlot.cs` : suppression du champ privé inutilisé `SlotNumber`.
- [x] `DefaultPed.cs` : initialisation explicite de `working`.
- [x] `DrugOrder.cs` : suppression de `payed`, assigné mais jamais lu.
- [x] `ShotHandler.cs` : suppression de `routeSet`, assigné mais jamais lu.
- [ ] `Threat.Responded` : écriture GitHub bloquée, non modifié.
- [ ] Reste : 27 avertissements de contrôles, 5 de terrain, 3 de pickups, code inaccessible HUD, état inutilisé de Prison ; migration à effectuer en conservant les comportements.
- [ ] Compiler le lot, puis tests GTA. `main` inchangée.

## Build 31/10/2026 01:37:08

- [x] Build utilisateur confirmé : **0 erreur / 52 avertissements**, DLL Release net48 générée.
- [x] Lot de nettoyage de champs privés inutilisés : `PedTeam`, `DefaultPed`, `DroppedBag` ; initialisation explicite de `PedDebugger.dTime`, `Pig.DEBUG`, `AreaDealer.DEBUG`.
- [ ] `ItemSlot.SlotNumber` : écriture GitHub bloquée, reste à traiter.
- [ ] Reste à migrer les contrôles (mapping précis Control/ControlAction), les pickups et les 5 accès terrain ; examiner les avertissements de code inaccessible et les champs réellement inutilisés.
- [ ] Recompiler après le lot ; tester en jeu avant validation de migration SHVDN3. Branche `main` inchangée.

## Build 31/10/2026 01:35:49 — migration transversale

- [x] Build utilisateur : **16 erreurs CS7036, 57 avertissements**.
- [x] Correction des **16 erreurs** : `Wanted.SetWantedLevel(level, false)` et `Wanted.ApplyWantedLevelChangeNow(false)` dans `Pig.cs` et `DealerFunctions.cs`.
- [x] `AreaDealer.cs` : deux `DriveTo` migrés vers `VehicleDrivingFlags` avec l'ordre des paramètres SHVDN3.
- [x] `DriveBy.cs` : retrait de `DrivingStyle` obsolète, conservation des valeurs de flags d'origine.
- [x] `DroppedBag.cs` : explicitation de l'argument `value` du pickup ; surcharge exacte à contrôler au build.
- [ ] Recompiler ce lot ; aucun build local effectué par l'assistant.
- [ ] Restent contrôles (mapping ControlAction), terrain, pickups, code inaccessible et champs inutilisés. Tests en jeu nécessaires. `main` inchangée.

## Build 30/10/2026 16:05:55 — migration API

- [x] Build confirmé : **0 erreur, 77 avertissements** (contre 85 auparavant).
- [x] Migration des deux appels de navigation obsolètes `TaskInvoker.GoTo(Vector3)` vers `FollowNavMeshTo` dans `LSlife.Dealing/DrugDealer.cs`.
- [ ] `World.GetGroundHeight` : migration différée tant que la signature et l'enum de mode ne sont pas vérifiés ; ancienne implémentation restaurée pour éviter une régression de compilation.
- [ ] Autres avertissements : contrôles, police, argent, conduite, pickups, code inaccessible, champs inutilisés ; tests en jeu toujours requis.
- [ ] Relancer la compilation pour vérifier le dernier lot. Branche `main` inchangée.

## Build 30/10/2026 — 16:05:06 : compilation verte

- [x] **0 erreur, 85 avertissements** : génération de `bin/Release/net48/LSlife.dll` réussie après correction de l'horloge dans AreaLogic.
- [x] Nouveau lot : migration de cinq lectures `player.WantedLevel` vers `player.Wanted.WantedLevel` dans `LSlife.Core/Tick.cs`.
- [x] Nouveau lot : migration des états d'ignorance police/population et du contrôle joueur vers `Game.Player.Wanted.SetPoliceIgnorePlayer`, `SetEveryoneIgnorePlayer` et `Game.Player.SetControlState` dans `LSlife.World/Prison.cs`.
- [ ] Recompiler pour vérifier les nouvelles signatures ; 85 avertissements concernent encore les contrôles, Wanted, argent, conduite, hauteur du sol, pickups, Game.IsLoading et champs inutilisés.
- [ ] Tester en jeu la prison, la police, les contrôles et les interactions ; aucun test GTA exécuté par l'assistant. Branche `main` inchangée.

## Build du 30/10/2026 à 16:01:30 — correction transversale

- [x] Build utilisateur : **28 erreurs CS7036, 64 avertissements**.
- [x] Les **27 appels** `GTA.Input.Controls.IsDisabledControlJustPressed/Released` introduits au lot précédent exigeaient `ControlType` et `ControlAction` : retour aux appels `Game.IsControlJustPressed/Released` compatibles et précédemment fonctionnels, dans 8 fichiers, pour préserver la sémantique des commandes. Une véritable migration de cette API exige une correspondance explicite des groupes et actions.
- [x] Dernier `Notification.PostTicker` incomplet corrigé dans `ShotHandler.cs`.
- [x] Migrations complémentaires : `AreaDealer.MoneyCarried` (uint), `GameClock.Today` dans AreaLogic, `TaskSequence.AddTask.Wander/FollowNavMeshTo` dans PlayerLogic.
- [ ] Recompiler et vérifier les signatures des séquences de tâches ; aucun build local effectué par l'assistant.
- [ ] Reste : Wanted, police, conduite, pickups, hauteur du sol, finances, nettoyage warnings et tests fonctionnels en jeu. Ne pas toucher `main`.

## SHVDN3 — gros lot après build 30/10/2026 15:58:20

- [x] Build utilisateur analysé : **5 erreurs CS7036 et 94 avertissements**.
- [x] Correction des **5 appels `Notification.PostTicker`** manquants dans `LSlife.World/DriveBy.cs`, `LSlife.Shots/ShotHandler.cs`, `LSlife.World/AreaLogic.cs` ; les trois fichiers précédemment bloqués ont finalement été poussés.
- [x] Migration de **27 appels de contrôle** (`Game.IsControlJustPressed/Released` vers `GTA.Input.Controls.IsDisabledControlJustPressed/Released`) dans 8 fichiers : HirePed, DrugDealer, StashHouse, StashVehicle, Shot, PlayerDealer, Tick, AreaDealer.
- [x] Modernisation complémentaire de navigation et géométrie dans Tick, Customer, Pig.
- [ ] **Build local requis** : lot non compilé par l'assistant ; vérifier signatures des API `GTA.Input.Controls` et `TaskInvoker` et corriger tout écart en gros lots.
- [ ] Continuer les avertissements SHVDN3 : police/Wanted, conduite/VehicleDrivingFlags, argent/MoneyCarried, pickups, hauteur du sol, horloge, Game.IsLoading ; vérifier la sémantique avant remplacement.
- [ ] Tests en jeu indispensables : interactions, menus, iFruit, contrôles, PNJ, sauvegardes, HUD et véhicules. `main` inchangée.

## Build 30/10/2026 — 15:55:26 (lot de correction)

- [x] Diagnostic : échec du build après migration API ; **91 appels** `Notification.PostTicker` incomplets sur 20 fichiers et une erreur `FollowNavMeshTo(Entity)` dans `LSlife.Core/Tick.cs`.
- [x] Correction de **86 appels PostTicker** dans 17 fichiers et conversion de la cible de navigation en `player.Character.Position`.
- [ ] Trois fichiers restent à corriger : `LSlife.World/DriveBy.cs` (2), `LSlife.Shots/ShotHandler.cs` (2), `LSlife.World/AreaLogic.cs` (1) ; leurs écritures ont été bloquées par les contrôles de sécurité de l'outil.
- [ ] Recompiler après correction des trois fichiers restants ; ne pas annoncer un build vert avant validation locale.
- [ ] Continuer la migration des API obsolètes (contrôles, police, conduite, tâches, argent, pickups, terrain, horloge), puis tests fonctionnels en jeu.

## SHVDN3 — build du 30/10/2026 à 14:45:31 et lot transversal

- [x] Compilation locale fournie : **0 erreur, 233 avertissements** (216 CS0618, 17 autres diagnostics), contre 241 avertissements au précédent build vert.
- [x] Gros lot transversal poussé sur `feature/shvdn3-migration` : migration de `Notification.Show` vers `Notification.PostTicker`, créations `World.CreatePed/Vehicle/Blip/Prop` vers les fabriques des entités, `AlwaysKeepTask` vers `KeepTaskWhenMarkedAsNoLongerNeeded`, et tâches `GoTo`, `WanderAround`, `FightAgainst`, `Wait` vers les API SHVDN3 recommandées dans de nombreux modules (Core, Dealing, Inventory, World, Shots, Services, Player, OldCustomers).
- [ ] **Recompiler** ce lot : il n'a pas été validé localement et peut révéler des signatures incompatibles. Corriger en gros lots, sans mini-correctifs isolés.
- [ ] Poursuivre la migration des contrôles `Game.IsControlJustPressed/Released`, niveaux de recherche `Player.WantedLevel`, conduite `DrivingStyle/DriveTo`, argent `Ped.Money`, hauteur du sol, pickups et horloge.
- [ ] Vérifier ensuite en jeu les menus Custom, le téléphone, la navigation des PNJ, les interactions, la sauvegarde et les véhicules ; 0 erreur de compilation n'est **pas** une validation fonctionnelle.
- [ ] Ne pas modifier `main` ni fusionner avant validation complète.

## SHVDN3 — build du 30/10/2026 à 14:44:26

- [x] Journal analysé : une erreur bloquante `CS0266` dans `LSlife.Dealing/Customer.cs:92` (`int` vers `uint` pour `Ped.MoneyCarried`).
- [x] Correction du montant avec borne minimale zéro et conversion explicite vers `uint` (`d10c0fd5`).
- [ ] Relancer `Build-Baseline.ps1` et confirmer le retour à zéro erreur.
- [ ] Après build vert, poursuivre la réduction des avertissements SHVDN3 par gros lots, puis les tests en jeu (menus Custom, téléphone, stocks, sauvegardes).
- [ ] Ne jamais modifier `main` durant la migration ; travailler exclusivement sur `feature/shvdn3-migration`.

## SHVDN3 — build du 30/10/2026 à 14:34:37

- [x] Analyse du journal : **10 erreurs distinctes**, dans `Tick.cs`, `PedDebugger.cs`, `AreaLogic.cs`, `StashVehicle.cs`.
- [x] Migration des derniers appels `UIText` identifiés vers `GTA.UI.TextElement` (`Tick`, `PedDebugger`, `AreaLogic`).
- [x] Remplacement `WeaponComponent.Hash` inexistant par `WeaponComponent.ComponentHash` (`Tick`, `StashVehicle`).
- [x] Remplacement du membre `Hash.IS_VEHICLE_NEON_LIGHT_ENABLED` absent par sa valeur native dans `StashVehicle`.
- [ ] Relancer la compilation et contrôler les nouveaux diagnostics, puis tester en jeu.
- [ ] Nettoyer les avertissements d'obsolescence progressivement après stabilisation.

## SHVDN3 — build du 30/10/2026 à 14:31:22

- [x] Journal complet analysé : **37 diagnostics d'erreur distincts** ; nombreuses alertes d'obsolescence sans échec de compilation.
- [x] Lot de corrections : `DrugDealer` (TaskInvoker), `PlayerLogic` (VehicleDrivingFlags), `WorldFunctions` (Blip.Delete), `AreaLogic` (heure/HUD), `Hud` (masquage natif), `CustomerLogic` (alpha natif), `Pig` (navigation), `AreaDealer` (ciblage), `StashVehicle` (néons et composants d'armes), `Tick` (commandes, composants, armes, waypoint), `PedDebugger` (position des os).
- [ ] Recompiler : les modifications du lot ne sont pas encore validées.
- [ ] Terminer migration `UIText` dans `PedDebugger`, `Tick`, `AreaLogic` et vérifier les API natives ciblage et waypoint ; corriger tout diagnostic nouveau.
- [ ] Tester en jeu après compilation réussie ; ne pas fusionner avant validation.

## SHVDN3 — build 30/10/2026 14:29:48

- [x] Le build s'arrête à l'analyse syntaxique : 2 erreurs `CS1525` / `CS1003` dans `DriveBy.cs:539`, aucune compilation sémantique effectuée.
- [x] Correction du cast `VehicleDrivingFlags` et de l'ordre des arguments `DriveTo` à la ligne 539 (`f899f9b5`).
- [x] Vérification visuelle des autres appels `DriveTo` / `CruiseWithVehicle` modifiés dans `DriveBy`, `Pig` et `Tick`.
- [ ] Relancer le build pour obtenir les éventuelles erreurs sémantiques restantes ; ne pas considérer 2 comme le total réel.

## SHVDN3 — build 30/10/2026 14:27

- [x] Analyse du journal : **68 erreurs distinctes** (hors doublons), contre 85 au build précédent.
- [x] `Tick.cs` : réparation des références au tueur introduites par migration précédente ; conversion des tâches de conduite.
- [x] `DisplayFunctions.cs`, `Types.cs`, `InputEvents.cs` : `Game.PlaySound` remplacé par `PLAY_SOUND_FRONTEND` natif.
- [x] `DriveBy.cs`, `Pig.cs` : mise à jour des arguments `VehicleDrivingFlags` pour les tâches de conduite.
- [x] `PlayerDealer.cs` : correction suppression blip.
- [ ] Recompiler et valider : aucun build réussi confirmé.
- [ ] Traiter `UIText`, `PedDebugger`, `AreaLogic`, `DrugDealer.Tasks`, `HudComponent`, `WorldFunctions`, autres erreurs restantes.

## SHVDN3 — build du 30/10/2026 à 14:13

- [x] 85 erreurs de compilation distinctes dans le journal (170 lignes dupliquées), contre 165 au précédent.
- [x] `Tick.cs` : migration des groupes de PNJ, zones, auteurs des décès et composants d'armes ; correction des appels de groupes de relations.
- [x] `PlayerDealer.cs` : migration suppression des blips et gel des positions.
- [x] `PersistenceFunctions.cs` : migration gel des positions.
- [ ] Recompiler pour vérifier les changements ; aucune compilation réussie confirmée.
- [ ] Corriger les incompatibilités restantes de `Tick.cs`, conduite, HUD/UIText, sons, `DrugDealer`, `PedDebugger` et `StashVehicle`.

## SHVDN3 — build 30/10/2026 14:05:38 : gros lot

- [x] Journal analysé : environ 165 erreurs uniques (fichier/position/code/message), avant corrections de ce lot.
- [x] `StashVehicle` : migration majeure des couleurs, jantes, plaques, teintes, livrées, kits, modifications et options vers `Vehicle.Mods` et types SHVDN3.
- [x] `Tick` / `InputEvents` : groupes de relations migrés vers `RelationshipGroup` ; récupération de zone corrigée.
- [x] `PedFunctions` / `Setup` : composants d'armes migrés vers `WeaponComponentHash`.
- [x] `OldCustomer` : recherche du responsable de la mort via native.
- [x] Autres fichiers : groupes de PNJ, blips et appels UI migrés dans DriveBy, CustomerLogic, PlayerLogic, WorldFunctions, UtilityFunctions, PoliceLogic, Types, DisplayFunctions et PedFunctions.
- [ ] Recompiler impérativement : aucune compilation réussie confirmée ; les autres incompatibilités de Tick, conduite, UI et sons restent à corriger.
- [ ] Préserver les branches stables et vérifier les comportements en jeu après build vert.

## Migration SHVDN3 — lot massif du 30/10/2026 (build 13:42)

- [x] Zones : `GetZoneLabel(Vector3)` via native `GET_NAME_OF_ZONE(x,y,z)` et migration des appels dans DrugDealer, AreaDealer, PlayerDealer, PlayerDealerHandler, CombatFunctions, Shot, Pig.
- [x] Portes des véhicules : migration des appels `OpenDoor`, `CloseDoor`, `IsDoorOpen` vers natives dans StashVehicle et DrugDealer.
- [x] Groupes de PNJ : `CurrentPedGroup` remplacé par `PedGroup` dans PlayerDealer, Setup, Shot, OldCustomer et OldCustomerHandler (API SHVDN3 vérifiée).
- [x] Composants d'armes : `WeaponComponentHash` et `Weapon.Components[hash].Active` dans WorkerWeaponTransfer, HouseStashTransfer, VehicleWeaponTransfer (API SHVDN3 vérifiée).
- [x] Plaques : retour à `Vehicle.NumberPlate` dans StashVehicle.
- [ ] Recompiler pour valider ces changements et traiter les erreurs restantes (Tick, StashVehicle, tâches, sons, armes, UI).
- [ ] Ne pas fusionner sans build et tests en jeu.

### Build 30/10/2026 13:35:41
- [x] Analyse : 243 diagnostics d'erreur uniques selon fichier/position/code/message (plusieurs sorties répétées).
- [x] Migration `Model.GetDimensions(out min, out max)` dans `DrugDealer.cs` (5 occurrences) et `StashVehicle.cs` (1 occurrence).
- [ ] Recompiler : les corrections ci-dessus n'ont pas encore été vérifiées par build.
- [ ] Priorité : portes de véhicule, `Tick.cs`, `PlayerDealer.cs`, groupes de peds et armes.

## SHVDN3 — build 30/10/2026 13:33

- [x] 244 erreurs de compilation distinctes, contre 268 au build précédent (-24).
- [x] `DriveBy.cs` : suppression de blip migrée vers `AttachedBlip.Delete()`.
- [ ] Principaux blocages : `StashVehicle.cs` (57), `Tick.cs` (51), `PlayerDealer.cs` (27), `DrugDealer.cs` (20).
- [ ] Adapter portes/coffres, dimensions de modèles, groupes de peds, composants d'armes et fonctions de zone selon les API SHVDN3.
- [ ] Recompiler et vérifier les erreurs restantes ; aucun build réussi confirmé.

## SHVDN3 — build 30/10/2026 13:29

- [x] 268 erreurs de compilation distinctes (536 lignes dupliquées), contre 300 au build précédent.
- [x] Migration des appels UI dans Prison, StashHouseHandler, PlayerDealerHandler, DealerFunctions, ShotHandler, PersistenceFunctions.
- [x] Suppression des blips ennemis adaptée dans EnemyHandler.
- [ ] Priorité : StashVehicle (57 erreurs distinctes), Tick (51), PlayerDealer (27), DrugDealer (20).
- [ ] Nouveau build requis après ce lot ; aucune compilation réussie confirmée.

## SHVDN3 — journal du 30/10/2026 13:26

- [x] Journal analysé : 300 erreurs de compilation distinctes (600 occurrences répétées).
- [x] Lot de correctifs UI dans InputEvents, AreaDealer, Pig, WorkerWeaponTransfer et AreaLogic.
- [x] Suppression des appels `Blip.Remove()` dans DroppedBag, StashHouse, InputEvents et DrugDealer.
- [ ] Corriger les erreurs concentrées dans StashVehicle, Tick, PlayerDealer et DrugDealer, notamment portes, groupes de PNJ, zones et armes.
- [ ] Recompiler et vérifier la réduction réelle avant fusion ; aucun build vert confirmé.

## Migration SHVDN3 — gros lots du 30 octobre 2026

- [x] Migration groupée des contrôles, sous-titres, blips et accès raycast dans 14 fichiers, sur 3 commits (`8bfbd2e0`, `09aa48d2`, `b00a28c8`).
- [ ] Recompiler pour mesurer la baisse réelle des erreurs.
- [ ] Traiter en priorité les véhicules/portes, groupes de PNJ, zones, armes et autres API cassées.
- [ ] Valider la compilation et les tests en jeu avant toute fusion vers la branche stable.

## Migration SHVDN3 — build du 30/10/2026 à 13:12

- [x] Build analysé : **475 erreurs**, **123 avertissements** (les erreurs sont répétées dans le journal).
- [x] Migration ciblée des sous-titres et `CurrentBlip` dans `PoliceLogic.cs`, `ShotHandler.cs` et `Shot.cs`.
- [ ] Corriger les API restantes : `Blip.Remove`, UI, groupes de PNJ, armes, véhicules, raycasts, zones et contrôles.
- [ ] Recompiler après les changements et valider en jeu ; aucun build réussi annoncé.

## Migration SHVDN3 — lot du 29 octobre 2026

- [x] Adaptation des groupes de relations dans `LSL.cs`.
- [x] Adaptation des contrôles et sous-titres dans `HirePed.cs`.
- [x] Correction du sous-titre dans `CustomHudOptionsMenu.cs`.
- [x] Migration des accès aux blips et de `FreezePosition` dans `AreaDealer.cs`.
- [x] Migration des accès aux blips dans `InputEvents.cs`.
- [ ] Relancer `Build-Baseline.ps1` et corriger les erreurs restantes par modules.
- [ ] Vérifier les comportements en jeu avant fusion ; compilation non encore validée.

## Build 29/10/2026 15:26 — première barrière de compilation

- [x] Diagnostic : 3 erreurs de types manquants (`PedHash`, `VehicleHash`, `UIText`), aucune autre erreur encore visible car compilation interrompue tôt.
- [x] `Enums.cs` : import de l'espace de noms `GTA` pour les modèles de PNJ et véhicules.
- [x] `LSL.cs` : type de texte migré vers `GTA.UI.TextElement`.
- [ ] Relancer la compilation pour révéler les incompatibilités suivantes ; aucun succès de build présumé.

## Migration SHVDN3 — reprise isolée (29 octobre 2026)

- [x] Branche dédiée `feature/shvdn3-migration` créée depuis le snapshot SHVDN2 restauré.
- [x] Référence du projet basculée sur `ScriptHookVDotNet3.dll`.
- [x] Premier lot : migration ciblée de l'API des tâches de conduite et des propriétés d'entités.
- [ ] Compiler avec la nightly `v3.7.0-nightly.195` et fournir le journal complet ; aucun succès de build n'est présumé.
- [ ] Traiter les incompatibilités par modules (armes, véhicules, UI, monde, tâches).
- [ ] Tester en jeu menus, téléphone, sauvegardes et gameplay avant fusion.
- [ ] Conserver `feature/custom-zee-menu`, `main` et `backup/pre-shvdn3-2026-10-29` inchangées.

## Cible SHVDN3 nightly (28 octobre 2026)

- [x] Source officielle choisie : `scripthookvdotnet/scripthookvdotnet-nightly`.
- [x] Derniere release verifiee au 28/10/2026 : `v3.7.0-nightly.195`, publiee le 24/10/2026 ; archive `ScriptHookVDotNet-v3.7.0-nightly.195.zip`.
- [ ] Installer / confirmer cette release cote GTA V Legacy et identifier les assemblies v3 pour la compilation.
- [ ] Auditer la compatibilite d'iFruitAddon2 avec SHVDN3 avant de remplacer la reference SHVDN2.
- [ ] Migration du code par lots, avec builds regulieres, sans modifier `main`.

## Validation en jeu NativeUI et lancement migration SHVDN3 — 28 octobre 2026

- [x] Migration NativeUI vers CustomMenu acceptee apres demarrage du mod et premier essai en jeu sans anomalie constatee ; tests approfondis a poursuivre en jouant.
- [x] Compilation sans NativeUI confirmee (0 erreur, 3369 avertissements SHVDN2).
- [ ] Confirmer l'installation et la version de ScriptHookVDotNet3.dll dans GTA V Legacy.
- [ ] Auditer la compatibilite de `iFruitAddon2.dll` avec SHVDN3 avant toute substitution de reference.
- [ ] Migrer les API SHVDN2 vers SHVDN3 par lots, sans toucher a `main` et sans casser les menus Custom ou le telephone natif.

## Jalon confirme — build sans NativeUI (28 octobre 2026, 16:39)

- [x] Compilation locale confirmee par `build-baseline(20261028-163955).log` : **0 erreur**, **3369 avertissements**.
- [x] Reference `NativeUI.dll` retiree du projet ; compilation sans NativeUI validee.
- [ ] Validation fonctionnelle en jeu : menus Custom (Zee, clients, employes, coffres, planques), telephone iFruit natif, commandes, sauvegarde.
- [ ] Migration SHVDN2 vers SHVDN3 : preparer un audit des changements d'API et des dependances (dont iFruitAddon2), puis migrer par lots coherents.
- [ ] Conserver les avertissements SHVDN2 comme reference de depart ; ne pas les masquer pour simuler une migration.
- [ ] Systeme d'alliances entre gangs : apres les migrations, comme convenu.

## Correctifs de compilation NativeUI — 28 octobre 2026

- [x] `Shot.cs` : demandes de livraison via `CustomCustomerMenu` et garde de visibilite Custom.
- [x] `PedFunctions.cs` : retrait de l'ecriture dans le sous-titre de `WorkerMainMenu`.
- [x] `DealerLogic.cs` : retrait des mises a jour des anciens curseurs de stock.
- [x] `DealerRecruitmentActions.cs` : retrait de la mise a jour de l'ancien curseur d'offre.
- [x] `CustomZeeMenu.cs` : ajout de `Close()` pour les appels existants.
- [ ] Compilation a confirmer apres ces corrections ; continuer a eliminer toute reference historique revelee par le build.

## Correctif compilation sans NativeUI — 28 octobre 2026

- [x] Supprimer les huit directives `using NativeUI;` restantes dans `Types.cs`, `CustomerLogic.cs`, `DealerLogic.cs`, `InventoryLogic.cs`, `PlayerLogic.cs`, `AreaLogic.cs`, `PoliceLogic.cs`, `SpawnLogic.cs` (CS0246).
- [x] `DealerLogic.cs` : retirer `PlaceOrder.Text` et remplacer `LsMenuPool.CloseAllMenus()` par `CustomZeeMenu.Close()`.
- [ ] Relancer `Build-Baseline.ps1` pour confirmer la compilation sans NativeUI et traiter toute nouvelle erreur.
- [ ] SHVDN3 seulement apres compilation et validation de l'absence de dependance NativeUI.

## Jalon NativeUI — retrait de la DLL (28 octobre 2026)

- [x] `LegacyMenuCompatibility.cs` : tous les champs NativeUI supprimes ; seuls les catalogues d'armes, tarifs, multiplicateurs et `deliveriesEnabled` sont conserves comme donnees de gameplay.
- [x] `LSlife.csproj` : reference `<Reference Include="NativeUI">` supprimee.
- [x] `LSlife.UI/HeatBars/BarHandler.cs` : gestionnaire `TimerBarPool` NativeUI inutilise supprime.
- [x] `PlayerMenus.cs`, `StashMenus.cs`, `VehicleMenus.cs` : callbacks historiques retires au lot precedent.
- [ ] **Compilation a confirmer** apres retrait de la DLL ; corriger les references residuelles sans reintroduire NativeUI.
- [ ] Une fois compilation confirmee, verifier le comportement des menus, transferts, commandes et telephone natif avant SHVDN3.

## Migration NativeUI — nettoyage des anciens callbacks de stockage (28 octobre 2026)

- [x] `PlayerMenus.cs` : ancien gestionnaire de retrait planque supprimé ; `HouseStashTransfer` et `CustomStashMenu` assurent le parcours actif.
- [x] `StashMenus.cs` : anciens sliders, listes et gestionnaire de dépôt NativeUI supprimés.
- [x] `VehicleMenus.cs` : anciens gestionnaires de dépôt/retrait coffre NativeUI supprimés ; transferts `VehicleStashTransfer` et `VehicleWeaponTransfer` conservés.
- [x] `CustomWeaponDealerMenu.cs` : correction du sélecteur d'armes qui dépendait encore de `LSL.melee` et `LSL.pistol` jamais initialisés.
- [ ] Supprimer `LegacyMenuCompatibility.cs` après extraction des listes métier et audit complet des références.
- [ ] Retirer `BarHandler.cs` et la référence NativeUI du `.csproj` après vérification des consommateurs.
- [ ] Compilation de ce lot non vérifiée.

## Suppression NativeUI — lot majeur (28 octobre 2026)

- [x] `Setup.cs` : appel à `InitializeLegacyMenus()` déjà retiré.
- [x] `LegacyMenuCompatibility.cs` : suppression intégrale de `InitializeLegacyMenus()` (plus de 20 000 caractères de construction de menus et branchements d'événements inutilisés).
- [x] `DealerMenus.cs`, `DealingMenus.cs`, `WorkerMenus.cs` : suppression des callbacks NativeUI ; conservation des points d'entrée métier (`CallWeaponDealer`, `CallLift`, `PayZeeDebt`, `CancelDealerRecruitment`, `SubmitDealerOffer`, `StartDealing`, `PlaceDrugOrder`, `HandleCustomerTransaction`, `GiveWeaponToWorker`).
- [x] `MenuEvents.cs` : suppression des callbacks NativeUI ; `NotifyZeeRestock()` conservé.
- [ ] Éliminer les références restantes dans `PlayerMenus.cs`, `StashMenus.cs`, `VehicleMenus.cs` et les champs de compatibilité.
- [ ] Supprimer `NativeUI.dll` du `.csproj` seulement après nettoyage complet des types ; compilation de ce lot à confirmer.

## Migration NativeUI — boucle de rendu (28 octobre 2026)

- [x] `CustomMenuShared.IsAnyGameplayMenuOpen()` : suppression du test `LsMenuPool.IsAnyMenuOpen()` ; les sept menus Custom sont seuls pris en compte.
- [x] `Tick.cs` : suppression de `LsMenuPool.ProcessMenus()` ; les menus Custom continuent leurs ticks individuels et le téléphone iFruit reste inchangé.
- [ ] `InitializeLegacyMenus()` dans `Setup.cs` et les callbacks NativeUI restent à retirer après audit de leurs références.
- [ ] Compilation non vérifiée dans cet environnement.

## Correctif CS1061 (28 octobre 2026)

- [x] Retirer `SetDrugDesc()` de `WorkerRevenueCollection.cs` et `WorkerSupplyTransfer.cs` : les deux appels étaient devenus invalides après suppression de la méthode d'affichage NativeUI.
- [x] Conserver les mutations de stock, la collecte des recettes et la sauvegarde.
- [ ] Confirmer la compilation ; audit global des références obligatoire avant les prochaines suppressions.

## Fin du lot employé — 28 octobre 2026

- [x] `UI/Menus/WorkerMenus.cs` : suppression des deux derniers appels historiques à `PlayerDealer.SetDrugDesc()` ; les transactions et la persistance restent inchangées.
- [ ] La compilation de ce lot n'a pas été exécutée localement.

## Lot NativeUI — accès au menu employé (28 octobre 2026)

- [x] `PlayerDealer.cs` ouvre directement `CustomWorkerMenu` sans initialiser les widgets NativeUI `wOrder`, `wArmour`, `wWeed`, `wCrack`, `wCocaine`, `wCollect`.
- [x] Suppression de `SetDealerOrders()`, `SetArmourButton()` et `SetDrugDesc()` dans `PlayerDealer` : ces routines ne construisaient que l'affichage historique.
- [ ] Les deux appels à `SetDrugDesc()` dans `UI/Menus/WorkerMenus.cs` (callbacks historiques) doivent être supprimés avant compilation ; ne pas considérer ce lot comme terminé tant que ces appels existent.

## Plan de sortie NativeUI (audit du 28 octobre 2026)

Objectif prioritaire : **supprimer la référence NativeUI du `.csproj` et compiler sans la DLL**. Les avertissements SHVDN2 sont hors périmètre : migration SHVDN3 uniquement après ce jalon.

- [ ] **Lot 1 — Couper les dépendances du gameplay** : rechercher et remplacer tous les accès encore actifs aux champs historiques (`LSL.*Menu`, `LsMenuPool`, curseurs et items) ; préserver les services métiers.
- [ ] **Lot 2 — Retirer les callbacks NativeUI** : `MenuEvents.cs`, `LsMenus.cs` et `UI/Menus/*.cs` ; vérifier que les actions sont déjà exposées aux menus Custom.
- [ ] **Lot 3 — Supprimer l'initialisation historique** : `InitializeLegacyMenus()` dans `Setup.cs`, `ProcessMenus()` dans `Tick.cs`, vérification du blocage d'interaction dans `CustomMenuShared.cs` ; déplacer les listes de données non visuelles nécessaires hors de `LegacyMenuCompatibility.cs`.
- [ ] **Lot 4 — Retirer la bibliothèque** : supprimer `LegacyMenuCompatibility.cs`, les derniers types `NativeUI` (notamment `HeatBars/BarHandler.cs` si inutilisé), puis la référence `<Reference Include="NativeUI">` dans `LSlife.csproj`.
- [ ] **Lot 5 si nécessaire — Stabilisation** : corriger les erreurs de compilation révélées par le retrait de la DLL, sans réintroduire de compatibilité NativeUI.

**Estimation : 4 lots techniques + éventuellement 1 lot de stabilisation.** Il ne s'agit pas d'une garantie de compilation ni d'une demande de test intermédiaire.

## Correctif compilation CS0117 — 28 octobre 2026

- [x] `DealingMenus.cs:61` : remplacement du dernier appel à `LsFunctions.UpdateOrderMenuDesc()` supprimée par l'affectation de la description via `LsFunctions.OrderMenuDesc()`.
- [ ] Recompilation à confirmer ; NativeUI reste présent dans les menus historiques.

## Suppression des routines de commande NativeUI — 28 octobre 2026

- [x] `DealerFunctions.cs` : suppression de `UpdateOrderMenuDesc()` (mise à jour des anciens champs `PlaceOrder`, `bushWeed`, `crack`, `cocaine`, `WeedBuy`, `OrderSummary*`, `LsMenuPool`).
- [x] Suppression de `UpdateDealerInventory()` : ne servait plus qu'à modifier les widgets de Zee et n'était plus appelée depuis le téléphone iFruit.
- [x] `OrderMenuDesc()` reste disponible comme résumé de facture sans dépendance NativeUI.
- [ ] Les appels historiques à `UpdateOrderMenuDesc` doivent être supprimés ou migrés dans les anciens gestionnaires avant retrait définitif de NativeUI ; compilation à confirmer.

## Correctif compilation CS1061 — 28 octobre 2026

- [x] Suppression des cinq appels résiduels à `StashHouseHandler.UpdateMenu()` signalés par `build-baseline(20261028-152244).log` dans `HouseStashTransfer.cs`, `StashHouse.cs`, `CustomStashMenu.cs`, `Menus/PlayerMenus.cs` et `Menus/StashMenus.cs`.
- [x] Les transferts conservent leurs appels de sauvegarde et la mise à jour d'inventaire ; le menu personnalisé utilise les stocks directement, sans rafraîchissement NativeUI.
- [ ] Compilation à confirmer après récupération des commits ; NativeUI n'est pas encore supprimé.

## Suppression du rafraîchissement des anciens stocks — 28 octobre 2026

- [x] `StashHouseHandler.UpdateMenu` supprimé : la méthode modifiait exclusivement les anciens composants NativeUI.
- [x] `StashHouse.cs` ne déclenche plus ce rafraîchissement avant d'ouvrir `CustomStashMenu`.
- [x] `StashVehicle.cs` : suppression des deux blocs d'initialisation des curseurs et listes NativeUI à l'ouverture du coffre.
- [x] `StashVehicle.UpdateCarStash()` conserve la reconstruction des listes d'armes joueur/véhicule mais ne modifie plus les widgets NativeUI.
- [ ] Supprimer les menus historiques et la DLL après nettoyage des autres consommateurs.
- [ ] Pas de demande de tests intermédiaires ; compilation non effectuée.

## Migration des interactions monde — 28 octobre 2026

- [x] `PlayerDealer.cs` ne dépend plus des objets `WorkerMainMenu`/`LsMenuPool` : ouverture, fermeture et visibilité passent par `CustomWorkerMenu` et `CustomMenuShared`.
- [x] Suppression du second appel `CustomWorkerMenu.TickOncePerFrame()` depuis chaque employé : le menu est déjà traité une fois par frame dans `LSL.OnTick`.
- [x] Ajout de `CustomWorkerMenu.Close()` pour les transitions de proximité et d'état de l'employé.
- [x] `StashHouse.cs` : visibilité et fermeture de la planque via `CustomStashMenu`, sans widgets historiques.
- [x] `DrugDealer.cs` et `AreaDealer.cs` : vérifications d'interaction basées sur `CustomMenuShared.IsAnyGameplayMenuOpen()`.
- [ ] Il reste notamment `StashHouseHandler.cs`, `StashVehicle.cs`, `DealerFunctions.cs`, `LegacyMenuCompatibility.cs`, les callbacks `Menus/*.cs`, `MenuEvents.cs` et la référence NativeUI dans `LSlife.csproj`.
- [ ] Aucun test demandé à l'utilisateur avant retrait effectif de NativeUI et validation de compilation.

## Migration coffre / gilets — 28 octobre 2026

- [x] Nouvelle logique `VehicleArmourTransfer.Transfer(bool deposit, int requested)` sans NativeUI : validation des quantités, capacité des gilets, équipement automatique, stockage et sauvegarde XML.
- [x] `MenuEvents.cs` : remplacement des deux longues routines de transfert par des appels au service ; seules les listes de l'ancien menu restent dans les callbacks.
- [x] Correction d'un défaut de retrait : un gilet équipé sur le personnage est désormais également décompté du coffre.
- [ ] Vérifier les transferts de gilets depuis le coffre, les limites 5/6, l'équipement automatique et le rechargement de la sauvegarde ; compilation non exécutée ici.
- [ ] NativeUI toujours requis : autres callbacks et initialisation historique non encore supprimés.

## Audit de suppression NativeUI — 28 octobre 2026

- [x] Contact iFruit Zee déjà indépendant du pool ; fermeture du téléphone protégée contre une instance absente.
- [x] Rafraîchissement de l'ancien menu de commande protégé contre un pool nul.
- [x] Vérifié : `LegacyMenuCompatibility.cs` construit toujours les menus NativeUI dans `InitializeLegacyMenus()` ; `Setup.cs` l'appelle ; `Tick.cs` traite le pool ; `BarHandler.cs` dépend des classes `TimerBarPool`/`BarTimerBar` ; le projet référence `NativeUI.dll`.
- [ ] **Blocage à la suppression** : retirer tous les callbacks NativeUI et les appels aux widgets historiques, puis supprimer `InitializeLegacyMenus`, le pool, les barres et la référence csproj dans un ensemble compilable.
- [ ] Compilation et tests GTA nécessaires ; ne pas annoncer la migration terminée avant validation.

## Correctif HUD réputation / migration — 28 octobre 2026

- [x] Diagnostic : l'ancienne jauge « Quartier » affichait `Area.Heat`, et non `Area.Reputation`.
- [x] Ajout d'une troisième jauge « Réputation » en bas à droite, indépendante des jauges de pression joueur et quartier.
- [x] Progression de réputation calculée jusqu'au niveau suivant selon `LsFunctions.CurrentRepLvl` ; 88,2 points donnent environ 14 % de la première tranche (625 points).
- [x] Protection des calculs de pression et de réputation contre les indices de quartier invalides.
- [ ] Compiler et vérifier l'affichage des trois jauges en jeu ; confirmer que la réputation du quartier courant évolue après une vente.
- [ ] NativeUI : `BarHandler`, `LegacyMenuCompatibility`, les callbacks historiques et la référence csproj restent à retirer après découplage complet.

## Migration téléphone et coffre — 28 octobre 2026

- [x] Le contact Zee d'iFruit ouvre directement `CustomZeeMenu` sans dépendre de l'existence de `LsMenuPool`, des anciens boutons de vente ni de l'ancien écran Zee.
- [x] Les huit vérifications d'ouverture du coffre dans `StashVehicle.cs` passent par `CustomMenuShared.IsAnyGameplayMenuOpen()` ; plus d'accès direct au pool historique dans ce fichier.
- [x] Import NativeUI inutilisé supprimé dans `InputEvents.cs`.
- [ ] Le code historique de rafraîchissement des menus et les autres callbacks NativeUI restent présents ; ne pas retirer la référence DLL avant migration et compilation complète.
- [ ] Tester appel Zee depuis iFruit (avec et sans activité de deal), fermeture du téléphone, coffre et planque, commandes et HUD ; compiler avec `Build-Baseline.ps1`.

## Migration des notifications Zee — 28 octobre 2026

- [x] Notification de réassort extraite de `OrderDrugs_OnMenuOpen` vers `LSL.NotifyZeeRestock()` sans dépendance aux types de menu.
- [x] Le menu de commande personnalisé Zee affiche maintenant cette notification à l'ouverture, comme l'ancien menu.
- [x] Retrait de l'import `NativeUI` inutilisé dans `LsMenus.cs`.
- [ ] Compiler et vérifier la notification de réassort Zee, ainsi que les commandes normales ; NativeUI reste indispensable aux autres anciens gestionnaires.

## Migration NativeUI — correctif du cycle de vie employé (28 octobre 2026)

- [x] `CustomWorkerMenu.TickOncePerFrame()` appelé depuis `LSL.OnTick` au même titre que les six autres menus personnalisés, indépendamment de `LsMenuPool`.
- [x] Maintien du traitement iFruit avant les menus et du pool historique pendant la transition.
- [ ] Vérifier en jeu la navigation, les transferts, l'équipement et le retour arrière des sous-menus employés ; compilation non exécutée ici.
- [ ] Supprimer NativeUI uniquement après migration de tous les anciens consommateurs et validation de la compilation.

## Découplage des actions métier — 28 octobre 2026

- [x] Le menu Zee conserve ses quatre options de vente/livraison en mémoire sans lire les cases à cocher NativeUI ; valeurs initiales inchangées (activées).
- [x] `LsMenus.HandleCustomerTransaction(bool)` délègue directement à `CustomerTransactionActions` ; l'activation/désactivation de l'ancien bouton n'est plus une condition de la transaction.
- [x] `LsMenus.PayZeeDebt()` ne modifie plus de composant NativeUI ; le rafraîchissement du libellé reste dans le callback historique `OnZeeMenuSelect`.
- [ ] NativeUI reste présent pour les anciens callbacks et menus ; pas de suppression de DLL dans ce lot.
- [ ] Compiler et tester : options de dealer Zee (fermer/réouvrir), vente/refus client, remboursement dette Zee, persistance des données et menus historiques.

## Lot ouverture des menus personnalisés — 28 octobre 2026

- [x] Zee, planques, coffres, clients, employés, recrutement et marchand d'armes : ouverture protégée par un contrôle partagé qui accepte un pool historique absent et refuse les menus concurrents.
- [x] Ajout du menu employé dans la détection globale des menus ouverts et dans le masquage du HUD DEBUG.
- [x] Suppression des accès directs à `LsMenuPool.IsAnyMenuOpen()` dans les sept interfaces personnalisées concernées.
- [ ] NativeUI reste nécessaire aux anciens menus ; la DLL et `LegacyMenuCompatibility.cs` ne sont pas encore supprimés.
- [ ] Vérifier compilation et GTA : ouverture/fermeture de chaque menu, transitions Zee/HUD, employés, recrutement, clients, coffre, planque et marchand d'armes.

## Lot Tick / interactions — 28 octobre 2026

- [x] Le traitement des six menus personnalisés dans `Tick.cs` n'est plus conditionné par l'existence de `LsMenuPool`.
- [x] Les vérifications d'ouverture de menu du Tick (HUD DEBUG, aide, marchand d'armes, client, interactions) passent par `CustomMenuShared.IsAnyGameplayMenuOpen()`, qui tient compte des six menus CustomMenu et du pool historique.
- [x] Import direct `using NativeUI;` retiré de `Tick.cs` ; aucune modification des sauvegardes XML ou de l'initialisation iFruit.
- [ ] `LsMenuPool.ProcessMenus()` et NativeUI.dll restent nécessaires pour les consommateurs historiques ; ne pas les supprimer avant leur migration.
- [ ] Compilation Windows et test GTA à effectuer : démarrage à froid/téléphone Zee, interactions client/marchand, HUD DEBUG, fermeture des menus et sauvegardes.

## Correctif Tick — 28 octobre 2026

- [x] Interaction marchand d'armes : vérification de `LsMenuPool` avant son accès et suppression du double test `IsAnyMenuOpen()`.
- [ ] La migration NativeUI de `Tick.cs` reste à effectuer ; ce correctif ne supprime aucune dépendance.
- [ ] Compilation Windows et test en jeu non effectués pour ce correctif.

# Migration NativeUI — état au 28 octobre 2026

**Branche de travail :** `feature/custom-zee-menu` ; **référence stable :** `main`. Ne pas supprimer `NativeUI.dll` avant compilation et vérification des derniers consommateurs.

## Dernier lot livré

- [x] `Setup.cs` : initialisation des anciens menus déplacée dans `LSlife.Core/LegacyMenuCompatibility.cs`, et imports directs NativeUI retirés.
- [x] `LSL.cs` : anciens champs NativeUI déplacés dans la classe partielle de compatibilité.
- [x] Correction des erreurs `UIText` / `GTA.UI` dans la couche de compatibilité (commit `6bdc9ef`).
- [x] `Setup.cs` : correction de migration des sauvegardes d'inventaire ; ajout indépendant des champs manquants `weedOz`, `crackOz`, `cocaineOz`, `armour`, et écriture unique après migration ; rechargement des quantités sans doublon de clés (commit `25d8f4e`).
- [ ] **Compilation Windows du dernier lot non confirmée** ; les erreurs éventuelles doivent être corrigées avant test GTA.
- [ ] **NativeUI.dll toujours indispensable** : l'extraction des anciens champs n'est pas une suppression de dépendance.

## Travaux restants, par priorité

1. [ ] Identifier et migrer tous les appels historiques encore actifs dans `Tick.cs` et les gestionnaires de planques, véhicules, dealer, employé, options et interactions.
2. [ ] Remplacer les accès aux `UIMenu*` et aux sliders/listes NativeUI par les états et services des menus personnalisés ; préserver les sauvegardes et les callbacks iFruit.
3. [ ] Supprimer les initialisations et événements devenus inutiles dans `LegacyMenuCompatibility.cs`, puis supprimer ce fichier quand il n'a plus de consommateurs.
4. [ ] Retirer `LsMenuPool.ProcessMenus()` / `IsAnyMenuOpen()` et centraliser l'ouverture, fermeture et navigation des menus personnalisés.
5. [ ] Retirer la référence NativeUI du `.csproj` et du déploiement ; compiler et tester à froid GTA V Legacy (téléphone Zee, argent, menus, employés, planques, véhicules, armes, sauvegardes).
6. [ ] Nettoyer les avertissements et les anciennes traces de décompilation après stabilisation, sans confondre avertissements SHVDN2 et erreurs bloquantes.

## Procédure de test

```powershell
cd E:\LSLife
git pull origin feature/custom-zee-menu
.\Build-Baseline.ps1
```

Ne lancer GTA qu'après une compilation réussie. Éviter de demander plusieurs redémarrages pour des changements purement internes.

---

## Lot isolé 01 — affichage Employé (26 octobre 2026)

- [x] Ajout de deux lignes **Instruction actuelle** et **Recette brute** dans le menu principal Employé uniquement (`CustomWorkerMenu.cs`).
- [x] Aucune modification de `Tick`, des contrôles, des transferts, de la sauvegarde ou d'iFruit.
- [ ] À valider en jeu : téléphone/contact Zee après démarrage complet, puis ouverture du menu Employé et vérification des deux nouvelles informations.
- [ ] En cas de panne du téléphone, revert **uniquement** le commit `20d33c2` et examiner l'effet de cette modification visuelle isolée.

## Analyse différentielle de la régression téléphone — 26 octobre 2026

État de référence : commit `c8e1a5b` (téléphone confirmé opérationnel après démarrage complet). Le retour des deux fichiers au contenu de cet état, dans `45e8859` et `c6df9ed`, a également été confirmé fonctionnel.

**Commits suspects examinés :**

- `bb70c20` — `CustomWorkerMenu.cs` : ajout d'un ID de session du dealer, resets du contrôleur horizontal, deux appels à `GTA.UI.ShowSubtitle`, lignes d'information supplémentaires.
- `087372f` — `CustomWorkerMenu.cs` : vérifications `((GTA.Entity)ped).Exists()`, validation d'index d'arme, affichage du total de transfert.
- `b6f89bb` — `CustomStashMenu.cs` : resets horizontaux et garde sur l'existence de la planque/coffre dans `TickOncePerFrame()`.

**Constats vérifiés dans les diffs :** ces commits ne changent ni `Setup.cs`, ni `Tick.cs`, ni `iFruitAddon2`. Les gardes supplémentaires se trouvent dans des chemins de menus qui commencent par `if (!Visible) return` ; le mécanisme exact de la panne au lancement n'est donc **pas identifié**. Le retour fonctionnel après revert établit la corrélation avec le lot, pas une ligne coupable.

**Protocole de reprise :**
1. Ne pas réintroduire le lot suspect tel quel.
2. Préparer d'abord des modifications purement visuelles et sans nouvelle API GTA dans un seul menu ; éviter les changements d'initialisation, de Tick global et de contrôles tant que l'origine n'est pas prouvée.
3. Compiler, fermer GTA entièrement, redémarrer et vérifier le téléphone/contact Zee **avant** d'ouvrir les menus.
4. En cas de régression, conserver le log `ScriptHookVDotNet.log` du démarrage et examiner les exceptions de chargement/initialisation.
5. Vérifier également la version de DLL réellement déployée et l'absence de copies en double dans `scripts`.

## Régression téléphone après lot Employé/Stockage — 26 octobre 2026

- [x] Retour de `CustomWorkerMenu.cs` et `CustomStashMenu.cs` à l'état `c8e1a5b` (dernier état avec téléphone confirmé fonctionnel par l'utilisateur).
- [x] Correction d'initialisation différée d'iFruit dans `Setup.cs` et `Tick.cs` conservée intacte.
- [ ] Vérifier **après fermeture complète et redémarrage** de GTA V ; ne pas se limiter à `Reload()`.
- [ ] Si la régression subsiste, analyser `ScriptHookVDotNet.log` du démarrage concerné avant toute nouvelle modification.
- [ ] Réintroduire les améliorations des menus progressivement après identification de la cause, sans modifier iFruitAddon2.

## Lot de stabilisation des menus personnalisés — 26 octobre 2026

- [x] Menu employé : verrouillage de la session sur l'identifiant du dealer sélectionné ; fermeture si le dealer change ou si son PNJ disparaît.
- [x] Menu employé : réinitialisation des contrôles à l'ouverture, au retour et entre sous-menus ; prévention des répétitions horizontales parasites.
- [x] Menu employé : affichage de l'identifiant, de l'instruction, de la recette brute, de la recette nette, du total à transférer et du multiplicateur.
- [x] Menu employé : confirmation visuelle de l'approvisionnement et de la collecte, vérification de l'existence du PNJ et de l'index d'arme avant transfert.
- [x] Menus planque/coffre : fermeture de l'interface si le stockage actif disparaît et remise à zéro de la répétition au changement de contexte.
- [x] iFruit : **aucune modification** des fichiers `Setup.cs`, `Tick.cs`, de l'initialisation différée du téléphone ou des contacts ; NativeUI.dll conservée.
- [ ] Compiler, puis tester en une session complète : téléphone à froid, contact Zee, HUD/argent, commandes, recrutement, vente client, employé (approvisionner, récolter, équiper, instructions), planque et coffre.
- [ ] Poursuivre la migration NativeUI uniquement après validation du lot ; ne pas retirer la DLL tant que les appels historiques existent.

## Priorité — iFruit au démarrage complet (26 octobre 2026)

- [x] Audit de la chaîne d'appel : `LSL()` → `Setup()` → création `CustomiFruit` / contact Zee ; `onTick()` → test `Game.IsLoading` → `iFruit.Update()`.
- [x] Problème d'ordre identifié : `Setup()` pouvait créer le téléphone avant la fin du chargement, tandis que `Reload()` intervient sur un jeu déjà chargé.
- [x] Initialisation déplacée vers `onTick()` après `Game.IsLoading` et disponibilité du personnage ; création unique et mise à jour protégée.
- [x] Aucun changement aux callbacks des contacts, à iFruitAddon2.dll, à l'argent, au HUD ou aux commandes.
- [ ] **Validation indispensable : fermer complètement GTA V, redémarrer, vérifier téléphone du protagoniste, contact Zee, argent HUD et paiement ; répéter un Reload() pour comparer.**
- [ ] Si le téléphone manque encore : récupérer `ScriptHookVDotNet.log` et identifier l'exception au démarrage (la cause exacte reste à confirmer en jeu).

## Lot groupé — recrutement et clients (26 octobre 2026)

- [x] Recrutement : acceptation, refus, paiement et annulation accessibles par méthodes métier sans paramètres NativeUI ; menu personnalisé branché sur ces méthodes.
- [x] Clients : vente et refus déplacés dans un traitement sans menu ni élément NativeUI ; le menu personnalisé n'ouvre plus artificiellement `customerMainMenu`.
- [x] Compatibilité : callbacks NativeUI historiques conservés comme adaptateurs.
- [x] Protection : aucun changement à iFruitAddon2, `Game.Player.Money`, au HUD financier ni aux commandes Zee validées.
- [ ] **À compiler et tester** : recrutement (annulation, refus, acceptation, débit), clients (vente, refus, véhicule, client éloigné), sauvegardes, téléphone et argent.
- [ ] **NativeUI.dll reste nécessaire** : équipement, planques, véhicules et initialisations historiques à migrer avant retrait final.

## Stabilisation Zee et iFruit — 26 octobre 2026

- [x] Retour aux traitements de commande Zee précédemment validés après régression téléphone/argent (restaurés dans `CustomZeeMenu.cs` et `DealingMenus.cs`).
- [x] Ajout du maintien gauche/droite dans les sous-menus Cannabis, Crack et Cocaïne, pour quantités et multiplicateurs (délai initial 320 ms, répétition 65 ms).
- [x] Réinitialisation de la répétition lors des changements de menus, sélection et retour, pour éviter les mouvements parasites.
- [x] Aucune modification de `iFruitAddon2`, du traitement iFruit dans `Tick`, de `Game.Player.Money`, ni des paiements des livreurs dans ce lot.
- [ ] Compilation et test en jeu : téléphone et contacts, argent HUD, commandes et livraisons, appui prolongé gauche/droite sur les trois produits, retour menus.
- [ ] Dépendances NativeUI restantes : à retirer uniquement après migration fonctionnelle complète et validation.

## Lot commandes Zee — 26 octobre 2026

- [x] Migration des trois commandes Cannabis / Crack / Cocaïne : quantités, pas de sélection, prix, réservation de stock et validation via traitement commun indépendant des sliders NativeUI.
- [x] Adaptateurs des anciens menus NativeUI conservés pour les autres appels historiques.
- [x] Suppression des références directes aux sliders NativeUI dans `CustomZeeMenu`.
- [ ] **À compiler et tester en jeu** : quantités et multiplicateurs des trois produits, prix et total, stock fournisseur, plusieurs ajouts, validation et livraison, sauvegarde et téléphone.
- [ ] Suppression de la DLL NativeUI : **non effectuée** ; autres systèmes historiques toujours dépendants.

## Nettoyage Zee — 26 octobre 2026

- [x] Sauvegarde `backup/pre-zee-order-decoupling-20261026`.
- [x] Suppression du pont de retour vers les anciens sous-menus NativeUI inutilisé dans `CustomZeeMenu`.
- [ ] Extraction de l'état des quantités et de la validation des commandes : **non effectuée** ; les contrôles NativeUI restent nécessaires.
- [ ] Compiler et vérifier la navigation Zee, les sous-menus et le téléphone après nettoyage.

## Migration Zee — services (26 octobre 2026)

- [x] Sauvegarde : `backup/pre-zee-services-decoupling-20261026`.
- [x] Zee : actions marchand d'armes, chauffeur et remboursement extraites des callbacks NativeUI.
- [x] Zee : menu personnalisé appelle directement ces actions, sans rendre `zeeMenu` visible artificiellement.
- [x] Callback NativeUI historique conservé en adaptateur temporaire.
- [ ] **À compiler et tester** : marchand d'armes (appel / renvoi), chauffeur (appel / annulation), remboursement partiel / total, contacts et menus.
- [ ] Zee : commandes de drogue toujours synchronisées aux sliders NativeUI ; extraire l'état et les actions métier avant suppression.
- [ ] Supprimer les définitions et dépendances NativeUI uniquement après migration des autres fonctionnalités (planques, véhicules, menus historiques).

## Lot Worker — alertes et découplage (25 octobre 2026)

- [x] Sauvegarde de sécurité : `backup/pre-worker-alerts-20261025` au commit `177d7d2`.
- [x] Alertes de stock Worker : cannabis, crack, cocaïne, et message prioritaire si tous les stocks sont vides.
- [x] Détection des transitions, anti-répétition jusqu'au réapprovisionnement, délai global entre notifications.
- [x] Alerte de recette nette à récolter, seuil configurable, réarmement après collecte.
- [x] Configuration `[WORKER_ALERTS]` documentée dans README ; notifications GTA avec son SMS, sans boîte de réception persistante.
- [x] Collecte de recette depuis CustomWorkerMenu sans callback NativeUI.
- [ ] **À compiler et tester en jeu** : ventes simulées, stock épuisé, réapprovisionnement, collecte, cooldown, sauvegardes, iFruit et menus.
- [x] Menu Worker : transfert d'armes appelé par index, sans dépendance au sélecteur NativeUI (ancien callback conservé pour compatibilité).
- [ ] Menu Zee : commandes de drogues et services encore reliés aux contrôles/callbacks NativeUI ; migration non terminée.
- [ ] Suppression complète de NativeUI : impossible tant que Setup.cs, LSL.cs et les anciens menus/callbacks le référencent.
- [ ] Tester le transfert d'armes Worker, les munitions, composants, sauvegardes et l'inventaire après migration.

# Roadmap — LS Life V2

> Branche de migration : `feature/custom-zee-menu`. `main` reste la référence stable. Ne déclarer une fonctionnalité terminée qu'après validation en jeu.

## État vérifié — 25 octobre 2026

> Mise à jour d'après le code de la branche et les retours de tests en jeu. « Fonctionnel en jeu » ne signifie pas « indépendant de NativeUI ».

- **Téléphone iFruit** : apparence et contacts validés après réorganisation de l'ordre de traitement dans `Tick.cs`.
- **Menus personnalisés** : Zee, employés, planque, clients, marchand d'armes et recrutement possèdent des interfaces CustomMenu ; plusieurs actions utilisent encore les objets ou événements NativeUI en arrière-plan.
- **Marchand d'armes** : achats et notifications validés en jeu ; disposition à améliorer.
- **Recrutement** : ouverture, offre acceptée, paiement et suivi du dealer validés en jeu. Repérage et contournement de réputation réservés à DEBUG.
- **HUD** : `HudRender.cs` utilise désormais les natives GTA V pour le HUD principal, les pressions et les panneaux DEBUG ; `CustomHudOptionsMenu.cs` remplace les réglages NativeUI. **Rendu, positionnement, maintien des touches et DEBUG validés en jeu par l'utilisateur.**
- **Dépendance NativeUI** : encore requise pour les menus historiques et des composants internes ; le HUD principal n'en dépend plus pour son rendu. Ne pas supprimer la DLL avant élimination des références.
- **Tests** : HUD et options confirmés fonctionnels en jeu par l'utilisateur ; audit des dépendances restantes dans `AUDIT_NATIVEUI.md`.

## Sauvegarde et migration NativeUI — 25 octobre 2026

- [x] Sauvegarde Git avant travaux : `backup/pre-nativeui-decoupling-20261025` (`bf9e174`).
- [x] Extraire deux actions métier du menu Zee sans modifier les stocks ni les sauvegardes.
- [ ] Compiler et tester en jeu : activer/désactiver le mode vente, valider une commande, vérifier le retour des menus et le téléphone.
- [ ] Découpler les sliders, la sélection de produits et les services Zee de NativeUI.

## Retours HUD — réglages et lisibilité (25 octobre 2026)

- [x] Structurer les options HUD : taille écran, stock, influence et pression, DEBUG conditionnel, enregistrer et retour.
- [x] Afficher les options DEBUG seulement lorsque AREADEBUG est activé dans l'état courant du mod.
- [x] Ajuster les coordonnées au pixel près, avec répétition automatique au maintien gauche/droite.
- [x] Masquer les panneaux DEBUG lorsque les menus personnalisés sont ouverts pour éviter la superposition.
- [x] Compiler et tester en jeu les nouveaux réglages, l'alignement des panneaux, le maintien des touches et la réapparition du DEBUG après fermeture (retour utilisateur : parfait).

## Correctifs HUD — 25 octobre 2026

- [x] Corriger la ligne Zee « Aucune dette en cours » : elle doit dépendre de la ligne de dette, et non du dernier élément du menu.
- [x] Ajouter trois groupes de positions indépendants : stocks, barres de pression et panneaux DEBUG, avec sauvegarde XML (`heatPosX/Y`, `debugPosX/Y`) et compatibilité des anciennes coordonnées.
- [x] Vérifier en jeu les réglages et le menu Zee corrigé (retour utilisateur positif ; persistance après redémarrage à recontrôler séparément).

## Priorité immédiate — Migrer réellement le HUD

- [x] Auditer les fonctions d'affichage du HUD principal, les barres de pression, les panneaux DEBUG, les paramètres de position et leurs sauvegardes.
- [x] Remplacer le rendu du HUD principal, des barres de pression et des quatre panneaux DEBUG par `HudRender.cs` utilisant les natives GTA V. **Affichage vérifié en jeu**. Le `BarHandler` historique utilise encore `TimerBarPool` : vérifier ses appels avant de le retirer.
- [x] Migrer les réglages de position vers `CustomHudOptionsMenu` accessible depuis Zee, sans `UIMenu*` dans `Hud.cs` ; sauvegarde XML et compatibilité `hudPosVersion` conservées. **Interface vérifiée en jeu ; nouveaux réglages à retester**.
- [ ] Garder les emplacements actuels : argent/stocks/mode vente en haut à droite ; pression joueur/quartier en bas à droite. Conserver les valeurs et conditions d'affichage historiques.
- [ ] Conserver le thème sombre avec accents dorés de la maquette approuvée ; mode compact, réputation et capacité des dealers restent des **options futures**, non activées par défaut.
- [ ] Vérifier la compilation, les affichages avec/sans menu, les différentes résolutions, le téléphone iFruit et les modes DEBUG ; obtenir validation en jeu.
- [ ] **Ne pas supprimer NativeUI.dll** : d'autres systèmes en dépendent encore.

## P0 — Stabilité et correctifs

- [x] Retrouver une base fonctionnelle.
- [x] Valider les nouveaux panneaux à hauteur automatique et les séparateurs à marges réelles.
- [x] Harmoniser les boutons sélectionnés et inactifs.
- [ ] **Corriger Échap clavier** : revenir au menu précédent sans ouvrir le menu pause de GTA V. Vérifier les contrôles désactivés et leur ordre d'exécution, sans casser la manette.
- [ ] Compiler et tester en jeu chaque étape ; vérifier les logs et les régressions.

## P1 — Migration NativeUI → CustomMenu (C#)

Objectif : menus et sous-menus autonomes, rendus via `CustomMenuRenderer` et `CustomMenuLayout`. Préserver les traitements métier existants. Supprimer NativeUI uniquement après audit complet de ses références.

- [x] **Zee** : menus et sous-menus personnalisés validés visuellement.
- [x] **Worker / Employé(e)s** : menu principal.
- [x] **Worker / Approvisionner**.
- [x] **Worker / Équipement**.
- [x] **Worker / Instruction** : sélection gauche/droite dans le menu principal (Ne rien faire / Me suivre / Travailler), validée en jeu ; conserver la fermeture historique lors du passage au travail.
- [ ] **Stash / Planque (maison)** : menu principal CustomMenu raccordé à l'interaction en jeu (à compiler et valider) ; retrait et dépôt restent provisoirement NativeUI. Source de vérité : `LSL.HouseHandler.CurrentHouse` (Weed/Crack/Cocain/Money/Armor/Weapons), sauvegarde de maison. Ne jamais mélanger avec le coffre de voiture.\n- [ ] **Stash / Coffre de voiture** : menu principal CustomMenu raccordé à l'interaction en jeu (à compiler et valider) ; retrait et dépôt restent provisoirement NativeUI. Auditer son propre inventaire, ses limites et sa sauvegarde avant modification. Ne jamais utiliser `CurrentHouse` pour le coffre.\n- [ ] **Stash / Vérification** : tester les transferts dans les deux sens, les quantités limites et la persistance de chaque stockage séparément.
- [x] **Customer / Clients** : interface personnalisée et interactions raccordées ; découplage NativeUI interne restant.
- [x] **Audit statique NativeUI** : inventaire des fichiers et ordre de migration documentés dans `AUDIT_NATIVEUI.md`.
- [ ] **Découplage métier** : début effectué sur Zee : démarrage du mode vente (`StartDealing`) et validation de commande (`PlaceDrugOrder`) ne nécessitent plus d'objets `UIMenu*` depuis CustomZeeMenu. **À compiler et tester en jeu** ; sliders, ajout de produits, services et autres callbacks restent à migrer.
- [ ] **Audit final après migration** : vérifier l'absence totale de références NativeUI avant suppression de la DLL.
- [ ] Supprimer les références et la dépendance NativeUI seulement lorsque plus aucun code ne l'utilise.

## P2 — Améliorations après migration

- [ ] **Alertes de réapprovisionnement Worker** : SMS de l’employé lorsque Cannabis, Crack ou Cocaïne est épuisé ; message spécifique lorsque tout le stock est vide. Prévoir anti-spam, seuils et déclenchement uniquement lors d’un changement de stock.\n- [ ] **Alerte recette Worker** : SMS lorsque la somme à récolter dépasse un seuil configurable ; éviter les répétitions jusqu’à la récolte.\n- [ ] **Équipement Worker** : donner une nouvelle arme et récupérer automatiquement l'ancienne ; gérer inventaire, munitions, composants, sauvegarde et cas limites.
- [x] **HUD** : rendu et options personnalisés, validés en jeu ; surveiller les régressions lors du découplage final de NativeUI.
- [ ] **HUD / Session** : afficher les bénéfices de la session actuelle, avec une définition claire des revenus et dépenses comptabilisés.
- [ ] **Livre des comptes** (étude de faisabilité) : historique des transactions, bénéfices, dépenses et synthèses par session ; évaluer persistance et performances avant développement.

## P3 — Traduction française et fiabilisation

- [ ] Compléter la traduction des textes visibles, notifications et instructions.
- [ ] Vérifier les libellés des armes, équipements et actions.
- [ ] Corriger les bugs reproduits, sans modifier inutilement le gameplay.
- [ ] Vérifier les sauvegardes et les parcours principaux.

## P4 — Conversion métrique finale

- [ ] Auditer les unités de stockage, d'affichage et de calcul.
- [ ] Planifier la conversion complète et la compatibilité des sauvegardes.
- [ ] Tester les stocks, ventes, achats, commandes et transferts.

**Conversion métrique à traiter en dernier**, conformément à la décision du projet.

## Méthode

1. **Lire ROADMAP.md et REGLES.md au début de chaque intervention**, puis le code réel de la branche ; mettre à jour la roadmap dès qu'une étape change.
2. Migrer un ensemble cohérent de menus sans modifier les traitements métier non concernés.
3. Centraliser les éléments de rendu réutilisables.
4. Compiler, tester dans GTA V et corriger les régressions.
5. Valider chaque menu avant de passer au suivant.
