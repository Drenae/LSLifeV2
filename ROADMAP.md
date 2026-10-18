# LS Life V2 — Roadmap

> Branche de travail : `main`. Sauvegarde de référence : `backup/v2-stable-before-fr`.
> V2 : ScriptHookVDotNet2 + NativeUI. La migration SHVDN3 / LemonUI est réservée à une éventuelle V3.

## Phase 1 — Traduction française complète (EN COURS)

- [x] Première passe sur les menus, le stockage et certains dialogues.
- [x] Correction de l'identifiant `sellingCocaine` modifié accidentellement.
- [x] Clarification initiale de l'unité « oz » dans le HUD.
- [ ] Inventorier tous les textes visibles : menus, descriptions, notifications, sous-titres, messages iFruit, aide et options.
- [ ] Traduire et harmoniser les textes en français naturel, accents compris.
- [ ] Remplacer les mentions **visibles** de Zee par **Rafael Mendoza** ; conserver les identifiants internes nécessaires à la compatibilité.
- [ ] Vérifier les textes dynamiques et les placeholders, sans modifier les clés de sauvegarde.
- [ ] Compiler et tester les interactions principales en jeu.

## Suivi de traduction — inventaire exhaustif des fichiers C#

**Règle :** `[x]` après audit intégral des textes visibles et relecture du fichier ; la compilation reste une validation distincte à effectuer sur l'ensemble du projet. Les fichiers purement techniques sont également inspectés avant validation. Le statut « partiel » ne vaut pas terminé.

### Racine (1)

- [x] `-PrivateImplementationDetails-.cs` — audit intégral : aucun texte utilisateur à traduire

### LSlife.Gangs (4)

- [x] `LSlife.Gangs/Gang.cs` — audit intégral : aucun texte utilisateur à traduire
- [x] `LSlife.Gangs/GangUtilities.cs` — audit intégral : aucun texte utilisateur à traduire ; noms de gangs conservés volontairement
- [x] `LSlife.Gangs/Threat.cs` — audit intégral : aucun texte utilisateur à traduire
- [x] `LSlife.Gangs/ThreatHandler.cs` — audit intégral : aucun texte utilisateur à traduire

### LSlife.HeatBars (3)

- [x] `LSlife.HeatBars/AreaHeatBar.cs` — audit intégral des chaînes et libellés effectué
- [x] `LSlife.HeatBars/BarHandler.cs` — audit intégral des chaînes et libellés effectué
- [x] `LSlife.HeatBars/PlayerHeatBar.cs` — audit intégral des chaînes et libellés effectué

### LSlife.Inventory (4)

- [x] `LSlife.Inventory/Cocaine.cs` — audit intégral : aucun libellé codé en dur, paramètres de texte conservés
- [x] `LSlife.Inventory/InventoryObject.cs` — audit intégral : aucun libellé codé en dur, paramètres de texte conservés
- [x] `LSlife.Inventory/ItemObject.cs` — audit intégral : aucun libellé codé en dur, paramètres de texte conservés
- [x] `LSlife.Inventory/ItemSlot.cs` — audit intégral : aucun libellé codé en dur, paramètres de texte conservés

### LSlife.OldCustomers (2)

- [ ] `LSlife.OldCustomers/OldCustomer.cs`
- [ ] `LSlife.OldCustomers/OldCustomerHandler.cs`

### LSlife.Shots (2)

- [ ] `LSlife.Shots/Shot.cs`
- [ ] `LSlife.Shots/ShotHandler.cs`

### LSlife (35)

