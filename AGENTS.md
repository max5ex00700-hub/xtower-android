# X MATCH repository instructions

These rules apply to coding agents and contributors.

## Architecture

1. Keep match-3 game rules in `Assets/XMatch/Core`.
2. Code in `XMatch.Core` must not reference `UnityEngine`.
3. Unity-facing animation, input, audio, and scene code belong outside Core.
4. Prefer deterministic APIs for board simulation so automated balancing can be added later.
5. Do not put generated Unity folders such as Library, Temp, Logs, Obj, Builds, or UserSettings into Git.

## Gameplay rules

- Board coordinates use `(0, 0)` as the bottom-left cell.
- A legal player swap must be orthogonally adjacent and create a match involving at least one swapped tile.
- Empty cells use `TileKind.Empty`.
- Normal tile identities are gameplay data. Visual art must not be hard-coded into core rules.

## Product rules

- X MATCH is an original game. Do not copy another game's protected artwork, UI, names, characters, or level content.
- Romance characters are explicitly adults.
- The first priority is readability, responsiveness, and a satisfying puzzle loop.

## Change discipline

- Keep `main` stable.
- Develop on `develop` or a feature branch.
- For core-rule changes, add or update tests when a Unity test project is available.
- Keep commits focused and explain behavior changes in the PR.
- Never commit credentials, API keys, signing files, or store secrets.
