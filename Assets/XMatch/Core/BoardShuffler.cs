using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class BoardShuffler
    {
        public static bool TryShuffle(
            BoardState board,
            int seed,
            int maxAttempts = 200)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (maxAttempts <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxAttempts));
            }

            var original =
                new List<TileKind>(board.Width * board.Height);

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    TileKind kind =
                        board.Get(new BoardPosition(x, y));

                    if (kind == TileKind.Empty)
                    {
                        throw new InvalidOperationException(
                            "A stable board cannot be shuffled while it contains empty cells.");
                    }

                    original.Add(kind);
                }
            }

            var rng = new ShuffleRng(seed);

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                var shuffled = new List<TileKind>(original);
                FisherYates(shuffled, ref rng);
                Write(board, shuffled);

                if (MatchFinder.FindMatches(board).Count == 0 &&
                    BoardMoveFinder.HasLegalMove(board))
                {
                    return true;
                }
            }

            Write(board, original);
            return false;
        }

        private static void FisherYates(
            List<TileKind> tiles,
            ref ShuffleRng rng)
        {
            for (int i = tiles.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);

                TileKind temp = tiles[i];
                tiles[i] = tiles[j];
                tiles[j] = temp;
            }
        }

        private static void Write(
            BoardState board,
            IReadOnlyList<TileKind> cells)
        {
            int index = 0;

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    board.Set(
                        new BoardPosition(x, y),
                        cells[index++]);
                }
            }
        }

        private struct ShuffleRng
        {
            private uint state;

            public ShuffleRng(int seed)
            {
                state = unchecked((uint)seed);

                if (state == 0)
                {
                    state = 0x9E3779B9u;
                }
            }

            public int Next(int exclusiveMax)
            {
                if (exclusiveMax <= 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(exclusiveMax));
                }

                uint value = state;
                value ^= value << 13;
                value ^= value >> 17;
                value ^= value << 5;
                state = value;

                return (int)(value % (uint)exclusiveMax);
            }
        }
    }
}
