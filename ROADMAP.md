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

## Suivi de traduction — fichiers source

**Règle :** cocher `[x]` uniquement après traduction exhaustive de tous les textes visibles du fichier, relecture des chaînes dynamiques et vérification de la compilation. Une première passe, même importante, **ne signifie pas 100 %**. Les fichiers sans textes destinés au joueur pourront être cochés après vérification explicite.

### Fichiers déjà identifiés (tous à finaliser)

- [ ] `LSlife/LSL.cs` — menus principaux, commandes, planques, stockage ; traduction partielle.
- [ ] `LSlife/LsMenus.cs` — interactions et dialogues ; traduction partielle.
- [ ] `LSlife/LsFunctions.cs` — notifications, messages, textes dynamiques ; traduction partielle.
- [ ] `LSlife/DrugOrder.cs` — commande et livraison ; traduction partielle.
- [ ] `LSlife/InventoryText.cs` — interface d'inventaire ; traduction partielle.
- [ ] `LSlife/StashHouse.cs` — planques ; première passe effectuée, audit complet restant.
- [ ] `LSlife/StashVehicle.cs` — véhicules de stockage ; traduction partielle.
- [ ] `LSlife/PlayerDealer.cs` — dealers recrutés ; traduction partielle.

### Inventaire complet du dépôt

- [ ] **Recenser tous les autres fichiers source du dépôt** (y compris sous-dossiers) et ajouter une case pour chacun. La liste ci-dessus recense les fichiers vérifiés jusqu'ici ; elle n'est pas encore certifiée exhaustive.
- [ ] Recenser les ressources contenant du texte visible (fichiers de configuration, XML, sprites portant des libellés, etc.) et ajouter des cases adaptées.

**Progression validée : 0/8 fichiers identifiés à 100 %** — aucun fichier n'a encore fait l'objet d'un audit complet et d'une validation de compilation pour la traduction finale.

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
