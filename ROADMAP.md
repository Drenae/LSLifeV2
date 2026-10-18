# LS Life V2 — Roadmap

> Branche de travail : `main`. Sauvegarde de référence : `backup/v2-stable-before-fr`.
> V2 : ScriptHookVDotNet2 + NativeUI. La migration SHVDN3 / LemonUI est réservée à une éventuelle V3.

## Phase 1 — Traduction française complète (VALIDÉE — build et tests utilisateur OK)

- [x] Première passe sur les menus, le stockage et certains dialogues.
- [x] Correction de l'identifiant `sellingCocaine` modifié accidentellement.
- [x] Clarification initiale de l'unité « oz » dans le HUD.
- [x] Inventorier tous les textes visibles : menus, descriptions, notifications, sous-titres, messages iFruit, aide et options.
- [x] Traduire et harmoniser les textes en français naturel, accents compris.
- [x] Remplacer les mentions **visibles** de Zee par **Rafael Mendoza** ; conserver les identifiants internes nécessaires à la compatibilité.
- [x] Vérifier les textes dynamiques et les placeholders, sans modifier les clés de sauvegarde.
- [x] Compiler et tester les interactions principales en jeu.

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

- [x] `LSlife.OldCustomers/OldCustomer.cs` — textes et messages audités intégralement, traduction vérifiée
- [x] `LSlife.OldCustomers/OldCustomerHandler.cs` — textes et messages audités intégralement, traduction vérifiée

### LSlife.Shots (2)

- [x] `LSlife.Shots/Shot.cs` — textes et messages audités intégralement, traduction vérifiée
- [x] `LSlife.Shots/ShotHandler.cs` — textes et messages audités intégralement, traduction vérifiée

### LSlife (35)

- [x] `LSlife/Area.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/AreaDealer.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/Customer.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/CustomerPed.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/Dealer.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/DealerSpawnPostion.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/DefaultPed.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/DriveBy.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/DriveByHandler.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/DropDrugs.cs` — chaînes visibles auditées, identifiants internes conservés
- [x] `LSlife/DroppedBag.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/Drug.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/DrugDealer.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/DrugOrder.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés — traduction entamée, audit final restant
- [x] `LSlife/EnemyHandler.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/Enums.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/HeadShot.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/HeadShotHandler.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/HirePed.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés
- [x] `LSlife/InventoryText.cs` — audit intégral des chaînes, identifiants et sauvegardes préservés — traduction entamée, audit final restant
- [x] `LSlife/LSL.cs` — audit des textes, clés techniques et identifiants conservés — traduction entamée, audit final restant
- [x] `LSlife/LsFunctions.cs` — audit des textes, clés techniques et identifiants conservés — traduction entamée, audit final restant
- [x] `LSlife/LsLPeds.cs` — audit des textes, clés techniques et identifiants conservés
- [x] `LSlife/LsMenus.cs` — audit des textes, clés techniques et identifiants conservés — traduction entamée, audit final restant
- [x] `LSlife/LsTimer.cs` — audit des textes, clés techniques et identifiants conservés
- [x] `LSlife/PedDebugger.cs` — audit des textes, clés techniques et identifiants conservés
- [x] `LSlife/PedTeam.cs` — audit des textes, clés techniques et identifiants conservés
- [x] `LSlife/PickupHandler.cs` — audit des textes, clés techniques et identifiants conservés
- [x] `LSlife/Pig.cs` — audit des textes, clés techniques et identifiants conservés
- [x] `LSlife/PlayerDealer.cs` — audit des textes, clés techniques et identifiants conservés — traduction entamée, audit final restant
- [x] `LSlife/PlayerDealerHandler.cs` — audit des chaînes et compatibilité des identifiants/sauvegardes
- [x] `LSlife/Prison.cs` — audit des chaînes et compatibilité des identifiants/sauvegardes
- [x] `LSlife/StashHouse.cs` — audit des chaînes et compatibilité des identifiants/sauvegardes — traduction entamée, audit final restant
- [x] `LSlife/StashHouseHandler.cs` — audit des chaînes et compatibilité des identifiants/sauvegardes
- [x] `LSlife/StashVehicle.cs` — audit des chaînes et compatibilité des identifiants/sauvegardes — traduction entamée, audit final restant

### Properties (1)

- [x] `Properties/AssemblyInfo.cs` — audit des chaînes et compatibilité des identifiants/sauvegardes

**Suivi : 52/52 fichiers C# audités pour la traduction ; compilation et tests utilisateur validés.**

**Périmètre :** inventaire généré depuis l'arborescence GitHub de `main` ; fichiers `.cs` uniquement. Les scripts de build, projets et autres ressources ne sont pas des fichiers de traduction C#.

## Phase 2 — Identité graphique et crédits (PARTIELLE — suite reportée)

- [ ] Créer et intégrer une bannière principale LS Life V2.
- [x] Créer les cinq bannières : Customer, Dealer, Stash, Worker, Mendoza (PNG livrés séparément ; installation et rendu en jeu non vérifiés ici).
- [x] Définir et appliquer aux cinq bannières la direction visuelle sombre émeraude / noir / or.
- [ ] Vérifier le rendu des menus et les tailles/chemins des textures en jeu.
- [ ] Adapter les crédits en respectant l'auteur d'origine ; demander le pseudonyme exact à ajouter.

## Phase 3 — Système métrique français (EN COURS — audit préalable)

- [ ] Auditer toutes les opérations dépendantes des onces : commandes, prix, livraisons, ventes, stockage, poids porté et conversions. (Audit commencé : `LsFunctions.cs`, `LsMenus.cs`, `DrugOrder.cs`, `LSL.cs`, `StashHouseHandler.cs`, `StashVehicle.cs`, `InventoryText.cs` ; reste à couvrir tous les chemins.)
- [ ] Définir un modèle interne cohérent en grammes, avec kilogrammes pour l'affichage.
- [ ] Format attendu : `500 g`, `1 kg`, `1 kg 250 g` (pas `1,25 kg`).
- [ ] Préserver **deux états de stock distincts** : non préparé (rouge) et prêt à vendre (vert).
- [ ] Adapter les commandes, la préparation en planque, les inventaires joueur/véhicule/planque et les calculs de prix.
- [ ] Vérifier les contraintes d'achats (quantités entières autorisées, minimums et prix) sans introduire d'arrondis exploitables.
- [ ] Mettre en place une migration des anciennes sauvegardes en onces ; sauvegarder l'original avant conversion.
- [ ] Tester les anciennes et nouvelles sauvegardes, notamment les transferts entre stocks.

### Premiers constats techniques — phase 3

- `LsFunctions.pWeedOunce`, `pCrackOunce`, `pCocaineOunce` et les stocks véhicules utilisent encore des quantités en onces ; le poids porté ajoute `28 g` par once.
- `DrugOrder.cs` transfère les commandes en onces et applique une limite de `7168 g` : vérifier sa cohérence avant conversion.
- `LsMenus.cs` transfère les onces entre joueur et véhicule, avec écritures XML immédiates.
- `LSLifeSave.xml` et les véhicules conservent des clés `weedOz`, `crackOz`, `cocaineOz`/`cocainOz` : une migration versionnée avec sauvegarde préalable est indispensable.
- Le stock prêt à vendre est déjà distinct dans `PlayerInventory` et les stocks des planques ; ne jamais fusionner les deux états.
- Aucune mécanique de poids, commande ou sauvegarde modifiée à cette étape : audit uniquement.

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
