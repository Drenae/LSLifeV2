## Lot Tick / interactions — 28 octobre 2026

- [x] Le traitement des six menus personnalisés dans `Tick.cs` n'est plus conditionné par l'existence de `LsMenuPool`.
- [x] Les vérifications d'ouverture de menu du Tick (HUD DEBUG, aide, marchand d'armes, client, interactions) passent par `CustomMenuShared.IsAnyGameplayMenuOpen()`, qui tient compte des six menus CustomMenu et du pool historique.
- [x] Import direct `using NativeUI;` retiré de `Tick.cs` ; aucune modification des sauvegardes XML ou de l'initialisation iFruit.
- [ ] `LsMenuPool.ProcessMenus()` et NativeUI.dll restent nécessaires pour les consommateurs historiques ; ne pas les supprimer avant leur migration.
- [ ] Compilation Windows et test GTA à effectuer : démarrage à froid/téléphone Zee, interactions client/marchand, HUD DEBUG, fermeture des menus et sauvegardes.

## Correctif Tick — 28 octobre 2026

- [x] Interaction marchand d'armes : vérification de `LsMenuPool` avant son accès et suppression du double test `IsAnyMenuOpen()`.
- [ ] La migration NativeUI de `Tick.cs` reste à effectuer ; ce correctif ne supprime aucune dépendance.
- [ ] Compilation Windows et test en jeu non effectués pour ce correctif.

# Migration NativeUI — état au 28 octobre 2026

**Branche de travail :** `feature/custom-zee-menu` ; **référence stable :** `main`. Ne pas supprimer `NativeUI.dll` avant compilation et vérification des derniers consommateurs.

## Dernier lot livré

- [x] `Setup.cs` : initialisation des anciens menus déplacée dans `LSlife.Core/LegacyMenuCompatibility.cs`, et imports directs NativeUI retirés.
- [x] `LSL.cs` : anciens champs NativeUI déplacés dans la classe partielle de compatibilité.
- [x] Correction des erreurs `UIText` / `GTA.UI` dans la couche de compatibilité (commit `6bdc9ef`).
- [x] `Setup.cs` : correction de migration des sauvegardes d'inventaire ; ajout indépendant des champs manquants `weedOz`, `crackOz`, `cocaineOz`, `armour`, et écriture unique après migration ; rechargement des quantités sans doublon de clés (commit `25d8f4e`).
- [ ] **Compilation Windows du dernier lot non confirmée** ; les erreurs éventuelles doivent être corrigées avant test GTA.
- [ ] **NativeUI.dll toujours indispensable** : l'extraction des anciens champs n'est pas une suppression de dépendance.

## Travaux restants, par priorité

1. [ ] Identifier et migrer tous les appels historiques encore actifs dans `Tick.cs` et les gestionnaires de planques, véhicules, dealer, employé, options et interactions.
2. [ ] Remplacer les accès aux `UIMenu*` et aux sliders/listes NativeUI par les états et services des menus personnalisés ; préserver les sauvegardes et les callbacks iFruit.
3. [ ] Supprimer les initialisations et événements devenus inutiles dans `LegacyMenuCompatibility.cs`, puis supprimer ce fichier quand il n'a plus de consommateurs.
4. [ ] Retirer `LsMenuPool.ProcessMenus()` / `IsAnyMenuOpen()` et centraliser l'ouverture, fermeture et navigation des menus personnalisés.
5. [ ] Retirer la référence NativeUI du `.csproj` et du déploiement ; compiler et tester à froid GTA V Legacy (téléphone Zee, argent, menus, employés, planques, véhicules, armes, sauvegardes).
6. [ ] Nettoyer les avertissements et les anciennes traces de décompilation après stabilisation, sans confondre avertissements SHVDN2 et erreurs bloquantes.

## Procédure de test

```powershell
cd E:\LSLife
git pull origin feature/custom-zee-menu
.\Build-Baseline.ps1
```

Ne lancer GTA qu'après une compilation réussie. Éviter de demander plusieurs redémarrages pour des changements purement internes.

---

## Lot isolé 01 — affichage Employé (26 octobre 2026)

