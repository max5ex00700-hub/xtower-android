using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class PowerUpResolver
    {
        public static void ExpandTriggeredPowerUps(
            BoardState board,
            HashSet<BoardPosition> clearSet)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (clearSet == null)
            {
                throw new ArgumentNullException(nameof(clearSet));
            }

            var queue = new Queue<BoardPosition>(clearSet);

            while (queue.Count > 0)
            {
                BoardPosition position = queue.Dequeue();

                if (!board.IsInside(position) ||
                    board.Get(position) == TileKind.Empty)
                {
                    continue;
                }

                PowerUpKind power = board.GetPowerUp(position);

                switch (power)
                {
                    case PowerUpKind.RowBlast:
                        AddRow(board, clearSet, queue, position.Y);
                        break;

                    case PowerUpKind.ColumnBlast:
                        AddColumn(board, clearSet, queue, position.X);
                        break;

                    case PowerUpKind.Bomb:
                        AddBomb(board, clearSet, queue, position, 1);
                        break;

                    case PowerUpKind.ColorOrb:
                        AddMostCommonColor(board, clearSet, queue);
                        break;

                    case PowerUpKind.Seeker:
                        AddSeekerTarget(board, clearSet, queue, position);
                        break;
                }
            }
        }

        public static HashSet<BoardPosition> CreatePowerUpSwapClear(
            BoardState board,
            BoardPosition from,
            BoardPosition to,
            TileKind fromKindBeforeSwap,
            TileKind toKindBeforeSwap,
            PowerUpKind fromPowerBeforeSwap,
            PowerUpKind toPowerBeforeSwap)
        {
            var clearSet = new HashSet<BoardPosition>
            {
                from,
                to
            };

            bool fromOrb = fromPowerBeforeSwap == PowerUpKind.ColorOrb;
            bool toOrb = toPowerBeforeSwap == PowerUpKind.ColorOrb;

            if (fromOrb && toOrb)
            {
                AddAll(board, clearSet);
                return clearSet;
            }

            if (fromOrb || toOrb)
            {
                TileKind targetKind =
                    fromOrb
                        ? toKindBeforeSwap
                        : fromKindBeforeSwap;

                if (MatchFinder.IsMatchable(targetKind))
                {
                    AddColor(board, clearSet, targetKind);
                }
            }

            ExpandTriggeredPowerUps(board, clearSet);
            return clearSet;
        }

        private static void AddRow(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            int y)
        {
            for (int x = 0; x < board.Width; x++)
            {
                Add(board, clearSet, queue, new BoardPosition(x, y));
            }
        }

        private static void AddColumn(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            int x)
        {
            for (int y = 0; y < board.Height; y++)
            {
                Add(board, clearSet, queue, new BoardPosition(x, y));
            }
        }

        private static void AddBomb(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition center,
            int radius)
        {
            for (int y = center.Y - radius; y <= center.Y + radius; y++)
            {
                for (int x = center.X - radius; x <= center.X + radius; x++)
                {
                    Add(board, clearSet, queue, new BoardPosition(x, y));
                }
            }
        }

        private static void AddMostCommonColor(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue)
        {
            var counts = new Dictionary<TileKind, int>();

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TileKind kind = board.Get(new BoardPosition(x, y));

                    if (!MatchFinder.IsMatchable(kind))
                    {
                        continue;
                    }

                    int count;
                    counts.TryGetValue(kind, out count);
                    counts[kind] = count + 1;
                }
            }

            TileKind bestKind = TileKind.Empty;
            int bestCount = 0;

            foreach (KeyValuePair<TileKind, int> pair in counts)
            {
                if (pair.Value > bestCount ||
                    (pair.Value == bestCount &&
                     (int)pair.Key < (int)bestKind))
                {
                    bestKind = pair.Key;
                    bestCount = pair.Value;
                }
            }

            if (bestKind != TileKind.Empty)
            {
                AddColor(board, clearSet, queue, bestKind);
            }
        }

        private static void AddColor(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            TileKind kind)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var position = new BoardPosition(x, y);

                    if (board.Get(position) == kind)
                    {
                        clearSet.Add(position);
                    }
                }
            }
        }

        private static void AddColor(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            TileKind kind)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var position = new BoardPosition(x, y);

                    if (board.Get(position) == kind)
                    {
                        Add(board, clearSet, queue, position);
                    }
                }
            }
        }

        private static void AddSeekerTarget(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition origin)
        {
            BoardPosition? best = null;
            int bestDistance = int.MaxValue;

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var candidate = new BoardPosition(x, y);

                    if (clearSet.Contains(candidate) ||
                        board.Get(candidate) == TileKind.Empty)
                    {
                        continue;
                    }

                    int distance =
                        Math.Abs(candidate.X - origin.X) +
                        Math.Abs(candidate.Y - origin.Y);

                    if (distance < bestDistance)
                    {
                        best = candidate;
                        bestDistance = distance;
                    }
                }
            }

            if (best.HasValue)
            {
                Add(board, clearSet, queue, best.Value);
            }
        }

        private static void AddAll(
            BoardState board,
            HashSet<BoardPosition> clearSet)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var position = new BoardPosition(x, y);

                    if (board.Get(position) != TileKind.Empty)
                    {
                        clearSet.Add(position);
                    }
                }
            }
        }

        private static void Add(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition position)
        {
            if (!board.IsInside(position) ||
                board.Get(position) == TileKind.Empty)
            {
                return;
            }

            if (clearSet.Add(position))
            {
                queue.Enqueue(position);
            }
        }
    }
}
