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
- [ ] Auditer les gangs, leurs chefs, territoires, relations, réputation, sauvegardes et interactions.
- [ ] Identifier les mécanismes de guerre, recrutement, distribution et contrôle de zone réutilisables.
- [ ] Définir les règles de compatibilité avec les sauvegardes actuelles.

### Diplomatie et relations
- [ ] Définir les états de relation : hostile, neutre, négociation, allié, cartel.
- [ ] Définir une confiance/réputation entre gangs et les conditions de progression.
- [ ] Ajouter des rencontres et propositions diplomatiques via un menu adapté.
- [ ] Permettre de refuser, rompre ou renégocier une alliance.

### Accords et fonctionnement d'un cartel
- [ ] Concevoir les accords : non-agression, commerce, soutien armé, défense mutuelle.
- [ ] Définir la création d'un cartel regroupant plusieurs gangs.
- [ ] Définir le rôle du joueur : fondateur, partenaire ou dirigeant, selon le gameplay existant.
- [ ] Prévoir les contributions et le partage des revenus, avec règles transparentes.
- [ ] Gérer les rivalités, trahisons et conséquences sur les relations et territoires.

### Interface et sauvegarde
- [ ] Concevoir un menu « Diplomatie / Cartel » cohérent avec CustomMenu.
- [ ] Afficher membres, statut, confiance, accords, territoires et revenus.
- [ ] Sauvegarder les alliances et restaurer leur état au chargement.
- [ ] Tester les cas limites : mort d'un chef, disparition d'un gang, rupture d'accord, rechargement.

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
- [ ] Vérifier les menus, le HUD, le téléphone et les sauvegardes.
- [ ] Étudier une conversion métrique globale en dernier, avec migration sûre des données.

## Règles de suivi
- Conserver ici uniquement les tâches ouvertes et les prochaines étapes.
- Retirer les tâches terminées au lieu d'accumuler des journaux de correctifs.
- Consulter `REGLES.md` avant toute modification.
- Ne pas engager de migration SHVDN3 sans nouvelle demande explicite.