- [x] Ajout de deux lignes **Instruction actuelle** et **Recette brute** dans le menu principal Employé uniquement (`CustomWorkerMenu.cs`).
- [x] Aucune modification de `Tick`, des contrôles, des transferts, de la sauvegarde ou d'iFruit.
- [ ] À valider en jeu : téléphone/contact Zee après démarrage complet, puis ouverture du menu Employé et vérification des deux nouvelles informations.
- [ ] En cas de panne du téléphone, revert **uniquement** le commit `20d33c2` et examiner l'effet de cette modification visuelle isolée.

## Analyse différentielle de la régression téléphone — 26 octobre 2026

État de référence : commit `c8e1a5b` (téléphone confirmé opérationnel après démarrage complet). Le retour des deux fichiers au contenu de cet état, dans `45e8859` et `c6df9ed`, a également été confirmé fonctionnel.

**Commits suspects examinés :**

- `bb70c20` — `CustomWorkerMenu.cs` : ajout d'un ID de session du dealer, resets du contrôleur horizontal, deux appels à `GTA.UI.ShowSubtitle`, lignes d'information supplémentaires.
- `087372f` — `CustomWorkerMenu.cs` : vérifications `((GTA.Entity)ped).Exists()`, validation d'index d'arme, affichage du total de transfert.
- `b6f89bb` — `CustomStashMenu.cs` : resets horizontaux et garde sur l'existence de la planque/coffre dans `TickOncePerFrame()`.

**Constats vérifiés dans les diffs :** ces commits ne changent ni `Setup.cs`, ni `Tick.cs`, ni `iFruitAddon2`. Les gardes supplémentaires se trouvent dans des chemins de menus qui commencent par `if (!Visible) return` ; le mécanisme exact de la panne au lancement n'est donc **pas identifié**. Le retour fonctionnel après revert établit la corrélation avec le lot, pas une ligne coupable.

**Protocole de reprise :**
1. Ne pas réintroduire le lot suspect tel quel.
2. Préparer d'abord des modifications purement visuelles et sans nouvelle API GTA dans un seul menu ; éviter les changements d'initialisation, de Tick global et de contrôles tant que l'origine n'est pas prouvée.
3. Compiler, fermer GTA entièrement, redémarrer et vérifier le téléphone/contact Zee **avant** d'ouvrir les menus.
4. En cas de régression, conserver le log `ScriptHookVDotNet.log` du démarrage et examiner les exceptions de chargement/initialisation.
5. Vérifier également la version de DLL réellement déployée et l'absence de copies en double dans `scripts`.

## Régression téléphone après lot Employé/Stockage — 26 octobre 2026

- [x] Retour de `CustomWorkerMenu.cs` et `CustomStashMenu.cs` à l'état `c8e1a5b` (dernier état avec téléphone confirmé fonctionnel par l'utilisateur).
- [x] Correction d'initialisation différée d'iFruit dans `Setup.cs` et `Tick.cs` conservée intacte.
- [ ] Vérifier **après fermeture complète et redémarrage** de GTA V ; ne pas se limiter à `Reload()`.
- [ ] Si la régression subsiste, analyser `ScriptHookVDotNet.log` du démarrage concerné avant toute nouvelle modification.
- [ ] Réintroduire les améliorations des menus progressivement après identification de la cause, sans modifier iFruitAddon2.

## Lot de stabilisation des menus personnalisés — 26 octobre 2026

- [x] Menu employé : verrouillage de la session sur l'identifiant du dealer sélectionné ; fermeture si le dealer change ou si son PNJ disparaît.
- [x] Menu employé : réinitialisation des contrôles à l'ouverture, au retour et entre sous-menus ; prévention des répétitions horizontales parasites.
- [x] Menu employé : affichage de l'identifiant, de l'instruction, de la recette brute, de la recette nette, du total à transférer et du multiplicateur.
- [x] Menu employé : confirmation visuelle de l'approvisionnement et de la collecte, vérification de l'existence du PNJ et de l'index d'arme avant transfert.
- [x] Menus planque/coffre : fermeture de l'interface si le stockage actif disparaît et remise à zéro de la répétition au changement de contexte.
- [x] iFruit : **aucune modification** des fichiers `Setup.cs`, `Tick.cs`, de l'initialisation différée du téléphone ou des contacts ; NativeUI.dll conservée.
- [ ] Compiler, puis tester en une session complète : téléphone à froid, contact Zee, HUD/argent, commandes, recrutement, vente client, employé (approvisionner, récolter, équiper, instructions), planque et coffre.
- [ ] Poursuivre la migration NativeUI uniquement après validation du lot ; ne pas retirer la DLL tant que les appels historiques existent.

