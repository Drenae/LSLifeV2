# Règles de développement — LS Life V2

## 1. Objectif non négociable

Remettre en état LS Life pour GTA V Legacy : **fonctionnement, bugs, traduction française, conversion métrique et personnalisation légère**. Ne pas transformer le projet en réécriture complète.

## 2. Socle technique à conserver

- **GTA V Legacy**, mode histoire.
- **ScriptHookVDotNet2**.
- **NativeUI 1.9.1 d'origine**.
- **iFruitAddon2**.
- Projet C# / .NET Framework 4.8 / x64.

Ne pas migrer vers SHVDN3, LemonUI, ni créer LSNativeUI sans nouvelle demande explicite.

## 3. Protection de la stabilité

- `main` représente la version fonctionnelle de référence.
- Ne jamais supprimer ou remplacer une fonctionnalité opérationnelle sans accord.
- Ne jamais faire de reset destructeur, réécrire l'historique ou modifier les sauvegardes du joueur sans autorisation.
- Compiler après les changements et distinguer **compilation réussie** de **fonctionnement vérifié en jeu**.
- Prévoir un retour arrière simple pour toute modification risquée.

## 4. Vérifier avant de coder

- Consulter directement le dépôt GitHub et les fichiers concernés.
- Ne pas inventer d'API, de signatures, de chemins ni de fonctionnalités.
- Vérifier les usages et dépendances avant de renommer ou supprimer.
- Si une information n'est pas vérifiée, le dire explicitement.

## 5. Changements ciblés et cohérents

- Préférer une mise à jour substantielle et cohérente à une succession de micro-corrections, **sans sacrifier les tests**.
- Ne pas toucher à des systèmes non concernés.
- Préserver les comportements, événements et sauvegardes existants sauf décision explicite.
- Éviter les dépendances nouvelles inutiles.

## 6. Conversion métrique

- Employer des unités cohérentes dans l'interface et la logique.
- Vérifier les conversions, calculs et sérialisations avant de modifier des valeurs.
- Ne pas conserver d'anciennes conventions arbitraires uniquement par héritage si elles contredisent le nouveau système, mais préserver les données tant qu'une migration sûre n'est pas établie.

## 7. Traduction

- Traduire les textes destinés au joueur, pas les identifiants du code par simple souci esthétique.
- Employer un français naturel, cohérent avec le contexte du jeu.
- Tester l'affichage et la longueur des libellés.

## 8. Communication et Git

- Répondre en français, directement, avec peu de blabla.
- Annoncer clairement ce qui a **réellement** été modifié et ce qui reste à tester.
- Fournir les commandes PowerShell regroupées dans un seul bloc, en fin de réponse quand elles sont nécessaires.
- Travailler sur le dépôt réel et éviter de demander des fichiers déjà accessibles.
- Ne pas créer de branche supplémentaire sans besoin ni accord.
- Ne pas supprimer les branches distantes sans demande explicite.
