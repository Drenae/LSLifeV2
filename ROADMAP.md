## Lot Worker — alertes et découplage (25 octobre 2026)

- [x] Sauvegarde de sécurité : `backup/pre-worker-alerts-20261025` au commit `177d7d2`.
- [x] Alertes de stock Worker : cannabis, crack, cocaïne, et message prioritaire si tous les stocks sont vides.
- [x] Détection des transitions, anti-répétition jusqu'au réapprovisionnement, délai global entre notifications.
- [x] Alerte de recette nette à récolter, seuil configurable, réarmement après collecte.
- [x] Configuration `[WORKER_ALERTS]` documentée dans README ; notifications GTA avec son SMS, sans boîte de réception persistante.
- [x] Collecte de recette depuis CustomWorkerMenu sans callback NativeUI.
- [ ] **À compiler et tester en jeu** : ventes simulées, stock épuisé, réapprovisionnement, collecte, cooldown, sauvegardes, iFruit et menus.
- [ ] Suite du découplage NativeUI : sélecteurs et ajouts de commandes Zee, équipements Worker, services Zee.

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
