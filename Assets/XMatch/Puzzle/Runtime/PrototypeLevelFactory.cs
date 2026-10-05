using System.Collections.Generic;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public static class PrototypeLevelFactory
    {
        public static StageSession CreateStage()
        {
            const int width = 8;
            const int height = 9;

            IReadOnlyList<TileKind> cells =
                BoardGenerator.GenerateStableCells(
                    width,
                    height,
                    seed: 20261004);

            var goals = new[]
            {
                GoalDefinition.CollectTile(
                    TileKind.Heart,
                    12),
                GoalDefinition.CollectTile(
                    TileKind.Diamond,
                    10),
                GoalDefinition.CollectTile(
                    TileKind.Rose,
                    10)
            };

            var level = new LevelDefinition(
                id: "prototype-001",
                width: width,
                height: height,
                moveLimit: 25,
                refillSeed: 910041,
                initialCells: cells,
                goals: goals);

            return new StageSession(level);
        }
    }
}
