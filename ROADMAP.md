# Roadmap — LS Life V2

Ce document décrit les **priorités**, pas une liste de fonctionnalités déjà livrées. Vérifier le code et les tests en jeu avant de déclarer une tâche terminée.

## P0 — Préserver la version fonctionnelle

- [x] Retrouver une base stable compilable et fonctionnelle en jeu.
- [x] Conserver `main` comme référence stable.
- [ ] Vérifier la compilation après chaque modification.
- [ ] Tester en jeu les fonctions touchées et contrôler les logs/erreurs.

## P1 — Traduction française

- [ ] Recenser les textes visibles en jeu : menus, notifications, descriptions, aides et dialogues.
- [ ] Traduire de manière cohérente, sans modifier les identifiants techniques ni la logique.
- [ ] Vérifier accents, longueurs de texte et affichage dans NativeUI.
- [ ] Tester les principaux parcours en jeu.

## P2 — Corrections de bugs

- [ ] Relever les bugs réellement reproduits.
- [ ] Corriger par priorité : crashs, blocages, pertes de données, logique de gameplay, puis affichage.
- [ ] Pour chaque correction, documenter le symptôme, la cause vérifiée et le test de non-régression.
- [ ] Éviter les refactorisations sans lien avec le problème.

## P3 — Personnalisation légère

- [ ] Améliorer les menus existants avec **NativeUI 1.9.1**, sans créer de bibliothèque alternative.
- [ ] Ajuster les propriétés accessibles (position, largeur, bannières, présentation) si utile.
- [ ] Évaluer les améliorations de confort de jeu une par une.

## P4 — Finaliser la conversion métrique

- [ ] Auditer le code actuel pour distinguer les conversions déjà réalisées de celles restant à faire.
- [ ] Uniformiser les unités affichées, calculées et sauvegardées lorsque nécessaire.
- [ ] Vérifier achats, ventes, stocks, commandes, quantités et inventaires.
- [ ] Tester avec une nouvelle sauvegarde du mod, puis documenter les éventuelles incompatibilités.
- [ ] Supprimer les anciennes unités uniquement après validation de leur remplacement.

**À traiter en dernier**, après traduction, corrections de bugs et personnalisation légère.

## Hors périmètre

- Migration SHVDN3 / LemonUI.
- Fork, recompilation ou remplacement de NativeUI.
- Réécriture complète du mod.
- Ajout massif de fonctionnalités sans rapport avec la remise en état.

## Méthode

1. Lire le code réel de `main`.
2. Définir un ensemble cohérent de corrections.
3. Modifier uniquement les fichiers nécessaires.
4. Compiler et tester.
5. Mettre à jour cette roadmap après validation.
