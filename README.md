# X MATCH

X MATCH is an original mobile match-3 character puzzle game.

## Current state

The repository now contains:

- **v0.01 puzzle core:** complete in source
- **v0.02 Unity playable-board prototype:** verified through Android build and on-device play
- **v0.03 prototype campaign:** 10 playable stages, special blocks, unlimited test boosters, and first visual-polish pass

The prototype now opens into a 10-stage selector. Stages use 7 x 8, 8 x 8, and 8 x 9 boards with distinct goals, move limits, deterministic seeds, special-block creation, and unlimited test boosters.

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
- 10-stage selector
- 7 x 8 / 8 x 8 / 8 x 9 boards
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
- 4-match line blasts
- 5-match color orb
- T/L bomb
- 2 x 2 seeker
- special-block chain reactions
- unlimited Hammer / Row / Column / Shuffle test boosters
- rounded glossy runtime tile styling and per-stage color themes
- replay / next-level flow

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

v0.04 focuses on the character/meta vertical slice:

1. first adult heroine
2. affection progression
3. short story episode
4. clear reward
5. character/outfit unlock presentation
6. stronger VFX/audio pass for special blocks
