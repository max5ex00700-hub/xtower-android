# XMatch.Puzzle

Unity-facing match-3 presentation layer.

## Implemented prototype

- runtime bootstrap
- 8 x 9 board renderer
- generated placeholder tile visuals
- touch swipe and editor mouse-drag input
- valid / invalid swap animation
- clear animation
- gravity and refill animation
- cascade timing and combo banner
- moves and goal HUD
- win / fail overlay
- dead-board shuffle presentation
- replay

The presentation consumes results from `XMatch.Core`. It does not reimplement match rules.

## Next visual step

Replace generated placeholder blocks with production-quality X MATCH art only after Unity compile, interaction, and device validation are complete.
