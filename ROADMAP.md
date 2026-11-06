# ROADMAP — LS Life V2 : organisation du joueur

**Branche active :** `feature/player-gang` (créée depuis `feature/stabilization-metric-ui`).  
**Cible :** GTA V Legacy solo, SHVDN2, .NET Framework 4.8 x64, iFruitAddon2, menus CustomMenu.  
**Historique :** voir `ROADMAP_ARCHIVE.md`. Les anciennes pistes SHVDN3, NativeUI et Réseau Gangs ne sont pas des tâches actives.

## 1. État de référence validé par l'utilisateur

- [x] Stabilisation générale de LS Life V2 et fiabilisation des sauvegardes XML.
- [x] Conversion métrique en grammes et kilogrammes.
- [x] Modernisation des menus compacts et améliorations des fonctions existantes.
- [x] Deux méthodes actuelles de recrutement des dealers conservées : proposition d'un client et débauchage d'un dealer rival.
- [x] Choix de principe : territoire contrôlé par la réputation locale, et perdu si la réputation baisse suffisamment.
- [x] Aucun revenu automatique ou passif lié à la possession d'un territoire.
- [x] Aucun MLO ou QG requis pour créer et gérer le gang.

Les trois premiers jalons sont déclarés terminés par l'utilisateur. Ne pas les relancer sans bug constaté. Les nouvelles fonctionnalités ne sont **pas encore implémentées** sur cette branche.

## 2. Règles fonctionnelles décidées

1. Le joueur peut créer **son propre gang** : identité, nom, données persistantes et gestion depuis un menu CustomMenu compact.
2. La **réputation de chaque quartier** est le mécanisme de progression et de contrôle territorial ; réutiliser `Area.Reputation` et la logique de zones existante, sans modifier les spawns natifs.
3. Un quartier peut devenir propriété du gang quand sa réputation atteint un seuil ; il redevient libre lorsque la réputation descend sous un seuil inférieur. Prévoir une marge entre les deux seuils pour éviter les changements d'état incessants.
4. Les dealers doivent travailler réellement pour soutenir la réputation : leur simple présence ne garantit pas le maintien du territoire. Examiner la réputation accordée par les ventes de dealers et la décroissance avant de fixer le rythme.
5. **Recrutement : uniquement les deux mécanismes existants.** Pas d'achat de dealers ni de recrutement direct arbitraire de PNJ.
6. Les territoires **ne rapportent jamais directement de l'argent**. Les revenus proviennent des ventes réelles. Collecter les recettes d'un dealer est un transfert, pas un nouveau revenu.
7. Une **trésorerie du gang** doit être distincte de l'argent personnel et des recettes en attente chez les dealers. Éviter toute création/duplication d'argent.
8. Ne pas reprendre automatiquement le code des anciennes branches de réseau, conquête ou alliances de gangs.

## 3. Paramètres proposés — non validés définitivement

- Acquisition d'un quartier : **niveau de réputation 10** (environ **62 500 points**).
- Perte d'un quartier : réputation **sous le niveau 8** (environ **40 000 points**).
- Les seuils doivent être configurables, sauvegardés ou dérivés de façon stable.
- La **décroissance de réputation**, ses conditions et sa vitesse sont **à réfléchir plus tard** ; ne pas inventer un équilibrage définitif dans le premier lot.
- Modalités exactes de la trésorerie (dépôt/retrait manuel, affectation des dépenses, éventuels versements) à préciser au fil du développement.

### Vérifications techniques déjà réalisées

- `Area.Reputation` est un `float` enregistré sous `aRep` dans `LSLifeAreas.xml`.
- `LsFunctions.CurrentRepLvl(rep)` calcule approximativement `0.04 * sqrt(rep)` ; aucun plafond général de niveau n'a été identifié dans les classes inspectées.
- `AreaHeatBar` affiche la progression vers le prochain niveau.
- Vérifier **dans tous les chemins de vente** comment les ventes du joueur et celles des employés modifient la réputation du quartier avant d'ajouter le contrôle territorial.

## 4. Lots de développement

### Lot 1 — Fondations, création et persistance du gang (priorité immédiate)

- [ ] Auditer `Area`, `AreaLogic`, `Gang`, les dealers, les menus et les services XML.
- [ ] Concevoir un modèle de gang du joueur **distinct des gangs natifs**, avec ID stable, nom, identité et état sauvegardé.
- [ ] Ajouter création/chargement/sauvegarde non destructifs, validation des données et compatibilité avec les sauvegardes existantes.
- [ ] Ajouter un menu CustomMenu compact : créer le gang, consulter son identité et accéder à sa gestion.
- [ ] Préparer les références de membres et de territoires sans dupliquer les données métier.
- [ ] Compiler puis faire valider en jeu par l'utilisateur.

### Lot 2 — Territoires fondés sur la réputation

- [ ] Inventorier les quartiers réellement gérés et leurs identifiants persistants.
- [ ] Auditer la réputation gagnée par les ventes des dealers, sans supposer qu'elle fonctionne déjà comme celle des ventes du joueur.
- [ ] Implémenter attribution/perte d'un territoire avec seuils distincts et transitions sauvegardées.
- [ ] Afficher les quartiers libres, contrôlés et leur progression dans le menu du gang.
- [ ] Ne générer **aucun revenu territorial passif**.
- [ ] Reporter le réglage précis de la décroissance de réputation à une décision ultérieure.
- [ ] Tester franchissement de seuil, rechargement, vente, changement de zone et absence de dealer.

### Lot 3 — Intégration des dealers existants

- [ ] Rattacher au gang les dealers recrutés via les **deux méthodes existantes seulement**.
- [ ] Préserver stocks, argent, armes, affectations et identités des dealers.
- [ ] Gérer départs, décès, sauvegardes et rechargements sans duplication.
- [ ] Prévoir une gestion depuis le menu du gang sans remplacer les menus employés actuels.
- [ ] Valider les effets réels des ventes des dealers sur la réputation locale.

### Lot 4 — Trésorerie

- [ ] Concevoir un solde de gang distinct de l'argent du joueur et des recettes des dealers.
- [ ] Ajouter dépôts, retraits et historique minimal selon les règles approuvées.
- [ ] Garantir qu'aucune opération ne crée ou ne perd silencieusement de l'argent ; sauvegardes XML sûres.
- [ ] Conserver les ventes comme **seule source de revenus liée au trafic** : aucun paiement pour la simple possession de terrain.

### Lot 5 — Évolutions ultérieures, sur décision

- [ ] Ajuster la décroissance de réputation et les conditions de conservation des quartiers.
- [ ] Définir les éventuels rôles, membres non-dealers, présence à Paleto Bay et fonctions supplémentaires du gang.
- [ ] Intégrer le bâtiment choisi **seulement après modification et validation du MLO dans Blender** : d'abord une StashHouse classique par téléportation, avec ID XML stable.
- [ ] **Tout à la fin**, transformer cette StashHouse en QG avec fonctions de gestion du gang.

## 5. Contraintes de livraison

- Développer en **gros lots cohérents**, pas en micro-commits.
- Lire `REGLES.md` et cette roadmap avant chaque lot, puis mettre à jour l'avancement réel.
- Ne pas changer SHVDN2, ne pas réintroduire NativeUI, ne pas fusionner les anciennes branches de gangs.
- Préserver toutes les sauvegardes existantes et la conversion métrique.
- Distinguer audit, code poussé, compilation et validation effective en jeu.
- Aucun point de téléportation, coordonnée ou interaction MLO fictif avant disponibilité du bâtiment.
