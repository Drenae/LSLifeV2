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

### Audit des conversions — diagnostic détaillé (18/10/2026)

**Inventaire des circuits vérifiés :** `DrugOrder.cs`, `LSL.cs`, `LsFunctions.cs`, `LsMenus.cs`, `StashHouse.cs`, `StashHouseHandler.cs`, `StashVehicle.cs`, `InventoryText.cs`, `PlayerDealer.cs`, `DrugDealer.cs`, `Shot.cs`, `DropDrugs.cs`, `DroppedBag.cs`.

| Circuit | État actuel | Conversion nécessaire |
| --- | --- | --- |
| Commande fournisseur | `DrugOrder.weed/crack/cocaine` comptent des paquets de 28 g ; prix par paquet ; `OrderSizeInGrams() = somme × 28` | Choisir le pas d'achat (paquet de 28 g conservé ou nouvelle quantité entière en grammes) et recalculer les prix sans arrondi exploitable |
| Réception | `PickupDrugs()` crédite les compteurs `p*Ounce` ou `drugCar*Oz` | Stocker les grammes **non préparés**, sans les confondre avec les grammes prêts à vendre |
| Préparation en planque | `StashHouse.AddDrugs()` et `LsMenus.onStashMenuSelect()` multiplient les paquets par 28 et les ajoutent à `StashHouse.Weed/Crack/Cocain` | Conserver une conversion explicite **non préparé → préparé** et la quantité totale |
| Transferts véhicule | `LsMenus.cs` transfère séparément grammes préparés et compteurs de paquets ; sauvegarde XML à chaque action | Conserver deux voies de transfert et contrôler les limites / soldes |
| Ventes et employés | `PlayerInventory`, `PlayerDealer.Drugs`, `Customer.amount` et `PricePerG` sont déjà en grammes | Préserver ces valeurs ; modifier seulement le formatage des quantités |
| HUD et menus | `GramsToOz()` transforme les grammes préparés en `oz + g` ; les paquets rouges affichent `oz` | Remplacer par `500 g`, `1 kg`, `1 kg 250 g` ; conserver les codes couleur GTA |
| Limite de poids | `pMaxDrugs = 7168` g ; `DrugOrder.PlayerCanPickup()` utilise `< 7168` | Clarifier la limite exacte, l'inclusion de la borne et la prise en compte des stocks |
| Sauvegardes | `LSLifeSave.xml` et `LSLife_Vehicles.xml` utilisent des champs `*Oz` ; `LSLife_StashHouses.xml` contient des grammes préparés | Migration **versionnée, idempotente et sauvegardée** avant toute écriture |

**Points de vigilance relevés dans le code :**
- `LsFunctions.TotalDrugs(Dictionary<...>)` additionne toujours les onces **du joueur**, même lorsqu'il reçoit l'inventaire d'un employé ; ne pas changer ce calcul isolément sans vérifier tous les appels.
- Le code véhicule mélange `cocaineOz` et `cocainOz` : relever tous les lecteurs et écrivains et choisir un seul schéma cible.
- `StashVehicle.TotalDrugs()` additionne uniquement le stock préparé ; vérifier chaque appel qui a besoin du poids **total**.
- Le tableau `LSL.multiplier` contient notamment `28`, `280`, `2800` ; adapter le pas des curseurs à l'unité retenue.
- `DrugOrder.PlayerCanPickup()` compare strictement `< 7168`, alors que la constante `pMaxDrugs` existe déjà.
- `GramsToOz()` est utilisée par les ventes, HUD et menus ; prévoir une fonction métrique unique plutôt que des conversions locales.

**Architecture cible proposée (à appliquer après audit de toutes les écritures XML) :**
1. Quantités internes entières en grammes : `RawWeedGrams`, `RawCrackGrams`, `RawCocaineGrams` (non préparé) distinctes des quantités préparées actuelles. Garder les anciens identifiants tant que la migration et les appels ne sont pas adaptés.
2. Conversion `1 paquet historique = 28 g` **uniquement lors de la migration** et aux points d'entrée d'une éventuelle vente de paquets historiques.
3. Affichage centralisé : `0 g`, `500 g`, `1 kg`, `1 kg 250 g`. Pas de nombres décimaux.
4. Préparation en planque : soustraire le brut et créditer la même masse au stock prêt à vendre ; les transferts ne changent jamais l'état du produit.
5. Migration : créer une copie de sécurité des XML **avant tout chargement qui les réécrit**, ajouter un marqueur de version, multiplier les anciens champs `*Oz` par 28 une seule fois, puis relire et vérifier les totaux. Ne jamais migrer un fichier isolé si la sauvegarde en contient plusieurs.
6. Validation : tests de non-régression pour achats, paiement, livraison, ramassage, stockage véhicule, dépôt/préparation en planque, revente, sacs au sol, persistance et chargement d'anciennes sauvegardes.

**État :** audit des principaux circuits terminé ; modification des mécaniques et migration **non commencées**. Vérification exhaustive de tous les appels et XML encore nécessaire avant la bascule.

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
