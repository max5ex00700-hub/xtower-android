using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class SwapLogic
    {
        public static bool AreAdjacent(BoardPosition a, BoardPosition b)
        {
            int dx = a.X - b.X;
            int dy = a.Y - b.Y;

            if (dx < 0)
            {
                dx = -dx;
            }

            if (dy < 0)
            {
                dy = -dy;
            }

            return dx + dy == 1;
        }

        public static bool IsPowerUpSwap(
            BoardState board,
            BoardPosition a,
            BoardPosition b)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (!CanAttemptSwap(board, a, b))
            {
                return false;
            }

            PowerUpKind aPower = board.GetPowerUp(a);
            PowerUpKind bPower = board.GetPowerUp(b);

            return aPower == PowerUpKind.ColorOrb ||
                   bPower == PowerUpKind.ColorOrb ||
                   (aPower != PowerUpKind.None &&
                    bPower != PowerUpKind.None);
        }

        public static bool WouldCreateMatch(
            BoardState board,
            BoardPosition a,
            BoardPosition b)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (!CanAttemptSwap(board, a, b))
            {
                return false;
            }

            if (IsPowerUpSwap(board, a, b))
            {
                return true;
            }

            board.Swap(a, b);

            try
            {
                HashSet<BoardPosition> found =
                    MatchFinder.FindMatches(board);

                return found.Contains(a) || found.Contains(b);
            }
            finally
            {
                board.Swap(a, b);
            }
        }

        public static bool TrySwap(
            BoardState board,
            BoardPosition a,
            BoardPosition b,
            out HashSet<BoardPosition> matches)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            matches = new HashSet<BoardPosition>();

            if (!CanAttemptSwap(board, a, b) ||
                IsPowerUpSwap(board, a, b))
            {
                return false;
            }

            board.Swap(a, b);

            HashSet<BoardPosition> found =
                MatchFinder.FindMatches(board);

            bool createsLocalMatch =
                found.Contains(a) || found.Contains(b);

            if (!createsLocalMatch)
            {
                board.Swap(a, b);
                return false;
            }

            matches = found;
            return true;
        }

        private static bool CanAttemptSwap(
            BoardState board,
            BoardPosition a,
            BoardPosition b)
        {
            if (!board.IsInside(a) ||
                !board.IsInside(b) ||
                !AreAdjacent(a, b))
            {
                return false;
            }

            TileKind aKind = board.Get(a);
            TileKind bKind = board.Get(b);

            if (aKind == TileKind.Empty ||
                bKind == TileKind.Empty)
            {
                return false;
            }

            if (aKind != bKind)
            {
                return true;
            }

            return board.GetPowerUp(a) != PowerUpKind.None ||
                   board.GetPowerUp(b) != PowerUpKind.None;
        }
    }
}
