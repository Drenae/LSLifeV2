# ROADMAP — LS Life V2

> Plan de travail uniquement : tâches à réaliser, sans historique de correctifs ni journal de commits.

## Socle de développement
- GTA V Legacy, mode histoire solo.
- C# / .NET Framework 4.8 / x64.
- ScriptHookVDotNet2 et menus CustomMenu existants.
- Branche stable de référence : `feature/custom-zee-menu`.
- Développement des nouvelles fonctionnalités sur `feature/gang-alliances`.
- Préserver la compatibilité des sauvegardes et les fonctionnalités existantes.

## Organisation UI et commandes
- [ ] Regrouper les menus actifs dans `LSlife.UI/Menus/`, sans melanger les anciens fichiers `DealerMenus.cs`, `PlayerMenus.cs`, etc. avec les nouveaux menus actifs avant audit de leurs usages.
- [ ] Renommer les classes/fichiers `Custom*Menu` en noms definitifs sans prefixe `Custom`, en actualisant toutes les references et en preservant `CustomMenu` comme composant technique jusqu'a migration complete.
- [ ] Verifier le stick gauche a pied, le D-pad et les commandes vehicule avec un menu ouvert, sur manette et clavier.

## Priorité 1 — Alliances entre gangs et cartels

### Étude du système existant
- [ ] Finaliser l'audit des gangs, chefs, territoires, relations, réputation, sauvegardes et interactions (classes Gang, Area, ThreatHandler, persistance et relations PNJ déjà examinées).
- [ ] Identifier précisément les mécanismes de guerre, recrutement, distribution et contrôle de zone réutilisables.

### Acces aux alliances
- [ ] Relier la reputation du joueur aux rencontres et missions pour chaque gang (la sauvegarde et les seuils existent, mais aucune progression en jeu pour le moment).
- [ ] Integrer les contacts des chefs dans iFruitAddon2 a partir du seuil de reputation et ouvrir la fiche du chef appele via `OpenContact`, sans contact automatique au debut.
- [ ] Ajouter les rendez-vous et rencontres physiques avec les chefs pour negocier, conclure et rompre les accords.
- [ ] Retirer les anciens parcours de navigateur de gangs devenus inutiles ; conserver les fiches individuelles pour les appels aux chefs.

- [ ] Valider en jeu le nouveau bloc HUD Alliances, ses coordonnees sauvegardees et le cas sans alliance (bloc masque).

### Renseignements et progression
- [ ] Ajouter la decouverte progressive des gangs par rencontres, presence sur leurs territoires et contacts, sans reveler leurs informations des le debut.
- [ ] Tester la migration des anciennes sauvegardes diplomatiques sans reputation et la conservation des valeurs apres Reload().
- [ ] Connecter les paliers de reputation du menu de chef a de vrais contacts et rencontres ; ne pas confondre reputation du joueur et confiance entre deux gangs.
- [ ] Verifier que les accords entre gangs tiers ne sont ni visibles ni modifiables par le joueur.

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
- [ ] Ajouter les interactions diplomatiques et les negociations avec les chefs dans les ecrans gangs/cartels, sans accord automatique.
- [ ] Relier les revenus affiches a l'economie effective, completer les informations des chefs et enrichir la fiche des membres de cartel.
- [ ] Tester les nouveaux ecrans Diplomatie / Accords, puis la restauration des territoires et alliances apres Reload() et les cas limites : mort d'un chef, disparition d'un gang, rupture d'accord.

### Validation
- [ ] Verifier en voiture et a pied que les touches des menus ne declenchent plus les commandes GTA (phares, radio, telephone, deplacement, attaque).
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
