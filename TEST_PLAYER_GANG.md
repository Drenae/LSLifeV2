# Validation en jeu — lot 1

## Installation

Sur `feature/player-gang`, compiler avec `Build-Baseline.ps1`, puis installer la nouvelle `bin/Release/net48/LSlife.dll` dans le dossier `scripts` du jeu ferme. Conserver les sauvegardes existantes. Aucun nouveau XML a installer : le fichier du gang est cree au premier enregistrement valide.

## Parcours principal

1. Charger une sauvegarde existante : inventaire, grammes, employes, argent et planques inchanges.
2. Appuyer sur **F9** : « Mon gang », aucun gang cree. Le raccourci se configure dans `LSlife.ini`, section `[KEYS]`, `GANG=F9` (cle facultative).
3. « Creer mon gang » → « Nom du gang » : saisir le nom au clavier GTA. Tester annulation, nom vide et nom valide. Un nom invalide ne doit pas creer de gang.
4. Choisir une couleur avec gauche/droite ou Entree. Annuler : aucun gang ni fichier ne doit etre cree.
5. Recommencer puis « Creer le gang » : notification de succes, joueur chef, identite et ID disponibles. Le menu ne propose plus de seconde creation.
6. Consulter l'ID complet, puis modifier nom et couleur. Verifier l'ID inchange et l'existence de `LSLife_PlayerGang.xml.bak` apres une modification effective.
7. Quitter et relancer le jeu / recharger les scripts : meme ID, nom, couleur et chef, sans recréer le gang.
8. Appeler Zee → « Mon gang » : meme menu. Retour ramene chez Zee. F9 permet aussi l'acces sans appel ni batiment ; F9 ferme le menu autonome.
9. Tester clavier et manette, maintien Entree/B lors des transitions, retour, pause et mort : pas de double activation ni de menu empile. F10 conserve l'inventaire.
10. Verifier qu'aucun argent n'apparait, qu'aucun territoire n'est attribue et que les recrutements existants fonctionnent toujours.

## Protection XML (sur une copie de sauvegarde, jeu ferme)

- Sauvegarder `LSLife_PlayerGang.xml` et son `.bak` avant ces essais.
- Rendre le XML invalide puis relancer : le mod doit continuer, le menu signale la lecture impossible ; creation/edition bloquees, fichier intact.
- Remettre le XML valide, puis « Recharger le fichier XML » : retrouver le gang et le meme ID.
- XML actif absent mais `.bak` present : pas de nouveau gang automatique ; remettre manuellement un XML valide a son emplacement actif, puis recharger.
- Le remplacement manuel d'un XML doit se faire jeu ferme ; la detection de modification externe empeche une edition d'identite d'ecraser un fichier change depuis sa lecture.

## Retour attendu

Indiquer compilation reussie ou joindre `build-baseline.log` en cas d'erreur ; confirmer creation, rechargement, edition et navigation. Le lot 2 commence apres validation en jeu du lot 1.
