using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class PowerUpPlanner
    {
        public static PowerUpCreation? Plan(
            BoardState board,
            IEnumerable<BoardPosition> matches,
            BoardPosition? preferred = null)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (matches == null)
            {
                throw new ArgumentNullException(nameof(matches));
            }

            var candidates = new List<BoardPosition>(matches);
            candidates.Sort(ComparePositions);

            if (preferred.HasValue)
            {
                int index = candidates.IndexOf(preferred.Value);

                if (index > 0)
                {
                    BoardPosition value = candidates[index];
                    candidates.RemoveAt(index);
                    candidates.Insert(0, value);
                }
            }

            PowerUpCreation? best = null;
            int bestScore = 0;

            for (int i = 0; i < candidates.Count; i++)
            {
                BoardPosition position = candidates[i];

                if (!board.IsInside(position) ||
                    board.GetPowerUp(position) != PowerUpKind.None)
                {
                    continue;
                }

                TileKind kind = board.Get(position);

                if (!MatchFinder.IsMatchable(kind))
                {
                    continue;
                }

                int horizontal = CountRun(board, position, 1, 0);
                int vertical = CountRun(board, position, 0, 1);

                PowerUpKind power = PowerUpKind.None;
                int score = 0;

                if (horizontal >= 5 || vertical >= 5)
                {
                    power = PowerUpKind.ColorOrb;
                    score = 50;
                }
                else if (horizontal >= 3 && vertical >= 3)
                {
                    power = PowerUpKind.Bomb;
                    score = 40;
                }
                else if (horizontal >= 4)
                {
                    power = PowerUpKind.RowBlast;
                    score = 30;
                }
                else if (vertical >= 4)
                {
                    power = PowerUpKind.ColumnBlast;
                    score = 30;
                }
                else if (IsPartOfSquare(board, position))
                {
                    power = PowerUpKind.Seeker;
                    score = 20;
                }

                if (score <= bestScore)
                {
                    continue;
                }

                TileKind createdKind =
                    power == PowerUpKind.ColorOrb
                        ? TileKind.Wild
                        : kind;

                best = new PowerUpCreation(
                    position,
                    createdKind,
                    power);
                bestScore = score;
            }

            return best;
        }

        private static int CountRun(
            BoardState board,
            BoardPosition position,
            int dx,
            int dy)
        {
            TileKind kind = board.Get(position);

            if (!MatchFinder.IsMatchable(kind))
            {
                return 0;
            }

            int count = 1;
            count += CountDirection(board, position, kind, dx, dy);
            count += CountDirection(board, position, kind, -dx, -dy);
            return count;
        }

        private static int CountDirection(
            BoardState board,
            BoardPosition origin,
            TileKind kind,
            int dx,
            int dy)
        {
            int count = 0;
            int x = origin.X + dx;
            int y = origin.Y + dy;

            while (x >= 0 && x < board.Width &&
                   y >= 0 && y < board.Height)
            {
                if (board.Get(new BoardPosition(x, y)) != kind)
                {
                    break;
                }

                count++;
                x += dx;
                y += dy;
            }

            return count;
        }

        private static bool IsPartOfSquare(
            BoardState board,
            BoardPosition position)
        {
            TileKind kind = board.Get(position);

            if (!MatchFinder.IsMatchable(kind))
            {
                return false;
            }

            for (int ox = -1; ox <= 0; ox++)
            {
                for (int oy = -1; oy <= 0; oy++)
                {
                    int x = position.X + ox;
                    int y = position.Y + oy;

                    if (x < 0 || y < 0 ||
                        x + 1 >= board.Width ||
                        y + 1 >= board.Height)
                    {
                        continue;
                    }

                    if (board.Get(new BoardPosition(x, y)) == kind &&
                        board.Get(new BoardPosition(x + 1, y)) == kind &&
                        board.Get(new BoardPosition(x, y + 1)) == kind &&
                        board.Get(new BoardPosition(x + 1, y + 1)) == kind)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static int ComparePositions(
            BoardPosition left,
            BoardPosition right)
        {
            int byY = left.Y.CompareTo(right.Y);

            return byY != 0
                ? byY
                : left.X.CompareTo(right.X);
        }
    }
}
