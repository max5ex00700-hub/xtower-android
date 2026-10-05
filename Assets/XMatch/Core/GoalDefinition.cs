using System;

namespace XMatch.Core
{
    public sealed class GoalDefinition
    {
        public GoalDefinition(
            GoalKind kind,
            TileKind tileKind,
            int targetCount)
        {
            if (targetCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(targetCount));
            }

            if (kind == GoalKind.CollectTile &&
                tileKind == TileKind.Empty)
            {
                throw new ArgumentException(
                    "CollectTile goals require a non-empty tile kind.",
                    nameof(tileKind));
            }

            Kind = kind;
            TileKind = tileKind;
            TargetCount = targetCount;
        }

        public GoalKind Kind { get; }
        public TileKind TileKind { get; }
        public int TargetCount { get; }

        public static GoalDefinition CollectTile(
            TileKind tileKind,
            int targetCount)
        {
            return new GoalDefinition(
                GoalKind.CollectTile,
                tileKind,
                targetCount);
        }
    }
}