## Priorité — iFruit au démarrage complet (26 octobre 2026)

- [x] Audit de la chaîne d'appel : `LSL()` → `Setup()` → création `CustomiFruit` / contact Zee ; `onTick()` → test `Game.IsLoading` → `iFruit.Update()`.
- [x] Problème d'ordre identifié : `Setup()` pouvait créer le téléphone avant la fin du chargement, tandis que `Reload()` intervient sur un jeu déjà chargé.
- [x] Initialisation déplacée vers `onTick()` après `Game.IsLoading` et disponibilité du personnage ; création unique et mise à jour protégée.
- [x] Aucun changement aux callbacks des contacts, à iFruitAddon2.dll, à l'argent, au HUD ou aux commandes.
- [ ] **Validation indispensable : fermer complètement GTA V, redémarrer, vérifier téléphone du protagoniste, contact Zee, argent HUD et paiement ; répéter un Reload() pour comparer.**
- [ ] Si le téléphone manque encore : récupérer `ScriptHookVDotNet.log` et identifier l'exception au démarrage (la cause exacte reste à confirmer en jeu).

## Lot groupé — recrutement et clients (26 octobre 2026)

- [x] Recrutement : acceptation, refus, paiement et annulation accessibles par méthodes métier sans paramètres NativeUI ; menu personnalisé branché sur ces méthodes.
- [x] Clients : vente et refus déplacés dans un traitement sans menu ni élément NativeUI ; le menu personnalisé n'ouvre plus artificiellement `customerMainMenu`.
- [x] Compatibilité : callbacks NativeUI historiques conservés comme adaptateurs.
- [x] Protection : aucun changement à iFruitAddon2, `Game.Player.Money`, au HUD financier ni aux commandes Zee validées.
- [ ] **À compiler et tester** : recrutement (annulation, refus, acceptation, débit), clients (vente, refus, véhicule, client éloigné), sauvegardes, téléphone et argent.
- [ ] **NativeUI.dll reste nécessaire** : équipement, planques, véhicules et initialisations historiques à migrer avant retrait final.

## Stabilisation Zee et iFruit — 26 octobre 2026

- [x] Retour aux traitements de commande Zee précédemment validés après régression téléphone/argent (restaurés dans `CustomZeeMenu.cs` et `DealingMenus.cs`).
- [x] Ajout du maintien gauche/droite dans les sous-menus Cannabis, Crack et Cocaïne, pour quantités et multiplicateurs (délai initial 320 ms, répétition 65 ms).
- [x] Réinitialisation de la répétition lors des changements de menus, sélection et retour, pour éviter les mouvements parasites.
- [x] Aucune modification de `iFruitAddon2`, du traitement iFruit dans `Tick`, de `Game.Player.Money`, ni des paiements des livreurs dans ce lot.
- [ ] Compilation et test en jeu : téléphone et contacts, argent HUD, commandes et livraisons, appui prolongé gauche/droite sur les trois produits, retour menus.
- [ ] Dépendances NativeUI restantes : à retirer uniquement après migration fonctionnelle complète et validation.

## Lot commandes Zee — 26 octobre 2026

- [x] Migration des trois commandes Cannabis / Crack / Cocaïne : quantités, pas de sélection, prix, réservation de stock et validation via traitement commun indépendant des sliders NativeUI.
- [x] Adaptateurs des anciens menus NativeUI conservés pour les autres appels historiques.
- [x] Suppression des références directes aux sliders NativeUI dans `CustomZeeMenu`.
- [ ] **À compiler et tester en jeu** : quantités et multiplicateurs des trois produits, prix et total, stock fournisseur, plusieurs ajouts, validation et livraison, sauvegarde et téléphone.
- [ ] Suppression de la DLL NativeUI : **non effectuée** ; autres systèmes historiques toujours dépendants.

## Nettoyage Zee — 26 octobre 2026

