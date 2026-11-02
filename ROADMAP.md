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

## Correctifs a valider en jeu
- [ ] Appeler un chef : conserver l'appel actif et l'animation du telephone a l'oreille pendant le CustomMenu ; fermer iFruit via Close(1000) uniquement sur Raccrocher ou Retour, puis verifier la reouverture normale du telephone.
- [ ] Verifier l'identite graphique des en-tetes CONTACT et INFORMATIONS (memes separateurs, couleurs et police).
- [ ] Verifier que le proprietaire d'une zone ne change plus entre plusieurs Reload() consecutifs grace a la sauvegarde immediate du premier proprietaire ; ne pas reinitialiser les sauvegardes existantes.

## Priorité 1 — Alliances entre gangs et cartels

### Persistance des quartiers decouverts
- [ ] Tester le parcours de plusieurs quartiers et verifier leur apparition immediate dans LSLife_GangDiplomacy.xml, puis leur conservation apres Reload().
- [ ] Auditer LS LifeAreas.xml : son chargement historique n'enregistre pas les nouvelles zones visitees ; definir si un inventaire complet doit y etre ajoute sans perturber les statistiques existantes.
- [ ] Ne jamais ecraser les proprietaires sauvegardes lors d'une nouvelle decouverte.

### Territoires natifs GTA V
- [ ] Valider la migration unique des quartiers connus et la conservation des reputations, relations et sauvegardes.
- [ ] Tester la persistance des conquetes apres plusieurs Reload().
- [ ] Cartographier les quartiers restants et supprimer progressivement les proprietaires aleatoires.
- [ ] Tester en jeu les modeles natifs distincts des representants (Ballas, Families, Vagos, Aztecas, Marabunta et Lost MC), leur spawn et leur nettoyage au Reload().
- [ ] Completer le mapping des autres factions et des quartiers natifs seulement apres validation de leurs emplacements et modeles.

### Étude du système existant
- [ ] Finaliser l'audit des gangs, chefs, territoires, relations, réputation, sauvegardes et interactions (classes Gang, Area, ThreatHandler, persistance et relations PNJ déjà examinées).
- [ ] Identifier précisément les mécanismes de guerre, recrutement, distribution et contrôle de zone réutilisables.

### Progression : rencontre et reputation
- [ ] Identifier les chefs de gangs en monde ouvert, sans donner leurs contacts automatiquement.
- [ ] Creer la premiere rencontre du vrai chef et de vraies missions de gang. Le representant est maintenant distinct des dealers, reste fixe et regarde le joueur. Le gain est de +5, avec 3 secondes en DEBUG et 2 minutes hors DEBUG. Reste a construire de vraies missions.
- [ ] Ajouter les pertes de reputation et des missions reelles, puis tester la persistance des gains de services locaux apres Reload().
- [ ] Verifier le PNJ representant au sol, son immobilite, le cooldown DEBUG de 3 secondes independant pour chaque gang et les paliers APRES rencontre du chef.

### Telephone des chefs
- [ ] Tester en jeu l'ajout dynamique des contacts iFruit apres cinq services au meme gang (+5 chacun), puis leur desactivation si la reputation baisse.
- [ ] Tester le raccrochage du chef (action Raccrocher et touche Retour) : fermeture du telephone et fin de l'animation d'appel, sans perturber Zee ; verifier iFruit apres nouvel appel.
- [ ] Tester le deblocage des actions Territoires (35) et Relations (55) uniquement apres rencontre physique, et leur disparition si la reputation redescend. Les informations restent non selectionnables.
- [ ] Tester en jeu les verrouillages des sous-menus de chef selon les seuils de reputation, notamment apres perte de confiance, et verifier qu'aucune relation entre deux gangs tiers n'est affichee.
- [ ] Retirer les parcours de navigateur de gangs/cartels devenus inutiles apres validation.

### Rendez-vous immersifs sur le modele de Zee
- [ ] Tester le filtrage des lieux Zee (400-1800 m) hors territoires natifs et hors proprietaires sauvegardes ; si aucun lieu neutre n'est disponible, ajouter un lieu fixe valide en jeu sous un pont ou dans une zone sans gang.
- [ ] Tester les modeles distincts chef / accompagnateur et le marqueur jaune au-dessus du chef a courte distance.
- [ ] Garder les deux PNJ sur place pendant l'interaction, sans disparition immediate du chef.
- [ ] Tester que le conducteur ne part qu'une fois le chef et l'accompagnateur tous deux remontes dans LE vehicule du rendez-vous ; aucun depart seul.
- [ ] Tester expiration, Reload(), vehicule detruit, mort des PNJ et blocage des rendez-vous concurrents ; verifier l'absence de PNJ orphelins.

### Rendez-vous et negociations en personne
- [ ] Tester les rendez-vous demandes par telephone : apparition du chef distinct, marqueur temporaire, expiration (5 min DEBUG / 10 min normal) et nettoyage au Reload().
- [ ] Tester la premiere rencontre physique du chef et la persistance XML de metLeaders apres Reload(), sans attribuer de faux nom au chef ; ameliorer ensuite les lieux de rendez-vous pour les rendre propres a chaque gang.
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

### Carte des territoires — TOUS les gangs decouverts
- [x] Phase 1 validee en jeu : points colores de tous les quartiers decouverts et occupes, sans limitation aux alliances, visibles dans le menu Pause.
- [x] Positions de quartiers conservees dans LSLife_TerritoryMap.xml et restaurees apres Reload() (validation utilisateur).
- [ ] Tester le rafraichissement de la couleur et du nom lors d'un changement de proprietaire, ainsi que le nettoyage des blips au Reload().
- [ ] Valider en jeu les rectangles normalises (fusion des superpositions et des bandes contigues) : pas de doubles teintes, aucune perte de surface, transparence 48, lisibilite et performances dans les zones complexes.
- [ ] Verifier le display 3 sur carte Pause (aucune surface ni marqueur sur la minimap), la transparence, la lisibilite des icones GTA, le plafond de 250 rectangles, les mises a jour de proprietaire et le nettoyage Reload().
- [ ] Prevoir une option de desactivation des surfaces si le rendu pose probleme, sans desactiver les marqueurs.
- [ ] Tester les noms de quartiers lisibles et la capture de position des nouvelles zones des leur enregistrement.
- [ ] Tester la sauvegarde atomique (.tmp/.bak), la restauration automatique depuis un .bak valide si le XML principal est corrompu, et la relance des ecritures apres erreur.
- [ ] Tester le filtrage des coordonnees invalides et des zones maritimes, puis la mise a jour des proprietaires toutes les 10 secondes.
- [ ] Comparer les limites XY importees depuis zones.json aux frontieres visibles en jeu, notamment les zones qui se touchent et les quartiers complexes ; ne pas confondre points de decouverte et contours.
- [ ] Etudier une legende de couleurs par gang, la visibilite uniquement dans le menu Pause et les performances avec de nombreuses zones.
- [ ] Verifier la compatibilite avec les autres mods de carte et les zones sans proprietaire.

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
