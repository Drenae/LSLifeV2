# Audit NativeUI — branche `feature/custom-zee-menu`

Date : 25 octobre 2026. Audit statique du code GitHub, **sans compilation ni test GTA V**.

## Conclusion

**Ne pas supprimer `NativeUI.dll` maintenant.** Le HUD est autonome pour son dessin, mais le mod dépend encore de NativeUI pour la création de menus hérités, les données des contrôles, leurs événements et la gestion du `MenuPool`. Les menus personnalisés visibles ne prouvent pas l'indépendance technique.

## Points d'entrée et dépendances vérifiés

| Fichier | Dépendance | Migration nécessaire |
| --- | --- | --- |
| `LSlife.csproj` | Référence à `$(GtaVPath)\scripts\NativeUI.dll` | Supprimer **en dernier** |
| `Lslife/LSL.cs` | `MenuPool` et nombreux champs `UIMenu`, `UIMenuItem`, `UIMenuSliderItem`, `UIMenuListItem`, `UIMenuCheckboxItem` | Remplacer les états UI par des données métier et des modèles CustomMenu |
| `LSlife.Core/Setup.cs` | Création et branchement des menus NativeUI, dont Zee, commandes, recrutement, employés, planques, véhicules | Retirer seulement après migration des événements et des parcours |
| `LSlife.Core/Tick.cs` | `LsMenuPool.ProcessMenus()` et tests `IsAnyMenuOpen()` | Centraliser l'état ouvert de tous les CustomMenu |
| `LSlife.UI/CustomZeeMenu.cs` | Réutilisation de sliders, cases à cocher et handlers NativeUI ; ouverture ponctuelle de menus historiques | **Priorité haute** : isoler les états de commandes/vente et les actions métier |
| `LSlife.UI/CustomWorkerMenu.cs` | Garde `LsMenuPool.IsAnyMenuOpen()` | Utiliser un gestionnaire d'état de menus commun |
| `LSlife.UI/MenuEvents.cs` | Signatures `UIMenu*` pour commandes et équipement | Extraire les opérations métier des callbacks |
| `LSlife.UI/Menus/DealerMenus.cs` | Signatures `UIMenu*` pour Zee, armes et recrutement | Supprimer les adaptateurs après migration des appels |
| `LSlife.UI/Menus/DealingMenus.cs` | Signatures `UIMenu*`, fermeture `LsMenuPool` | Extraire les commandes et démarrage des ventes |
| `LSlife.UI/Menus/PlayerMenus.cs` | Signature `UIMenu*` | Migrer les actions restantes |
| `LSlife.UI/Menus/StashMenus.cs` | Signatures `UIMenu*` et sliders | Vérifier les transferts maison et véhicule indépendamment |
| `LSlife.UI/Menus/VehicleMenus.cs` | Signatures `UIMenu*` | Migrer dépôt/retrait coffre sans confusion avec maison |
| `LSlife.UI/Menus/WorkerMenus.cs` | Signatures `UIMenu*` et sélecteurs | Extraire les actions métier des contrôles |
| `LSlife.UI/HeatBars/BarHandler.cs` | `TimerBarPool`, `BarTimerBar` | Vérifier les références avant suppression ; HUD principal rendu par `HudRender` |
| `LSlife.UI/LsMenus.cs` | `using NativeUI` résiduel | Nettoyer après vérification de la classe partielle |

`AreaHeatBar.cs` et `PlayerHeatBar.cs` calculent des pourcentages sans utiliser NativeUI. `Hud.cs` et `HudRender.cs` sont désormais conçus pour un rendu GTA natif.

## Ordre de migration recommandé

1. **Découplage métier** : créer des méthodes sans paramètres `UIMenu*` pour les actions réutilisées par les menus personnalisés. Garder temporairement des adaptateurs NativeUI si nécessaire.
2. **Zee** : retirer les dépendances aux sliders, checkboxes et événements NativeUI dans la commande de drogue, les ventes et les services.
3. **Planques / véhicules / employés / recrutement / clients / armes** : vérifier puis retirer les anciens objets et callbacks sans modifier les règles métier ni la persistance.
4. **Gestion centrale** : remplacer `MenuPool`, `ProcessMenus()`, `IsAnyMenuOpen()` par l'état réel des menus personnalisés, en vérifiant le téléphone iFruit et les contrôles clavier/manette.
5. **Dernier nettoyage** : supprimer les anciens champs et initialisations, vérifier `BarHandler`, retirer `using NativeUI`, puis retirer la référence `NativeUI.dll` du projet.
6. **Validation finale** : compilation .NET Framework 4.8 / SHVDN2, ouverture/fermeture de chaque menu, interactions, achats, transferts, sauvegardes, HUD, DEBUG, téléphone iFruit et logs en jeu.

## Garde-fous

- Aucun changement de sauvegarde, de prix, de gameplay ou d'unité pendant cette migration.
- `main` reste intacte ; travail sur `feature/custom-zee-menu`.
- Ne jamais présenter un menu *visuellement personnalisé* comme *techniquement indépendant* de NativeUI sans audit de ses appels.
- Ne retirer la DLL qu'après compilation sans référence NativeUI et validation fonctionnelle.