- [x] Sauvegarde `backup/pre-zee-order-decoupling-20261026`.
- [x] Suppression du pont de retour vers les anciens sous-menus NativeUI inutilisé dans `CustomZeeMenu`.
- [ ] Extraction de l'état des quantités et de la validation des commandes : **non effectuée** ; les contrôles NativeUI restent nécessaires.
- [ ] Compiler et vérifier la navigation Zee, les sous-menus et le téléphone après nettoyage.

## Migration Zee — services (26 octobre 2026)

- [x] Sauvegarde : `backup/pre-zee-services-decoupling-20261026`.
- [x] Zee : actions marchand d'armes, chauffeur et remboursement extraites des callbacks NativeUI.
- [x] Zee : menu personnalisé appelle directement ces actions, sans rendre `zeeMenu` visible artificiellement.
- [x] Callback NativeUI historique conservé en adaptateur temporaire.
- [ ] **À compiler et tester** : marchand d'armes (appel / renvoi), chauffeur (appel / annulation), remboursement partiel / total, contacts et menus.
- [ ] Zee : commandes de drogue toujours synchronisées aux sliders NativeUI ; extraire l'état et les actions métier avant suppression.
- [ ] Supprimer les définitions et dépendances NativeUI uniquement après migration des autres fonctionnalités (planques, véhicules, menus historiques).

## Lot Worker — alertes et découplage (25 octobre 2026)

- [x] Sauvegarde de sécurité : `backup/pre-worker-alerts-20261025` au commit `177d7d2`.
- [x] Alertes de stock Worker : cannabis, crack, cocaïne, et message prioritaire si tous les stocks sont vides.
- [x] Détection des transitions, anti-répétition jusqu'au réapprovisionnement, délai global entre notifications.
- [x] Alerte de recette nette à récolter, seuil configurable, réarmement après collecte.
- [x] Configuration `[WORKER_ALERTS]` documentée dans README ; notifications GTA avec son SMS, sans boîte de réception persistante.
- [x] Collecte de recette depuis CustomWorkerMenu sans callback NativeUI.
- [ ] **À compiler et tester en jeu** : ventes simulées, stock épuisé, réapprovisionnement, collecte, cooldown, sauvegardes, iFruit et menus.
- [x] Menu Worker : transfert d'armes appelé par index, sans dépendance au sélecteur NativeUI (ancien callback conservé pour compatibilité).
- [ ] Menu Zee : commandes de drogues et services encore reliés aux contrôles/callbacks NativeUI ; migration non terminée.
- [ ] Suppression complète de NativeUI : impossible tant que Setup.cs, LSL.cs et les anciens menus/callbacks le référencent.
- [ ] Tester le transfert d'armes Worker, les munitions, composants, sauvegardes et l'inventaire après migration.

# Roadmap — LS Life V2

> Branche de migration : `feature/custom-zee-menu`. `main` reste la référence stable. Ne déclarer une fonctionnalité terminée qu'après validation en jeu.

## État vérifié — 25 octobre 2026

> Mise à jour d'après le code de la branche et les retours de tests en jeu. « Fonctionnel en jeu » ne signifie pas « indépendant de NativeUI ».

- **Téléphone iFruit** : apparence et contacts validés après réorganisation de l'ordre de traitement dans `Tick.cs`.
- **Menus personnalisés** : Zee, employés, planque, clients, marchand d'armes et recrutement possèdent des interfaces CustomMenu ; plusieurs actions utilisent encore les objets ou événements NativeUI en arrière-plan.
- **Marchand d'armes** : achats et notifications validés en jeu ; disposition à améliorer.
- **Recrutement** : ouverture, offre acceptée, paiement et suivi du dealer validés en jeu. Repérage et contournement de réputation réservés à DEBUG.
- **HUD** : `HudRender.cs` utilise désormais les natives GTA V pour le HUD principal, les pressions et les panneaux DEBUG ; `CustomHudOptionsMenu.cs` remplace les réglages NativeUI. **Rendu, positionnement, maintien des touches et DEBUG validés en jeu par l'utilisateur.**
- **Dépendance NativeUI** : encore requise pour les menus historiques et des composants internes ; le HUD principal n'en dépend plus pour son rendu. Ne pas supprimer la DLL avant élimination des références.
- **Tests** : HUD et options confirmés fonctionnels en jeu par l'utilisateur ; audit des dépendances restantes dans `AUDIT_NATIVEUI.md`.

