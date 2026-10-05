using System;

namespace XMatch.Core
{
    public sealed class GoalProgress
    {
        public GoalProgress(GoalDefinition definition)
        {
            Definition = definition ??
                throw new ArgumentNullException(nameof(definition));
        }

        public GoalDefinition Definition { get; }
        public int CurrentCount { get; private set; }
        public int RemainingCount =>
            Math.Max(0, Definition.TargetCount - CurrentCount);
        public bool IsComplete =>
            CurrentCount >= Definition.TargetCount;

        internal void Apply(ClearedTile tile)
        {
            if (IsComplete)
            {
                return;
            }

            if (Definition.Kind == GoalKind.CollectTile &&
                tile.Kind == Definition.TileKind)
            {
                CurrentCount = Math.Min(
                    Definition.TargetCount,
                    CurrentCount + 1);
            }
        }
    }
}
