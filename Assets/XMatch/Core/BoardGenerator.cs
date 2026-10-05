using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class BoardGenerator
    {
        private static readonly TileKind[] NormalKinds =
        {
            TileKind.Heart,
            TileKind.Lips,
            TileKind.Diamond,
            TileKind.Perfume,
            TileKind.Rose
        };

        public static IReadOnlyList<TileKind> GenerateStableCells(
            int width,
            int height,
            int seed,
            int maxBoardAttempts = 200)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            if (maxBoardAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBoardAttempts));
            }

            for (int attempt = 0; attempt < maxBoardAttempts; attempt++)
            {
                int attemptSeed = unchecked(seed + (attempt * 104729));
                var source = new SeededTileSource(attemptSeed, NormalKinds);
                var board = new BoardState(width, height);

                FillWithoutImmediateMatches(board, source);

                if (MatchFinder.FindMatches(board).Count == 0 &&
                    BoardMoveFinder.HasLegalMove(board))
                {
                    return Flatten(board).AsReadOnly();
                }
            }

            throw new InvalidOperationException(
                $"Could not generate a stable {width}x{height} board " +
                $"with a legal move after {maxBoardAttempts} attempts.");
        }

        private static void FillWithoutImmediateMatches(
            BoardState board,
            ITileSource source)
        {
            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var position = new BoardPosition(x, y);
                    bool placed = false;

                    for (int candidateAttempt = 0;
                         candidateAttempt < 20;
                         candidateAttempt++)
                    {
                        TileKind candidate = source.NextTile(position);

                        if (!CreatesImmediateRun(
                                board,
                                position,
                                candidate))
                        {
                            board.Set(position, candidate);
                            placed = true;
                            break;
                        }
                    }

                    if (placed)
                    {
                        continue;
                    }

                    for (int i = 0; i < NormalKinds.Length; i++)
                    {
                        TileKind fallback = NormalKinds[i];

                        if (!CreatesImmediateRun(
                                board,
                                position,
                                fallback))
                        {
                            board.Set(position, fallback);
                            placed = true;
                            break;
                        }
                    }

                    if (!placed)
                    {
                        throw new InvalidOperationException(
                            $"Could not place a tile at {position}.");
                    }
                }
            }
        }

        private static bool CreatesImmediateRun(
            BoardState board,
            BoardPosition position,
            TileKind candidate)
        {
            if (position.X >= 2)
            {
                TileKind left1 = board.Get(
                    new BoardPosition(position.X - 1, position.Y));
                TileKind left2 = board.Get(
                    new BoardPosition(position.X - 2, position.Y));

                if (left1 == candidate && left2 == candidate)
                {
                    return true;
                }
            }

            if (position.Y >= 2)
            {
                TileKind down1 = board.Get(
                    new BoardPosition(position.X, position.Y - 1));
                TileKind down2 = board.Get(
                    new BoardPosition(position.X, position.Y - 2));

                if (down1 == candidate && down2 == candidate)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<TileKind> Flatten(BoardState board)
        {
            var cells =
                new List<TileKind>(board.Width * board.Height);

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    cells.Add(
                        board.Get(new BoardPosition(x, y)));
                }
            }

            return cells;
        }
    }
}