## Sauvegarde et migration NativeUI — 25 octobre 2026

- [x] Sauvegarde Git avant travaux : `backup/pre-nativeui-decoupling-20261025` (`bf9e174`).
- [x] Extraire deux actions métier du menu Zee sans modifier les stocks ni les sauvegardes.
- [ ] Compiler et tester en jeu : activer/désactiver le mode vente, valider une commande, vérifier le retour des menus et le téléphone.
- [ ] Découpler les sliders, la sélection de produits et les services Zee de NativeUI.

## Retours HUD — réglages et lisibilité (25 octobre 2026)

- [x] Structurer les options HUD : taille écran, stock, influence et pression, DEBUG conditionnel, enregistrer et retour.
- [x] Afficher les options DEBUG seulement lorsque AREADEBUG est activé dans l'état courant du mod.
- [x] Ajuster les coordonnées au pixel près, avec répétition automatique au maintien gauche/droite.
- [x] Masquer les panneaux DEBUG lorsque les menus personnalisés sont ouverts pour éviter la superposition.
- [x] Compiler et tester en jeu les nouveaux réglages, l'alignement des panneaux, le maintien des touches et la réapparition du DEBUG après fermeture (retour utilisateur : parfait).

## Correctifs HUD — 25 octobre 2026

- [x] Corriger la ligne Zee « Aucune dette en cours » : elle doit dépendre de la ligne de dette, et non du dernier élément du menu.
- [x] Ajouter trois groupes de positions indépendants : stocks, barres de pression et panneaux DEBUG, avec sauvegarde XML (`heatPosX/Y`, `debugPosX/Y`) et compatibilité des anciennes coordonnées.
- [x] Vérifier en jeu les réglages et le menu Zee corrigé (retour utilisateur positif ; persistance après redémarrage à recontrôler séparément).

## Priorité immédiate — Migrer réellement le HUD

- [x] Auditer les fonctions d'affichage du HUD principal, les barres de pression, les panneaux DEBUG, les paramètres de position et leurs sauvegardes.
- [x] Remplacer le rendu du HUD principal, des barres de pression et des quatre panneaux DEBUG par `HudRender.cs` utilisant les natives GTA V. **Affichage vérifié en jeu**. Le `BarHandler` historique utilise encore `TimerBarPool` : vérifier ses appels avant de le retirer.
- [x] Migrer les réglages de position vers `CustomHudOptionsMenu` accessible depuis Zee, sans `UIMenu*` dans `Hud.cs` ; sauvegarde XML et compatibilité `hudPosVersion` conservées. **Interface vérifiée en jeu ; nouveaux réglages à retester**.
- [ ] Garder les emplacements actuels : argent/stocks/mode vente en haut à droite ; pression joueur/quartier en bas à droite. Conserver les valeurs et conditions d'affichage historiques.
- [ ] Conserver le thème sombre avec accents dorés de la maquette approuvée ; mode compact, réputation et capacité des dealers restent des **options futures**, non activées par défaut.
- [ ] Vérifier la compilation, les affichages avec/sans menu, les différentes résolutions, le téléphone iFruit et les modes DEBUG ; obtenir validation en jeu.
- [ ] **Ne pas supprimer NativeUI.dll** : d'autres systèmes en dépendent encore.

## P0 — Stabilité et correctifs

- [x] Retrouver une base fonctionnelle.
- [x] Valider les nouveaux panneaux à hauteur automatique et les séparateurs à marges réelles.
- [x] Harmoniser les boutons sélectionnés et inactifs.
- [ ] **Corriger Échap clavier** : revenir au menu précédent sans ouvrir le menu pause de GTA V. Vérifier les contrôles désactivés et leur ordre d'exécution, sans casser la manette.
- [ ] Compiler et tester en jeu chaque étape ; vérifier les logs et les régressions.

## P1 — Migration NativeUI → CustomMenu (C#)

Objectif : menus et sous-menus autonomes, rendus via `CustomMenuRenderer` et `CustomMenuLayout`. Préserver les traitements métier existants. Supprimer NativeUI uniquement après audit complet de ses références.

