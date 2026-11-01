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

### Progression : rencontre et reputation
- [ ] Identifier les chefs de gangs en monde ouvert, sans donner leurs contacts automatiquement.
- [ ] Creer la premiere rencontre du vrai chef et de vraies missions de gang. L'interaction transitoire avec un dealer local exige maintenant son etat TalkToPlayer, n'intercepte pas Sprint et limite le gain de +5 par gang toutes les 2 minutes. A remplacer par un representant distinct du recrutement.
- [ ] Ajouter les pertes de reputation et des missions reelles, puis tester la persistance des gains de services locaux apres Reload().
- [ ] Retester en jeu la coexistence des interactions Sprint (recrutement) et E (reputation) apres correction de la lecture de JustPressed a chaque tick ; verifier l'aide contextuelle partagee, puis les paliers : contact 25, territoires 35, membres 45, relations 55, alliance 60, tresorerie 75.

### Telephone des chefs
- [ ] Tester en jeu l'ajout dynamique des contacts iFruit apres cinq services au meme gang (+5 chacun), puis leur desactivation si la reputation baisse.
- [ ] Tester l ouverture du menu individuel du chef appele, la navigation, le raccrochage et l absence de navigateur global.
- [ ] Finaliser le menu : dirigeant, territoires, membres, tresorerie, relations, demander un rendez-vous et raccrocher ; connecter les valeurs inconnues aux donnees de gameplay reelles.
- [ ] Tester en jeu les verrouillages des sous-menus de chef selon les seuils de reputation, notamment apres perte de confiance, et verifier qu'aucune relation entre deux gangs tiers n'est affichee.
- [ ] Retirer les parcours de navigateur de gangs/cartels devenus inutiles apres validation.

### Rendez-vous et negociations en personne
- [ ] Demander un rendez-vous par telephone : lieu adapte, confirmation, marqueur temporaire et expiration.
- [ ] Identifier ou faire apparaitre le chef au lieu du rendez-vous et permettre une interaction en personne.
- [ ] Ouvrir un menu distinct de negociations uniquement face au chef, jamais par simple appel.
- [ ] Commencer par la demande d alliance au seuil requis (60/100 de reputation personnelle, gang du joueur requis et confiance diplomatique suffisante) ; debloquer ensuite d autres accords progressivement.
- [ ] Gerer les refus, ruptures, renegociations, reactions et consequences sur la reputation.
- [ ] Interdire la modification des accords entre deux gangs tiers.

### HUD et sauvegarde
- [ ] Valider le bloc HUD Alliances avec et sans allies, son placement X/Y et sa sauvegarde.
- [ ] Tester la migration des anciennes sauvegardes sans reputation et la persistance des relations (les relations vers des gangs absents sont maintenant ignorees au chargement).
- [ ] Tester le comportement apres Reload(), disparition d un gang ou mort d un chef.

### Cartels (apres validation des gangs)
- [ ] Definir la creation d un cartel a partir de plusieurs gangs allies et l accord explicite des chefs.
- [ ] Prevoir les contributions, le partage des revenus, les rivalites et les trahisons.
- [ ] Etudier l impact des alliances sur les territoires, la pression policiere et les guerres de gangs.

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