- [ ] `LSlife/Area.cs`
- [ ] `LSlife/AreaDealer.cs`
- [ ] `LSlife/Customer.cs`
- [ ] `LSlife/CustomerPed.cs`
- [ ] `LSlife/Dealer.cs`
- [ ] `LSlife/DealerSpawnPostion.cs`
- [ ] `LSlife/DefaultPed.cs`
- [ ] `LSlife/DriveBy.cs`
- [ ] `LSlife/DriveByHandler.cs`
- [ ] `LSlife/DropDrugs.cs`
- [ ] `LSlife/DroppedBag.cs`
- [ ] `LSlife/Drug.cs`
- [ ] `LSlife/DrugDealer.cs`
- [ ] `LSlife/DrugOrder.cs` — traduction entamée, audit final restant
- [ ] `LSlife/EnemyHandler.cs`
- [ ] `LSlife/Enums.cs`
- [ ] `LSlife/HeadShot.cs`
- [ ] `LSlife/HeadShotHandler.cs`
- [ ] `LSlife/HirePed.cs`
- [ ] `LSlife/InventoryText.cs` — traduction entamée, audit final restant
- [ ] `LSlife/LSL.cs` — traduction entamée, audit final restant
- [ ] `LSlife/LsFunctions.cs` — traduction entamée, audit final restant
- [ ] `LSlife/LsLPeds.cs`
- [ ] `LSlife/LsMenus.cs` — traduction entamée, audit final restant
- [ ] `LSlife/LsTimer.cs`
- [ ] `LSlife/PedDebugger.cs`
- [ ] `LSlife/PedTeam.cs`
- [ ] `LSlife/PickupHandler.cs`
- [ ] `LSlife/Pig.cs`
- [ ] `LSlife/PlayerDealer.cs` — traduction entamée, audit final restant
- [ ] `LSlife/PlayerDealerHandler.cs`
- [ ] `LSlife/Prison.cs`
- [ ] `LSlife/StashHouse.cs` — traduction entamée, audit final restant
- [ ] `LSlife/StashHouseHandler.cs`
- [ ] `LSlife/StashVehicle.cs` — traduction entamée, audit final restant

### Properties (1)

- [ ] `Properties/AssemblyInfo.cs`

**Suivi : 12/52 fichiers C# validés à 100 % pour la traduction (compilation en attente du test utilisateur).**

**Périmètre :** inventaire généré depuis l'arborescence GitHub de `main` ; fichiers `.cs` uniquement. Les scripts de build, projets et autres ressources ne sont pas des fichiers de traduction C#.

## Phase 2 — Identité graphique et crédits

- [ ] Créer et intégrer une bannière principale LS Life V2.
- [ ] Créer et intégrer les cinq sprites NativeUI : Customer, Dealer, Stash, Worker, Mendoza.
- [ ] Adopter la direction visuelle sombre émeraude / noir / or.
- [ ] Vérifier le rendu des menus et les tailles/chemins des textures en jeu.
- [ ] Adapter les crédits en respectant l'auteur d'origine ; demander le pseudonyme exact à ajouter.

## Phase 3 — Système métrique français (APRÈS traduction et graphismes)

- [ ] Auditer toutes les opérations dépendantes des onces : commandes, prix, livraisons, ventes, stockage, poids porté et conversions.
- [ ] Définir un modèle interne cohérent en grammes, avec kilogrammes pour l'affichage.
- [ ] Format attendu : `500 g`, `1 kg`, `1 kg 250 g` (pas `1,25 kg`).
- [ ] Préserver **deux états de stock distincts** : non préparé (rouge) et prêt à vendre (vert).
- [ ] Adapter les commandes, la préparation en planque, les inventaires joueur/véhicule/planque et les calculs de prix.
- [ ] Vérifier les contraintes d'achats (quantités entières autorisées, minimums et prix) sans introduire d'arrondis exploitables.
- [ ] Mettre en place une migration des anciennes sauvegardes en onces ; sauvegarder l'original avant conversion.
- [ ] Tester les anciennes et nouvelles sauvegardes, notamment les transferts entre stocks.

## Phase 4 — Stabilisation et validation

- [ ] Exécuter `Build-Baseline.ps1`, `Audit-Warnings.ps1` et `Verify-Build.ps1`.
- [ ] Vérifier **zéro erreur de compilation** ; les avertissements SHVDN2 `CS0618` restent une dette technique connue de V2.
- [ ] Tester en jeu : contact fournisseur, commandes, livraison, conversion, vente, planques, véhicules, dealers et sauvegardes.
- [ ] Corriger les régressions puis publier une version V2 française stabilisée.

## Règles de modification

- Faire des commits ciblés et réversibles.
- Ajouter des commentaires français **utiles** autour des changements de mécanique, de conversion et de migration, avec un repère `LSL V2 FR` ; éviter les commentaires superflus sur les simples libellés.
- Ne jamais traduire les noms de variables, clés XML, chemins, identifiants d'API ou ressources sans analyse d'impact.
- Ne pas déclarer une phase validée avant compilation et tests en jeu.
