# X MATCH - Game Design v0.01

## 1. Game fantasy

X MATCH is a match-3 game where puzzle progress unlocks encounters, relationship progress, outfits, locations, and episodic character stories.

The player should always understand **what the next puzzle unlocks**.

## 2. Core loop

```text
Stage start
  -> Match-3 play
  -> Win
  -> Reward
  -> Character/story progression
  -> Tease the next reward
  -> Next stage
```

Failure returns the player to the same clear goal with minimal friction.

## 3. Puzzle pillars

### Immediate readability
A player should understand the board, goal, and remaining moves in a glance.

### Strong combo feedback
Special pieces and cascades should create escalating visual and audio payoff.

### Fair pressure
Difficulty should come from board structure and goals, not from unreadable rules.

### Short sessions
Early stages should generally resolve in a few minutes.

## 4. Initial tile set

The first five normal tile identities are:

- Heart
- Lips
- Diamond
- Perfume
- Rose

These are logical identities only. Art can change without changing puzzle rules.

## 5. Initial board

Target default board:

- Width: 8
- Height: 9
- Normal colors/types: 5

Level data must be able to override these values later.

## 6. First vertical slice

The first playable slice needs:

- 10 hand-authored levels
- 1 adult heroine
- 1 location
- 1 short episode
- 2 character expressions
- 1 affection meter
- 1 clear reward event
- 1 failure screen
- 3 boosters can be placeholders until the base board feels good

## 7. Reward structure

For the first slice, progression should be simple:

```text
Level clear
 -> episode progress +1
 -> affection progress
 -> dialogue / visual change at thresholds
```

Do not build seasons, clans, PvP, card collections, or a large store before the vertical slice proves retention value.

## 8. Success criteria

The vertical slice succeeds when a new player can:

1. Understand a stage without explanation.
2. Enjoy swaps, clears, cascades, and feedback.
3. Recognize what clearing the level unlocks.
4. Want to play the next stage because of both puzzle and character progression.
