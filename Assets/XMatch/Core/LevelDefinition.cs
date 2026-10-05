using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public sealed class LevelDefinition
    {
        private readonly IReadOnlyList<TileKind> initialCells;
        private readonly IReadOnlyList<GoalDefinition> goals;

        public LevelDefinition(
            string id,
            int width,
            int height,
            int moveLimit,
            int refillSeed,
            IEnumerable<TileKind> initialCells,
            IEnumerable<GoalDefinition> goals)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "A level id is required.",
                    nameof(id));
            }

            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (moveLimit <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(moveLimit));
            }

            if (initialCells == null)
            {
                throw new ArgumentNullException(nameof(initialCells));
            }

            if (goals == null)
            {
                throw new ArgumentNullException(nameof(goals));
            }

            var cellCopy = new List<TileKind>(initialCells);

            if (cellCopy.Count != width * height)
            {
                throw new ArgumentException(
                    $"Expected {width * height} initial cells but received {cellCopy.Count}.",
                    nameof(initialCells));
            }

            for (int i = 0; i < cellCopy.Count; i++)
            {
                if (cellCopy[i] == TileKind.Empty)
                {
                    throw new ArgumentException(
                        "Initial level cells cannot be TileKind.Empty in v0.01.",
                        nameof(initialCells));
                }
            }

            var goalCopy = new List<GoalDefinition>(goals);

            if (goalCopy.Count == 0)
            {
                throw new ArgumentException(
                    "At least one goal is required.",
                    nameof(goals));
            }

            for (int i = 0; i < goalCopy.Count; i++)
            {
                if (goalCopy[i] == null)
                {
                    throw new ArgumentException(
                        "Goal definitions cannot contain null.",
                        nameof(goals));
                }
            }

            Id = id;
            Width = width;
            Height = height;
            MoveLimit = moveLimit;
            RefillSeed = refillSeed;
            this.initialCells = cellCopy.AsReadOnly();
            this.goals = goalCopy.AsReadOnly();
        }

        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        public int MoveLimit { get; }
        public int RefillSeed { get; }
        public IReadOnlyList<TileKind> InitialCells => initialCells;
        public IReadOnlyList<GoalDefinition> Goals => goals;

        public BoardState CreateBoard()
        {
            var board = new BoardState(Width, Height);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int index = (y * Width) + x;
                    board.Set(
                        new BoardPosition(x, y),
                        initialCells[index]);
                }
            }

            return board;
        }
    }
}
