# X MATCH - Architecture

## 1. Design goal

Use Unity for rendering and mobile product integration while keeping the puzzle simulation in a Unity-independent C# layer.

```text
Unity Presentation
  UI / Input / Animation / Audio / VFX
              |
              v
        XMatch.Core
 Board / Match / Stage / Goals / Levels
              |
              v
       Save / Content Data
```

## 2. Module plan

### XMatch.Core

Pure C#. No `UnityEngine` dependency.

Responsibilities:

- board state
- tile identities
- legal swaps
- match detection
- clearing
- gravity
- refill
- cascade resolution
- legal-move / dead-board detection
- level definitions
- move limits
- goals and progress
- stage win / loss state
- deterministic simulation hooks

The core mutates the logical board and returns animation-ready records. Unity animates those records instead of recalculating puzzle outcomes.

### XMatch.Puzzle

Unity-facing board presentation.

Responsibilities:

- tile GameObjects / UI elements
- swipe and tap input
- animation sequencing
- VFX and SFX
- mapping logical tiles to visuals
- displaying dead-board shuffle when requested by the core

### XMatch.Character

Responsibilities:

- adult character identity
- affection
- expression state
- outfit state
- unlock conditions

### XMatch.Story

Responsibilities:

- episodes
- dialogue nodes
- conditions
- rewards
- story progression

### XMatch.UI

Responsibilities:

- home
- puzzle HUD
- win / fail
- character screen
- episode screen
- collection
- shop later

### XMatch.Editor

Responsibilities:

- level editor
- board painter
- goal authoring
- validation
- play-test shortcut
- later: batch simulation

## 3. Core coordinate convention

Board coordinates use bottom-left origin.

```text
y
^
|
|  (0,2) (1,2)
|  (0,1) (1,1)
|  (0,0) (1,0) -> x
```

Gravity moves tiles toward lower Y.

`LevelDefinition.InitialCells` is flat row-major data starting with the bottom row:

```text
index = y * width + x
```

## 4. Player move resolution

```text
StageSession.TryMove
  |
  +-- MoveResolver.TryResolve
  |     |
  |     +-- SwapLogic.TrySwap
  |     |     +-- invalid -> rollback / no move consumed
  |     |
  |     +-- CascadeResolver
  |           +-- clear
  |           +-- gravity
  |           +-- refill
  |           +-- match scan
  |           +-- repeat until stable
  |
  +-- decrement move counter
  +-- apply cleared tile identities to goals
  +-- evaluate win / loss
  +-- BoardMoveFinder checks whether shuffle is required
```

A `CascadeStep` records:

- cleared tile kind + board position
- tile movements caused by gravity
- newly spawned tiles
- chain number

This is the contract between puzzle logic and future Unity animation code.

## 5. Level validation

`LevelValidator` currently rejects:

- initial boards containing automatic matches
- initial boards with no legal swap

Future validation will also cover obstacles, spawn constraints, and goal feasibility.

## 6. Determinism

`SeededTileSource` uses an engine-owned deterministic xorshift generator instead of Unity or platform random APIs. Given the same seed and the same board operations, refill output is reproducible across simulation runs.

This supports:

- automated balancing
- replay/debug traces
- reproducible bug reports

## 7. Dependency rule

Allowed:

```text
Puzzle -> Core
Editor -> Core
UI -> Puzzle / Character / Story
```

Not allowed:

```text
Core -> UnityEngine
Core -> UI
Core -> Character art
```

## 8. Future simulator

The same `XMatch.Core` rules can run without a Unity scene. This enables future automated level simulation for:

- estimated clear rate
- average moves used
- dead-board frequency
- special-piece creation frequency
- bottleneck detection

The simulator is a strategic tool, not an MVP blocker.
