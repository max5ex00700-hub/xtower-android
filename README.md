# X MATCH

X MATCH is an original mobile match-3 character puzzle game.

## Current state

The repository now contains:

- **v0.01 puzzle core:** complete in source
- **v0.02 Unity playable-board prototype:** implemented in source
- **Unity compile / Play Mode / device validation:** still pending

The prototype is designed to open in Unity and immediately provide an 8 x 9 playable board with placeholder visuals.

## Product direction

Core loop:

1. Play a short match-3 stage.
2. Earn progression rewards.
3. Unlock character, relationship, outfit, location, or story progress.
4. Return to the next puzzle with a clear narrative reward ahead.

All featured romance characters are adults.

## Technical direction

- **Runtime / presentation:** Unity
- **Pinned editor:** Unity 6000.3.24f1
- **Core game rules:** pure C# in `XMatch.Core`
- **Presentation:** `XMatch.Puzzle`
- **Goal:** keep match-3 simulation independent from Unity so it can be tested, simulated, and reused.
- **Repository strategy:** `main` is stable, `develop` is active development.

## Implemented puzzle flow

```text
swipe
  -> validate swap
  -> consume move only on a valid match
  -> clear
  -> gravity
  -> refill
  -> cascades
  -> goal progress
  -> win / lose
  -> dead-board detection
  -> deterministic shuffle when needed
```

## Unity prototype

Implemented in source:

- portrait layout
- 8 x 9 board
- touch swipe
- mouse drag for editor testing
- valid and invalid swap animation
- clear animation
- falling and refill animation
- cascade / combo feedback
- moves HUD
- collection goals HUD
- win / fail state
- dead-board shuffle
- replay

See `docs/UNITY_PROTOTYPE.md`.

## Repository layout

```text
Assets/XMatch/
  Core/        Pure C# puzzle rules and simulation
  Puzzle/      Unity board presentation and input
  Character/   Character / affection / outfit systems
  Story/       Dialogue and episode systems
  UI/          Screens and HUD
  Editor/      Setup and future level-authoring tools

ProjectSettings/
Packages/
docs/
```

## Next milestone

v0.03 focuses on production tooling:

1. serialized level assets
2. visual level editor
3. board painter
4. goal / move-limit authoring
5. validation
6. 10 prototype levels

Before expanding content, the v0.02 prototype must be opened in Unity 6000.3.24f1 and verified on-device.
