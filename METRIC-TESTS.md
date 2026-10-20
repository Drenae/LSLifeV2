# LS Life V2 — Validation de la conversion métrique

Cette mise à jour **ne modifie pas le format des sauvegardes**. Les clés historiques `weedOz`, `crackOz`, `cocaineOz` et les valeurs des curseurs de paquets restent inchangées : **1 paquet = 28 g**. Les quantités déjà exprimées en grammes restent des grammes. NativeUI 1.9.1 n'est pas modifiée.

## Avant de tester

1. Fermer GTA V.
2. Sauvegarder le dossier `scripts/LSLife` (en particulier les fichiers XML).
3. Garder la DLL NativeUI 1.9.1 actuellement fonctionnelle.
4. Exécuter `git pull origin main` puis `.\Verify-Build.ps1` dans `E:\LSLife`.
5. Installer la DLL LS Life compilée **uniquement si la compilation réussit**.

## Tests en jeu — à effectuer dans cet ordre

| # | Zone | Action | Résultat attendu |
|---|---|---|---|
| 1 | Démarrage | Charger une sauvegarde existante | Aucune erreur ; stocks inchangés |
| 2 | Inventaire | Vérifier cannabis, crack, cocaïne | Quantités en g ou kg ; pas d'oz visible |
| 3 | Commande Rafael | Choisir 1 paquet de chaque produit | Chaque paquet correspond à 28 g ; prix inchangé |
| 4 | Commande Rafael | Choisir 10 paquets | Affichage 280 g ; total financier inchangé |
| 5 | Commande Rafael | Choisir 36 paquets | Affichage 1 kg 8 g |
| 6 | Livraison | Réceptionner la commande | Stocks augmentés des bonnes quantités |
| 7 | Véhicule | Déposer puis reprendre des produits non préparés | Même quantité avant/après ; affichage g/kg |
| 8 | Véhicule | Déposer puis reprendre des grammes préparés | Même quantité avant/après ; affichage g/kg |
| 9 | Planque | Déposer des produits non préparés | +28 g par paquet dans la planque |
| 10 | Planque | Déposer puis reprendre des grammes | Conservation exacte des quantités |
| 11 | Vente | Vendre aux clients | Quantité vendue et reste affichés en g/kg |
| 12 | Redémarrage | Sauvegarder, quitter, relancer | Stocks et sauvegardes conservés |
| 13 | Interface | Parcourir tous les menus | Aucun libellé d'once visible ; aucun plantage |
| 14 | Limites | Tester stocks nuls, quantité proche de 1 kg | 0 g, 999 g, 1 kg, 1 kg 1 g correctement affichés |

## Périmètre technique

- `LsFunctions.FormatMetric` est la fonction commune pour les quantités déjà en grammes.
- `LsFunctions.LegacyPacksToGrams` centralise la conversion des anciens paquets.
- `LsFunctions.FormatLegacyPacks` affiche ces paquets en g/kg.
- L'ancienne méthode `GramsToOz` reste disponible comme alias de compatibilité.
- Les clés XML et les champs internes contenant `Oz` sont conservés volontairement pour éviter une migration de sauvegarde risquée.
- Aucun changement de NativeUI ou LemonUI n'est inclus.

## Remontée de bugs

Pour chaque anomalie, relever le menu, la quantité avant/après, la valeur attendue et une capture si l'affichage est incorrect.
