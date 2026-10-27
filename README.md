# Zoo World

Test task: a top-down 3D scene where animals spawn every 1–2 seconds, move around, collide by physics and eat each other according to a food chain.

- **Unity:** 6000.3.24f1 (URP)
- **DI:** [Extenject (Zenject)](https://github.com/svermeulen/Extenject), installed from OpenUPM (registry already set in `Packages/manifest.json`)
- **Scene:** `Assets/Scenes/ZooWorld.unity`

- **`AnimalDefinition`** (ScriptableObject) — data of a species: name, food chain level, prefab.
- **`Animal`** — state of one animal (alive/dead) and events `Died` / `Ate`. Knows nothing about movement or food.
- **Movement** — separate components: `JumpMovement` (frog), `ContinuousMovement` (snake). They use `IArenaBounds` to turn back before leaving the screen.
- **Food chain** — `IFoodChainRule` decides who eats whom. The default `FoodChainLevelRule` compares levels: higher eats lower, prey (level 0) ignore each other, predators of the same level fight and one survives.
- **`ZooInstaller`** — composition root, all services are bound here.

## Adding a new animal

1. Create a prefab with `Animal`, a `Rigidbody`, a `Collider`, `AnimalCollision` and a movement component. Add `TastyFeedback` if it can eat.
2. If it moves in a new way (flying, swimming…), write a new movement component. Nothing else changes.
3. Create an `AnimalDefinition` (*Create → Zoo World → Animal Definition*), set its prefab and food chain level (frog = 0, snake = 10).
4. Add the definition to the `species` list of `AnimalSpawner`.

No existing code has to be edited. A different food chain (diets, omnivores…) is a new `IFoodChainRule` implementation bound in `ZooInstaller`.
