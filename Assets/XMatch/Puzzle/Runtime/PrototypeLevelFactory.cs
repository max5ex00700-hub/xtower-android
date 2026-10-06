using System;
using System.Collections.Generic;
using XMatch.Core;

namespace XMatch.Puzzle
{
    public static class PrototypeLevelFactory
    {
        public const int LevelCount = 10;

        public static StageSession CreateStage()
        {
            return CreateStage(0);
        }

        public static StageSession CreateStage(int levelIndex)
        {
            if (levelIndex < 0 ||
                levelIndex >= LevelCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(levelIndex));
            }

            LevelSpec spec = GetSpec(levelIndex);

            IReadOnlyList<TileKind> cells =
                BoardGenerator.GenerateStableCells(
                    spec.Width,
                    spec.Height,
                    spec.BoardSeed);

            var level = new LevelDefinition(
                id: $"prototype-{levelIndex + 1:000}",
                width: spec.Width,
                height: spec.Height,
                moveLimit: spec.MoveLimit,
                refillSeed: spec.RefillSeed,
                initialCells: cells,
                goals: spec.Goals);

            return new StageSession(level);
        }

        public static string GetTitle(int levelIndex)
        {
            switch (levelIndex)
            {
                case 0:
                    return "FIRST SPARK";
                case 1:
                    return "ROSE NIGHT";
                case 2:
                    return "DOUBLE DATE";
                case 3:
                    return "LINE FEVER";
                case 4:
                    return "COLOR CRUSH";
                case 5:
                    return "BOMBSHELL";
                case 6:
                    return "SECRET CHASE";
                case 7:
                    return "TRIPLE DESIRE";
                case 8:
                    return "MIDNIGHT COMBO";
                case 9:
                    return "FINAL RENDEZVOUS";
                default:
                    return "X MATCH";
            }
        }

        public static string GetHint(int levelIndex)
        {
            switch (levelIndex)
            {
                case 0:
                    return "Learn the rhythm";
                case 1:
                    return "Collect roses";
                case 2:
                    return "Two goals at once";
                case 3:
                    return "Make 4 for line blasts";
                case 4:
                    return "Make 5 for a color orb";
                case 5:
                    return "T and L shapes make bombs";
                case 6:
                    return "2x2 makes a seeker";
                case 7:
                    return "Chain special blocks";
                case 8:
                    return "Build bigger combos";
                case 9:
                    return "Use everything you learned";
                default:
                    return string.Empty;
            }
        }

        private static LevelSpec GetSpec(int levelIndex)
        {
            switch (levelIndex)
            {
                case 0:
                    return new LevelSpec(
                        7,
                        8,
                        18,
                        20261011,
                        910101,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Heart,
                                10)
                        });

                case 1:
                    return new LevelSpec(
                        7,
                        8,
                        20,
                        20261012,
                        910102,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Rose,
                                12)
                        });

                case 2:
                    return new LevelSpec(
                        8,
                        8,
                        20,
                        20261013,
                        910103,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Diamond,
                                8),
                            GoalDefinition.CollectTile(
                                TileKind.Perfume,
                                8)
                        });

                case 3:
                    return new LevelSpec(
                        8,
                        9,
                        22,
                        20261014,
                        910104,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Heart,
                                10),
                            GoalDefinition.CollectTile(
                                TileKind.Rose,
                                10)
                        });

                case 4:
                    return new LevelSpec(
                        8,
                        9,
                        18,
                        20261015,
                        910105,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Diamond,
                                12)
                        });

                case 5:
                    return new LevelSpec(
                        8,
                        9,
                        22,
                        20261016,
                        910106,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Perfume,
                                10),
                            GoalDefinition.CollectTile(
                                TileKind.Lips,
                                10)
                        });

                case 6:
                    return new LevelSpec(
                        8,
                        9,
                        20,
                        20261017,
                        910107,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Rose,
                                14)
                        });

                case 7:
                    return new LevelSpec(
                        8,
                        9,
                        24,
                        20261018,
                        910108,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Heart,
                                10),
                            GoalDefinition.CollectTile(
                                TileKind.Diamond,
                                10),
                            GoalDefinition.CollectTile(
                                TileKind.Perfume,
                                10)
                        });

                case 8:
                    return new LevelSpec(
                        8,
                        9,
                        23,
                        20261019,
                        910109,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Rose,
                                16),
                            GoalDefinition.CollectTile(
                                TileKind.Lips,
                                12)
                        });

                case 9:
                    return new LevelSpec(
                        8,
                        9,
                        28,
                        20261020,
                        910110,
                        new[]
                        {
                            GoalDefinition.CollectTile(
                                TileKind.Heart,
                                10),
                            GoalDefinition.CollectTile(
                                TileKind.Rose,
                                10),
                            GoalDefinition.CollectTile(
                                TileKind.Diamond,
                                8),
                            GoalDefinition.CollectTile(
                                TileKind.Perfume,
                                8),
                            GoalDefinition.CollectTile(
                                TileKind.Lips,
                                8)
                        });

                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(levelIndex));
            }
        }

        private readonly struct LevelSpec
        {
            public LevelSpec(
                int width,
                int height,
                int moveLimit,
                int boardSeed,
                int refillSeed,
                IReadOnlyList<GoalDefinition> goals)
            {
                Width = width;
                Height = height;
                MoveLimit = moveLimit;
                BoardSeed = boardSeed;
                RefillSeed = refillSeed;
                Goals = goals;
            }

            public int Width { get; }
            public int Height { get; }
            public int MoveLimit { get; }
            public int BoardSeed { get; }
            public int RefillSeed { get; }
            public IReadOnlyList<GoalDefinition> Goals { get; }
        }
    }
}
