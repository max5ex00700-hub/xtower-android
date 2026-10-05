using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public sealed class StageSession
    {
        private readonly ITileSource tileSource;
        private readonly List<GoalProgress> goals;
        private readonly IReadOnlyList<GoalProgress> readOnlyGoals;

        public StageSession(
            LevelDefinition level,
            ITileSource tileSource = null)
        {
            Level = level ??
                throw new ArgumentNullException(nameof(level));

            LevelValidator.ValidateOrThrow(Level);

            Board = Level.CreateBoard();
            MovesRemaining = Level.MoveLimit;
            Status = StageStatus.InProgress;

            this.tileSource =
                tileSource ?? new SeededTileSource(Level.RefillSeed);

            goals = new List<GoalProgress>(Level.Goals.Count);

            for (int i = 0; i < Level.Goals.Count; i++)
            {
                goals.Add(new GoalProgress(Level.Goals[i]));
            }

            readOnlyGoals = goals.AsReadOnly();
        }

        public LevelDefinition Level { get; }
        public BoardState Board { get; }
        public int MovesRemaining { get; private set; }
        public int ShuffleCount { get; private set; }
        public StageStatus Status { get; private set; }
        public IReadOnlyList<GoalProgress> Goals => readOnlyGoals;

        public bool NeedsShuffle =>
            Status == StageStatus.InProgress &&
            !BoardMoveFinder.HasLegalMove(Board);

        public StageTurnResult TryMove(
            BoardPosition from,
            BoardPosition to)
        {
            if (Status != StageStatus.InProgress)
            {
                throw new InvalidOperationException(
                    "Cannot play a move after the stage has ended.");
            }

            MoveResult move = MoveResolver.TryResolve(
                Board,
                from,
                to,
                tileSource);

            if (!move.Accepted)
            {
                return new StageTurnResult(
                    move,
                    MovesRemaining,
                    Status,
                    NeedsShuffle);
            }

            MovesRemaining--;
            ApplyGoalProgress(move.Cascades);
            UpdateStatus();

            return new StageTurnResult(
                move,
                MovesRemaining,
                Status,
                NeedsShuffle);
        }

        public StageTurnResult TryActivatePowerUp(
            BoardPosition target)
        {
            if (Status != StageStatus.InProgress)
            {
                throw new InvalidOperationException(
                    "Cannot activate a power-up after the stage has ended.");
            }

            if (!Board.IsInside(target) ||
                Board.Get(target) == TileKind.Empty ||
                Board.GetPowerUp(target) == PowerUpKind.None)
            {
                return new StageTurnResult(
                    MoveResult.Rejected(
                        target,
                        target),
                    MovesRemaining,
                    Status,
                    NeedsShuffle);
            }

            var clearSet =
                new HashSet<BoardPosition>
                {
                    target
                };

            CascadeResult cascades =
                CascadeResolver.ResolveForcedClear(
                    Board,
                    clearSet,
                    tileSource);

            MoveResult move =
                MoveResult.AcceptedMove(
                    target,
                    target,
                    cascades);

            MovesRemaining--;
            ApplyGoalProgress(cascades);
            UpdateStatus();

            return new StageTurnResult(
                move,
                MovesRemaining,
                Status,
                NeedsShuffle);
        }

        public CascadeResult UseBooster(
            BoosterKind booster,
            BoardPosition target)
        {
            if (Status != StageStatus.InProgress)
            {
                throw new InvalidOperationException(
                    "Cannot use a booster after the stage has ended.");
            }

            if (!Board.IsInside(target))
            {
                throw new ArgumentOutOfRangeException(nameof(target));
            }

            var clearSet = new HashSet<BoardPosition>();

            switch (booster)
            {
                case BoosterKind.Hammer:
                    clearSet.Add(target);
                    break;

                case BoosterKind.RowClear:
                    for (int x = 0; x < Board.Width; x++)
                    {
                        clearSet.Add(
                            new BoardPosition(x, target.Y));
                    }
                    break;

                case BoosterKind.ColumnClear:
                    for (int y = 0; y < Board.Height; y++)
                    {
                        clearSet.Add(
                            new BoardPosition(target.X, y));
                    }
                    break;

                default:
                    throw new ArgumentException(
                        "This booster requires a board target.",
                        nameof(booster));
            }

            CascadeResult cascades =
                CascadeResolver.ResolveForcedClear(
                    Board,
                    clearSet,
                    tileSource);

            ApplyGoalProgress(cascades);
            UpdateStatus();
            return cascades;
        }

        public bool UseShuffleBooster(int maxAttempts = 200)
        {
            if (Status != StageStatus.InProgress)
            {
                return false;
            }

            int seed = unchecked(
                Level.RefillSeed ^
                ((ShuffleCount + 1) * 1640531527));

            if (!BoardShuffler.TryShuffle(
                    Board,
                    seed,
                    maxAttempts))
            {
                return false;
            }

            ShuffleCount++;
            return true;
        }

        public bool TryShuffleBoard(int maxAttempts = 200)
        {
            if (Status != StageStatus.InProgress ||
                !NeedsShuffle)
            {
                return false;
            }

            int seed = unchecked(
                Level.RefillSeed ^
                ((ShuffleCount + 1) * 1103515245));

            if (!BoardShuffler.TryShuffle(
                    Board,
                    seed,
                    maxAttempts))
            {
                return false;
            }

            ShuffleCount++;
            return true;
        }

        private void ApplyGoalProgress(CascadeResult cascades)
        {
            for (int stepIndex = 0;
                 stepIndex < cascades.Steps.Count;
                 stepIndex++)
            {
                CascadeStep step = cascades.Steps[stepIndex];

                for (int clearedIndex = 0;
                     clearedIndex < step.Cleared.Count;
                     clearedIndex++)
                {
                    ClearedTile tile = step.Cleared[clearedIndex];

                    for (int goalIndex = 0;
                         goalIndex < goals.Count;
                         goalIndex++)
                    {
                        goals[goalIndex].Apply(tile);
                    }
                }
            }
        }

        private void UpdateStatus()
        {
            bool allComplete = true;

            for (int i = 0; i < goals.Count; i++)
            {
                if (!goals[i].IsComplete)
                {
                    allComplete = false;
                    break;
                }
            }

            if (allComplete)
            {
                Status = StageStatus.Won;
                return;
            }

            if (MovesRemaining <= 0)
            {
                Status = StageStatus.Lost;
            }
        }
    }
}
