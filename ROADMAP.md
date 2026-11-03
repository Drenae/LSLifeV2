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
- [ ] Verifier en jeu le contact permanent « Reseau Gangs » dans iFruit apres demarrage et Reload(), meme sans gang, et son ouverture du menu de creation ; verifier la coexistence avec Zee et les chefs debloques.
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

### Gestion du gang et economie — a valider
- [ ] Tester la page Gestion de mon gang : rang, tresorerie, membres, alliances et territoires, puis le retour dans le menu GANGS.
- [ ] Tester la liste paginee des quartiers possedes (aucun, 1, plus de 5), la navigation et la conservation apres Reload().
- [ ] Tester depot de 5 000 $ depuis l'argent du joueur, recrutement de membres pour 10 000 $ depuis la caisse, plafond de 100 membres et fonds insuffisants.
- [ ] Tester les revenus de 250 $ par territoire toutes les 10 minutes de jeu, sans revenu automatique lors du Reload().
- [ ] Tester la sortie volontaire d'un gang, la confirmation, la conservation des territoires/accords du gang quitte et le refus pendant une prise de territoire.
- [ ] Etudier la sauvegarde transactionnelle du depot pour eviter toute perte d'argent en cas d'echec disque, et la persistance des delais de conquete.
- [ ] Verifier la protection des pactes de non-agression et les pertes de confiance (-15) et de reputation (-10) lors d'une conquete.

### Conquete et gestion du territoire du joueur
- [ ] Tester en jeu le menu GANGS > Controle des territoires, quartier courant, proprietaire et compteur de zones.
- [ ] Tester la premiere conquete a pied (90 secondes dans le quartier), l'annulation en sortant / en vehicule / en mourant, la protection des allies et la restriction au chef.
- [ ] Tester le transfert du proprietaire dans les zones en memoire et LSLife_GangDiplomacy.xml, puis Reload() ; aucun territoire attribue automatiquement a la fondation.
- [ ] Ajouter une conquete armee immersive avec defenseurs, objectifs, riposte, risque de guerre, cooldown et controle de zone ; l'occupation actuelle est une premiere phase, pas une guerre complete.
- [ ] Prevoir le cas des organisations creees puis quittees, ainsi que la gestion des rangs et des zones neutres.

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

### Appartenance du joueur a un gang
- [ ] Tester la creation volontaire de « Mon organisation » dans le reseau GANGS, sa confirmation et sa persistance apres Reload().
- [ ] Tester l'adhesion volontaire a un gang existant face au chef (reputation 60, rencontre physique, confirmation, rang Recrue) et la persistance XML.
- [ ] Prevoir la personnalisation du nom de l'organisation et un vrai systeme de recrutement, de progression de rang et de sortie de gang.

### Accords diplomatiques progressifs
- [ ] Tester en jeu le pacte de non-agression (+10 confiance), l'accord commercial (+10) et le soutien arme (+5), avec confirmations physiques et persistance XML apres Reload().
- [ ] Tester le chemin 50 -> 60 -> 70 -> 75 puis l'alliance, ainsi que la rupture d'un accord (-10 confiance), les refus et l'absence de bonus en boucle.
- [ ] Ajouter des missions de cooperation, des consequences concretes des accords (commerce, soutien, non-agression), et des reactions de chefs avant de finaliser la diplomatie.

### Rendez-vous et negociations en personne
- [ ] PRIORITE ACTIVE : construire le menu de negociation physique du chef, distinct du telephone, puis les demandes d'alliance et leurs conditions.
- [ ] Tester les rendez-vous demandes par telephone : apparition du chef distinct, marqueur temporaire, expiration (5 min DEBUG / 10 min normal) et nettoyage au Reload().
- [ ] Tester la premiere rencontre physique du chef et la persistance XML de metLeaders apres Reload(), sans attribuer de faux nom au chef ; ameliorer ensuite les lieux de rendez-vous pour les rendre propres a chaque gang.
- [ ] Tester en jeu le menu physique de negociations du chef (CustomMenu, clavier/manette), uniquement pendant le rendez-vous, avec depart du chef apres fermeture et nettoyage Reload().
- [ ] Tester la demande d'alliance en personne : reputation >=60, gang du joueur requis, confiance diplomatique >=75, aucun accord entre gangs tiers ; verifier persistance XML et statut Allie apres Reload().
- [ ] Tester la confirmation explicite d'alliance et d'adhesion, les refus et le maintien du menu en face du chef ; ajouter ensuite les reactions du chef et d'autres accords diplomatiques.
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

### Carte des territoires — test opt-in OpenIV (en pause, apres alliances)
- [ ] Verifier en jeu la fusion horizontale ET verticale des rectangles IPL : contours, jonctions, nombre de blips et performance sur plusieurs quartiers voisins. `ADD_BLIP_FOR_AREA` reste rectangulaire ; un contour polygonal continu necessiterait un autre moteur de rendu.
- [ ] Confirmer les traces `IPL DIRECT` / `IPL ALIAS` / `IPL ZONE INTROUVABLE` dans les logs ; aucune surface ne doit provenir des anciennes limites pendant ce test.
- [ ] Tester TERRITORYMAPTEST=true avec scripts\\LSLife\\popzone.ipl (fichier OpenIV de 91 Ko), verifier les surfaces des quartiers visites sur la carte pause, les limites et les couleurs ; verifier les alias de codes GTA/ipl et la limite des blips.
- [ ] Verifier la compilation et le comportement apres Reload(), sans modifier les sauvegardes diplomatiques.
- [ ] Conserver le rendu des territoires desactive par defaut tant que le test n'est pas valide.

### Carte des territoires — en pause
- [ ] Reprendre ulterieurement l'affichage des points et des surfaces colorees de tous les gangs decouverts, sans restriction aux alliances.
- [ ] Revalider les limites des quartiers, les superpositions, le rendu sur carte Pause et les changements de proprietaire.
- [ ] Reactiver explicitement le chargement et le tick de GangTerritoryMap apres validation ; conserver les donnees et les fichiers XML existants.

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
