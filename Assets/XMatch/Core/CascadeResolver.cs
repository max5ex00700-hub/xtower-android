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
                null,
                maxCascades);
        }

        public static CascadeResult Resolve(
            BoardState board,
            IEnumerable<BoardPosition> initialMatches,
            ITileSource tileSource,
            int maxCascades = DefaultMaxCascades)
        {
            return Resolve(
                board,
                initialMatches,
                tileSource,
                null,
                maxCascades);
        }

        public static CascadeResult Resolve(
            BoardState board,
            IEnumerable<BoardPosition> initialMatches,
            ITileSource tileSource,
            BoardPosition? preferredPowerUpPosition,
            int maxCascades = DefaultMaxCascades)
        {
            return ResolveInternal(
                board,
                initialMatches,
                tileSource,
                preferredPowerUpPosition,
                firstStepIsForcedClear: false,
                maxCascades: maxCascades);
        }

        public static CascadeResult ResolveForcedClear(
            BoardState board,
            IEnumerable<BoardPosition> forcedClear,
            ITileSource tileSource,
            int maxCascades = DefaultMaxCascades)
        {
            return ResolveInternal(
                board,
                forcedClear,
                tileSource,
                preferredPowerUpPosition: null,
                firstStepIsForcedClear: true,
                maxCascades: maxCascades);
        }

        private static CascadeResult ResolveInternal(
            BoardState board,
            IEnumerable<BoardPosition> initialPositions,
            ITileSource tileSource,
            BoardPosition? preferredPowerUpPosition,
            bool firstStepIsForcedClear,
            int maxCascades)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (initialPositions == null)
            {
                throw new ArgumentNullException(nameof(initialPositions));
            }

            if (tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            }

            if (maxCascades <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxCascades));
            }

            var positions = new HashSet<BoardPosition>(initialPositions);
            var steps = new List<CascadeStep>();
            bool first = true;

            while (positions.Count > 0)
            {
                if (steps.Count >= maxCascades)
                {
                    throw new InvalidOperationException(
                        $"Cascade limit of {maxCascades} was reached. " +
                        "Check the tile source or level state for an endless cascade.");
                }

                bool forced = first && firstStepIsForcedClear;
                PowerUpCreation? creation = null;

                if (!forced)
                {
                    creation = PowerUpPlanner.Plan(
                        board,
                        positions,
                        first
                            ? preferredPowerUpPosition
                            : null);
                }

                var clearSet =
                    new HashSet<BoardPosition>(positions);

                if (creation.HasValue)
                {
                    clearSet.Remove(
                        creation.Value.Position);
                }

                PowerUpResolver.ExpandTriggeredPowerUps(
                    board,
                    clearSet);

                if (creation.HasValue)
                {
                    clearSet.Remove(
                        creation.Value.Position);
                }

                List<ClearedTile> cleared =
                    BoardResolver.Clear(board, clearSet);

                if (creation.HasValue)
                {
                    PowerUpCreation value =
                        creation.Value;

                    board.SetCell(
                        value.Position,
                        value.Kind,
                        value.PowerUp);
                }

                List<TileMove> moved =
                    BoardResolver.ApplyGravity(board);

                List<TileSpawn> spawned =
                    BoardResolver.Refill(board, tileSource);

                steps.Add(
                    new CascadeStep(
                        steps.Count + 1,
                        cleared,
                        moved,
                        spawned,
                        creation));

                positions = MatchFinder.FindMatches(board);
                first = false;
            }

            return new CascadeResult(steps);
        }
    }
}
