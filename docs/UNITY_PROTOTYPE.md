# X MATCH - Unity Prototype v0.02

## Target editor

The repository is pinned to Unity `6000.3.24f1`.

## What happens on first open

The editor script `XMatchProjectSetup` will:

1. set product name to X MATCH
2. set portrait orientation
3. create `Assets/Scenes/XMatchPrototype.unity` if it does not exist
4. add that scene to Build Settings
5. open the prototype scene

Press Play.

`XMatchRuntimeBootstrap` creates the camera and playable puzzle automatically.

## Prototype controls

### Phone / tablet

Swipe a tile up, down, left, or right.

### Editor

Click-drag with the mouse in the same four directions.

## Implemented presentation

- 8 x 9 board
- five placeholder tile styles
- valid / invalid swap animation
- match clear animation
- gravity animation
- spawn animation
- cascade timing
- combo banner
- moves HUD
- collection-goal HUD
- win / fail overlay
- automatic no-move shuffle presentation
- replay button

## Important limitation

This repository was authored remotely and has not yet been opened by a real Unity editor in this environment.

Therefore:

- source structure has been checked
- runtime logic has been integrated
- **Unity 6000.3.24f1 compile validation is still pending**
- **Android/iOS device validation is still pending**

Do not treat v0.02 as release-ready until those checks pass.

## Next visual pass

The current board intentionally uses generated placeholder blocks. Production art should replace them after interaction and game-rule validation, without changing `XMatch.Core`.
