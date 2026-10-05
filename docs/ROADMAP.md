# X MATCH - Development Roadmap

## v0.01 - Puzzle core foundation

### Scope

- [x] Repository bootstrap
- [x] Core module boundary
- [x] Board data model
- [x] Five normal tile identities
- [x] Horizontal / vertical 3+ match detection
- [x] Adjacent swap validation
- [x] Clear matched cells
- [x] Gravity
- [x] Refill
- [x] Cascade loop
- [x] Dead-board detection
- [x] Move counter
- [x] Goal model
- [x] Level data model
- [x] deterministic stable-board generator
- [x] deterministic dead-board shuffler

## v0.02 - First Unity playable board

### Implemented in source

- [x] Unity project version / package scaffolding
- [x] automatic prototype-scene setup
- [x] runtime bootstrap
- [x] 8 x 9 board rendered with generated placeholder tiles
- [x] touch / swipe input
- [x] mouse drag input for editor testing
- [x] valid swap animation
- [x] invalid swap return animation
- [x] clear animation
- [x] falling animation
- [x] refill animation
- [x] cascade timing
- [x] combo banner
- [x] move counter HUD
- [x] goal HUD
- [x] win / fail state
- [x] automatic dead-board shuffle presentation
- [x] replay button

### Validation still required

- [ ] open with Unity 6000.3.24f1 and complete compile check
- [ ] run prototype in Unity Play Mode
- [ ] test touch input on Android device
- [ ] test portrait scaling on several aspect ratios
- [ ] profile basic frame time and allocations

The v0.02 source implementation is present, but the milestone is not considered verified until the Unity/editor/device checks pass.

## v0.03 - Level authoring

- [ ] serialized level asset format
- [ ] board layout editor
- [ ] move-limit editor
- [ ] goal editor
- [ ] obstacle hooks
- [ ] validation report
- [ ] one-click play test
- [ ] 10 prototype levels

## v0.04 - Character meta slice

- [ ] one adult heroine
- [ ] character screen
- [ ] affection meter
- [ ] two expressions
- [ ] story dialogue
- [ ] clear reward
- [ ] one unlockable visual state

## v0.05 - Vertical slice polish

- [ ] final-quality feedback pass
- [ ] audio
- [ ] VFX
- [ ] onboarding
- [ ] first-session pacing
- [ ] analytics event plan
- [ ] device performance pass

## Deferred until the core is proven

- clans
- PvP
- season pass
- large economy
- card collection
- heavy live-ops
- complex store
