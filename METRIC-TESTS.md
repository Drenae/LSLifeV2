# LS Life V2 — Conversion métrique native (réinitialisation requise)

**IMPORTANT :** Cette branche modifie les unités **stockées**, pas seulement l'affichage. Les anciens fichiers XML utilisant `weedOz`, `crackOz`, `cocaineOz` ne doivent pas être réutilisés comme sauvegardes de progression. Fermer GTA V, sauvegarder puis réinitialiser les fichiers de progression avant de lancer le mod.

## Modifications

- Stocks de produits non préparés en grammes : `pWeedGrams`, `pCrackGrams`, `pCocaineGrams`.
- Stocks du véhicule en grammes : `drugCarWeedGrams`, `drugCarCrackGrams`, `drugCarCocainGrams`.
- Nouvelles clés XML : `weedGrams`, `crackGrams`, `cocaineGrams` (inventaire) ; `cocaineGrams` également pour le véhicule.
- Commandes du fournisseur : curseurs en grammes, sans lots de 28 g ; multiplicateurs 1, 5, 10, 25, 50, 100, 250, 500, 1000.
- Prix : calcul proportionnel au gramme à partir des tarifs historiques (prix affiché pour 100 g).
- Livraison, véhicule, planque et inventaire : quantités en grammes, avec affichage g/kg.
- Réapprovisionnement du fournisseur : volumes adaptés aux grammes.
- NativeUI 1.9.1 inchangée.

## Tests en jeu (à effectuer après compilation)

| Test | Action | Résultat attendu |
|---|---|---|
| Démarrage | Nouvelle sauvegarde, ouvrir inventaire | Aucun crash, 0 g |
| Commande 1 g | Acheter 1 g de cannabis | Facturation et livraison de 1 g |
| Commande 25 g | Acheter 25 g de crack | Stock augmenté de 25 g |
| Commande 100 g | Acheter 100 g de cocaïne | Prix proportionnel et livraison de 100 g |
| Commande 1 kg | Acheter 1000 g | Affichage 1 kg |
| Commande mixte | Commander 1 g + 25 g + 100 g | Facture = somme des lignes |
| Stock fournisseur | Commander puis annuler | Stock restitué intégralement |
| Véhicule | Déposer/reprendre 37 g non préparés | Exactement 37 g transférés |
| Planque | Déposer 53 g non préparés | Exactement 53 g ajoutés |
| Vente | Vendre puis contrôler l'inventaire | Décompte en grammes |
| Sauvegarde | Sauvegarder, quitter, relancer | Stocks identiques |
| XML | Inspecter les clés de stock | Clés *Grams et valeurs en grammes |
| Réapprovisionnement | Attendre le réassort du fournisseur | Stock positif et prix cohérents |
| Affichage | Parcourir menus | Pas d'oz ni de lot 28 g visible |

**Compilation et validation en jeu non réalisées par l'assistant.** Ne pas écraser les sauvegardes ni la DLL fonctionnelle sans copie de sécurité.
