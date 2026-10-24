# Roadmap — LS Life V2

> Branche de migration : `feature/custom-zee-menu`. `main` reste la référence stable. Ne déclarer une fonctionnalité terminée qu'après validation en jeu.

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
- [ ] **Worker / Instruction** : sélection gauche/droite directement dans le menu principal (Ne rien faire / Me suivre / Travailler (vendre)), sans états techniques ; confirmation visuelle ajoutée. Compilation et validation en jeu à effectuer.
- [ ] **Stash / Planque** : inventorier les actions et migrer.
- [ ] **Customer / Clients** : inventorier les actions et migrer.
- [ ] **Audit final** : recenser tous les autres menus, dialogues, sélecteurs et dépendances NativeUI ; migrer les éléments restants.
- [ ] Supprimer les références et la dépendance NativeUI seulement lorsque plus aucun code ne l'utilise.

## P2 — Améliorations après migration

- [ ] **Équipement Worker** : donner une nouvelle arme et récupérer automatiquement l'ancienne ; gérer inventaire, munitions, composants, sauvegarde et cas limites.
- [ ] **HUD** : refonte visuelle cohérente avec les menus CustomMenu.
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

1. Lire le code réel de la branche avant modification.
2. Migrer un ensemble cohérent de menus sans modifier les traitements métier non concernés.
3. Centraliser les éléments de rendu réutilisables.
4. Compiler, tester dans GTA V et corriger les régressions.
5. Valider chaque menu avant de passer au suivant.
