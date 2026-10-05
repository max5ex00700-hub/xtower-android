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

### Validation

- [x] Unity 6000.3.24f1 cloud compile / Android build
- [x] Android install and on-device play
- [x] touch input smoke test
- [ ] portrait scaling on several aspect ratios
- [ ] frame-time / allocation profiling

## v0.03 - Prototype campaign and special-block pass

- [x] 10 deterministic prototype levels
- [x] level selector
- [x] per-level goals / move limits / seeds
- [x] 4-match line blast
- [x] 5-match color orb
- [x] T/L bomb
- [x] 2 x 2 seeker
- [x] special-block chain reactions
- [x] unlimited test boosters: Hammer / Row / Column / Shuffle
- [x] first visual-polish pass with rounded glossy tiles and stage themes
- [ ] serialized level asset format
- [ ] visual board layout editor
- [ ] obstacle hooks
- [ ] validation report / one-click play test

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
