using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class CascadeResolver
    {
        public const int DefaultMaxCascades = 100;

        public static CascadeResult ResolveCurrentMatches(
            BoardState board,
            ITileSource tileSource,
            int maxCascades = DefaultMaxCascades)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            return Resolve(
                board,
                MatchFinder.FindMatches(board),
                tileSource,
                maxCascades);
        }

        public static CascadeResult Resolve(
            BoardState board,
            IEnumerable<BoardPosition> initialMatches,
            ITileSource tileSource,
            int maxCascades = DefaultMaxCascades)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (initialMatches == null)
            {
                throw new ArgumentNullException(nameof(initialMatches));
            }

            if (tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            }

            if (maxCascades <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCascades));
            }

            var matches = new HashSet<BoardPosition>(initialMatches);
            var steps = new List<CascadeStep>();

            while (matches.Count > 0)
            {
                if (steps.Count >= maxCascades)
                {
                    throw new InvalidOperationException(
                        $"Cascade limit of {maxCascades} was reached. " +
                        "Check the tile source or level state for an endless cascade.");
                }

                List<ClearedTile> cleared =
                    BoardResolver.Clear(board, matches);

                List<TileMove> moved =
                    BoardResolver.ApplyGravity(board);

                List<TileSpawn> spawned =
                    BoardResolver.Refill(board, tileSource);

                steps.Add(
                    new CascadeStep(
                        steps.Count + 1,
                        cleared,
                        moved,
                        spawned));

                matches = MatchFinder.FindMatches(board);
            }

            return new CascadeResult(steps);
        }
    }
}
