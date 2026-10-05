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

            HashSet<BoardPosition> initialMatches;

            if (!SwapLogic.TrySwap(
                    board,
                    from,
                    to,
                    out initialMatches))
            {
                return MoveResult.Rejected(from, to);
            }

            CascadeResult cascades = CascadeResolver.Resolve(
                board,
                initialMatches,
                tileSource,
                maxCascades);

            return MoveResult.AcceptedMove(
                from,
                to,
                cascades);
        }
    }
}
