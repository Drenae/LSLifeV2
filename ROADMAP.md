# ROADMAP — LS Life V2

> Plan de travail uniquement : tâches à réaliser, sans historique de correctifs ni journal de commits.

## Socle de développement
- GTA V Legacy, mode histoire solo.
- C# / .NET Framework 4.8 / x64.
- ScriptHookVDotNet2 et menus CustomMenu existants.
- Branche stable de référence : `feature/custom-zee-menu`.
- Développement des nouvelles fonctionnalités sur `feature/gang-alliances`.
- Préserver la compatibilité des sauvegardes et les fonctionnalités existantes.

## Priorité 1 — Alliances entre gangs et cartels

### Étude du système existant
- [ ] Finaliser l'audit des gangs, chefs, territoires, relations, réputation, sauvegardes et interactions (classes Gang, Area, ThreatHandler, persistance et relations PNJ déjà examinées).
- [ ] Identifier précisément les mécanismes de guerre, recrutement, distribution et contrôle de zone réutilisables.

### Diplomatie et relations
- [ ] Relier la confiance diplomatique aux actions du joueur et aux événements de gangs.
- [ ] Ajouter des rencontres avec les chefs et des propositions diplomatiques interactives (le navigateur actuel reste consultatif).
- [ ] Permettre de refuser, rompre ou renégocier une alliance.

### Accords et fonctionnement d'un cartel
- [ ] Prévoir les contributions et le partage des revenus, avec règles transparentes.
- [ ] Gérer les rivalités, trahisons et conséquences sur les relations et territoires.

### Carte des territoires (faisabilite a valider)
- [ ] Etudier l'affichage des territoires des gangs sur la carte du menu Pause uniquement, sans marquage sur la minimap.
- [ ] Si les API GTA V Legacy / SHVDN2 le permettent, colorer les zones controlees par gang ; laisser les zones libres sans marquage.
- [ ] Verifier la precision des limites de quartiers, les performances et la compatibilite avec les autres mods de carte avant implementation.

### Interface et sauvegarde
- [ ] Completer le navigateur des gangs avec les interactions diplomatiques et un ecran Cartel, sans permettre d'accord automatique sans rencontre.
- [ ] Completer les fiches gangs : accords, detail des territoires, revenus et informations des chefs.
- [ ] Tester en jeu la restauration des territoires et des alliances apres Reload(), ainsi que les cas limites : mort d'un chef, disparition d'un gang, rupture d'accord.

### Validation
- [ ] Implémenter par lots cohérents sans modifier les mécaniques non concernées.
- [ ] Compiler avec `Build-Baseline.ps1` après chaque lot.
- [ ] Tester en jeu la création, l'évolution et la dissolution des alliances.

## Priorité 2 — Évolutions gameplay
- [ ] Notifications des employés : rupture de stock et recettes à récupérer.
- [ ] Équipement des employés : échange d'armes, munitions et sauvegarde.
- [ ] Livre des comptes : recettes, dépenses et bénéfices de la session.
- [ ] Examiner les interactions entre alliances, zones et pression policière.

## Priorité 3 — Qualité et finition
- [ ] Compléter les traductions françaises visibles.
- [ ] Corriger les bugs reproductibles signalés pendant les tests.
- [ ] Vérifier les menus, le téléphone et les sauvegardes apres integration de la diplomatie.
- [ ] Étudier une conversion métrique globale en dernier, avec migration sûre des données.

## Règles de suivi
- Conserver ici uniquement les tâches ouvertes et les prochaines étapes.
- Retirer les tâches terminées au lieu d'accumuler des journaux de correctifs.
- Consulter `REGLES.md` avant toute modification.
- Ne pas engager de migration SHVDN3 sans nouvelle demande explicite.