- [x] **Zee** : menus et sous-menus personnalisés validés visuellement.
- [x] **Worker / Employé(e)s** : menu principal.
- [x] **Worker / Approvisionner**.
- [x] **Worker / Équipement**.
- [x] **Worker / Instruction** : sélection gauche/droite dans le menu principal (Ne rien faire / Me suivre / Travailler), validée en jeu ; conserver la fermeture historique lors du passage au travail.
- [ ] **Stash / Planque (maison)** : menu principal CustomMenu raccordé à l'interaction en jeu (à compiler et valider) ; retrait et dépôt restent provisoirement NativeUI. Source de vérité : `LSL.HouseHandler.CurrentHouse` (Weed/Crack/Cocain/Money/Armor/Weapons), sauvegarde de maison. Ne jamais mélanger avec le coffre de voiture.\n- [ ] **Stash / Coffre de voiture** : menu principal CustomMenu raccordé à l'interaction en jeu (à compiler et valider) ; retrait et dépôt restent provisoirement NativeUI. Auditer son propre inventaire, ses limites et sa sauvegarde avant modification. Ne jamais utiliser `CurrentHouse` pour le coffre.\n- [ ] **Stash / Vérification** : tester les transferts dans les deux sens, les quantités limites et la persistance de chaque stockage séparément.
- [x] **Customer / Clients** : interface personnalisée et interactions raccordées ; découplage NativeUI interne restant.
- [x] **Audit statique NativeUI** : inventaire des fichiers et ordre de migration documentés dans `AUDIT_NATIVEUI.md`.
- [ ] **Découplage métier** : début effectué sur Zee : démarrage du mode vente (`StartDealing`) et validation de commande (`PlaceDrugOrder`) ne nécessitent plus d'objets `UIMenu*` depuis CustomZeeMenu. **À compiler et tester en jeu** ; sliders, ajout de produits, services et autres callbacks restent à migrer.
- [ ] **Audit final après migration** : vérifier l'absence totale de références NativeUI avant suppression de la DLL.
- [ ] Supprimer les références et la dépendance NativeUI seulement lorsque plus aucun code ne l'utilise.

## P2 — Améliorations après migration

- [ ] **Alertes de réapprovisionnement Worker** : SMS de l’employé lorsque Cannabis, Crack ou Cocaïne est épuisé ; message spécifique lorsque tout le stock est vide. Prévoir anti-spam, seuils et déclenchement uniquement lors d’un changement de stock.\n- [ ] **Alerte recette Worker** : SMS lorsque la somme à récolter dépasse un seuil configurable ; éviter les répétitions jusqu’à la récolte.\n- [ ] **Équipement Worker** : donner une nouvelle arme et récupérer automatiquement l'ancienne ; gérer inventaire, munitions, composants, sauvegarde et cas limites.
- [x] **HUD** : rendu et options personnalisés, validés en jeu ; surveiller les régressions lors du découplage final de NativeUI.
- [ ] **HUD / Session** : afficher les bénéfices de la session actuelle, avec une définition claire des revenus et dépenses comptabilisés.
- [ ] **Livre des comptes** (étude de faisabilité) : historique des transactions, bénéfices, dépenses et synthèses par session ; évaluer persistance et performances avant développement.

## P3 — Traduction française et fiabilisation

- [ ] Compléter la traduction des textes visibles, notifications et instructions.
- [ ] Vérifier les libellés des armes, équipements et actions.
- [ ] Corriger les bugs reproduits, sans modifier inutilement le gameplay.
- [ ] Vérifier les sauvegardes et les parcours principaux.

## P4 — Conversion métrique finale

- [ ] Auditer les unités de stockage, d'affichage et de calcul.
- [ ] Planifier la conversion complète et la compatibilité des sauvegardes.
- [ ] Tester les stocks, ventes, achats, commandes et transferts.

**Conversion métrique à traiter en dernier**, conformément à la décision du projet.

## Méthode

1. **Lire ROADMAP.md et REGLES.md au début de chaque intervention**, puis le code réel de la branche ; mettre à jour la roadmap dès qu'une étape change.
2. Migrer un ensemble cohérent de menus sans modifier les traitements métier non concernés.
3. Centraliser les éléments de rendu réutilisables.
4. Compiler, tester dans GTA V et corriger les régressions.
5. Valider chaque menu avant de passer au suivant.
