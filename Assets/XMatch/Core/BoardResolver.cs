using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class BoardResolver
    {
        public static List<ClearedTile> Clear(
            BoardState board,
            IEnumerable<BoardPosition> positions)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (positions == null)
            {
                throw new ArgumentNullException(nameof(positions));
            }

            var unique = new HashSet<BoardPosition>(positions);
            var cleared = new List<ClearedTile>(unique.Count);

            foreach (BoardPosition position in unique)
            {
                if (!board.IsInside(position))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(positions),
                        $"Position {position} is outside the board.");
                }

                TileKind kind = board.Get(position);

                if (kind == TileKind.Empty)
                {
                    continue;
                }

                board.Set(position, TileKind.Empty);
                cleared.Add(new ClearedTile(kind, position));
            }

            cleared.Sort(CompareClearedTiles);
            return cleared;
        }

        public static List<TileMove> ApplyGravity(BoardState board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var moves = new List<TileMove>();

            for (int x = 0; x < board.Width; x++)
            {
                int writeY = 0;

                for (int readY = 0; readY < board.Height; readY++)
                {
                    var from = new BoardPosition(x, readY);
                    TileKind kind = board.Get(from);

                    if (kind == TileKind.Empty)
                    {
                        continue;
                    }

                    if (readY != writeY)
                    {
                        var to = new BoardPosition(x, writeY);

                        board.Set(to, kind);
                        board.Set(from, TileKind.Empty);
                        moves.Add(new TileMove(kind, from, to));
                    }

                    writeY++;
                }
            }

            return moves;
        }

        public static List<TileSpawn> Refill(
            BoardState board,
            ITileSource tileSource)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (tileSource == null)
            {
                throw new ArgumentNullException(nameof(tileSource));
            }

            var spawns = new List<TileSpawn>();

            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    var position = new BoardPosition(x, y);

                    if (board.Get(position) != TileKind.Empty)
                    {
                        continue;
                    }

                    TileKind kind = tileSource.NextTile(position);

                    if (kind == TileKind.Empty)
                    {
                        throw new InvalidOperationException(
                            "ITileSource returned TileKind.Empty during refill.");
                    }

                    board.Set(position, kind);
                    spawns.Add(new TileSpawn(kind, position));
                }
            }

            return spawns;
        }

        private static int CompareClearedTiles(
            ClearedTile left,
            ClearedTile right)
        {
            int byY = left.Position.Y.CompareTo(right.Position.Y);

            return byY != 0
                ? byY
                : left.Position.X.CompareTo(right.Position.X);
        }
    }
}
