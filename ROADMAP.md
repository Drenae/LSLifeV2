# Roadmap — LS Life V2

> Branche de migration : `feature/custom-zee-menu`. `main` reste la référence stable. Ne déclarer une fonctionnalité terminée qu'après validation en jeu.

## État vérifié — 25 octobre 2026

> Mise à jour d'après le code de la branche et les retours de tests en jeu. « Fonctionnel en jeu » ne signifie pas « indépendant de NativeUI ».

- **Téléphone iFruit** : apparence et contacts validés après réorganisation de l'ordre de traitement dans `Tick.cs`.
- **Menus personnalisés** : Zee, employés, planque, clients, marchand d'armes et recrutement possèdent des interfaces CustomMenu ; plusieurs actions utilisent encore les objets ou événements NativeUI en arrière-plan.
- **Marchand d'armes** : achats et notifications validés en jeu ; disposition à améliorer.
- **Recrutement** : ouverture, offre acceptée, paiement et suivi du dealer validés en jeu. Repérage et contournement de réputation réservés à DEBUG.
- **HUD** : nouvelle apparence anthracite/dorée introduite dans `Hud.cs`, **non encore validée en jeu et toujours dessinée avec NativeUI**. Ce n'est pas une migration technique.
- **Dépendance NativeUI** : encore requise pour les menus, le HUD et des composants internes. Ne pas supprimer la DLL avant élimination des références.
- **Compilations** : les changements récents du HUD n'ont pas été confirmés compilés ; tester avant de marquer terminés.

## Priorité immédiate — Migrer réellement le HUD

- [ ] Auditer toutes les fonctions d'affichage du HUD principal, les barres de pression, les panneaux DEBUG, les paramètres de position et leurs sauvegardes.
- [ ] Remplacer **tous** les dessins NativeUI du HUD (`UIResText`, `UIResRectangle`, `UIContainer`, `UIText`, `TimerBarPool` selon les usages réels) par le rendu natif GTA V / composants personnalisés, sans perte de données ni de fonctionnalités.
- [ ] Migrer les réglages de position du HUD qui dépendent de `UIMenu*` ; conserver la compatibilité des positions sauvegardées.
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
- [ ] **Audit final** : recenser tous les autres menus, dialogues, sélecteurs et dépendances NativeUI ; migrer les éléments restants.
- [ ] Supprimer les références et la dépendance NativeUI seulement lorsque plus aucun code ne l'utilise.

## P2 — Améliorations après migration

- [ ] **Alertes de réapprovisionnement Worker** : SMS de l’employé lorsque Cannabis, Crack ou Cocaïne est épuisé ; message spécifique lorsque tout le stock est vide. Prévoir anti-spam, seuils et déclenchement uniquement lors d’un changement de stock.\n- [ ] **Alerte recette Worker** : SMS lorsque la somme à récolter dépasse un seuil configurable ; éviter les répétitions jusqu’à la récolte.\n- [ ] **Équipement Worker** : donner une nouvelle arme et récupérer automatiquement l'ancienne ; gérer inventaire, munitions, composants, sauvegarde et cas limites.
- [ ] **HUD** : apparence initiale implémentée, **non validée** ; migration technique NativeUI vers rendu GTA natif prioritaire (voir ci-dessus).
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
