using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class MoveResolver
    {
        public static MoveResult TryResolve(
            BoardState board,
            BoardPosition from,
            BoardPosition to,
            ITileSource tileSource,
            int maxCascades = CascadeResolver.DefaultMaxCascades)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            }

            if (SwapLogic.IsPowerUpSwap(board, from, to))
            {
                TileKind fromKind = board.Get(from);
                TileKind toKind = board.Get(to);
                PowerUpKind fromPower = board.GetPowerUp(from);
                PowerUpKind toPower = board.GetPowerUp(to);

                board.Swap(from, to);

                HashSet<BoardPosition> forced =
                    PowerUpResolver.CreatePowerUpSwapClear(
                        board,
                        from,
                        to,
                        fromKind,
                        toKind,
                        fromPower,
                        toPower);

                CascadeResult powerCascades =
                    CascadeResolver.ResolveForcedClear(
                        board,
                        forced,
                        tileSource,
                        maxCascades);

                return MoveResult.AcceptedMove(
                    from,
                    to,
                    powerCascades);
            }

            HashSet<BoardPosition> initialMatches;

            if (!SwapLogic.TrySwap(
                    board,
                    from,
                    to,
                    out initialMatches))
            {
                return MoveResult.Rejected(from, to);
            }

            BoardPosition preferred =
                initialMatches.Contains(to)
                    ? to
                    : from;

            CascadeResult cascades = CascadeResolver.Resolve(
                board,
                initialMatches,
                tileSource,
                preferred,
                maxCascades);

            return MoveResult.AcceptedMove(
                from,
                to,
                cascades);
        }
    }
}
