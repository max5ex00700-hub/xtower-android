using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class MatchFinder
    {
        public static HashSet<BoardPosition> FindMatches(BoardState board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var matches = new HashSet<BoardPosition>();

            FindHorizontal(board, matches);
            FindVertical(board, matches);
            FindSquares(board, matches);

            return matches;
        }

        private static void FindHorizontal(
            BoardState board,
            HashSet<BoardPosition> matches)
        {
            for (int y = 0; y < board.Height; y++)
            {
                int runStart = 0;

                while (runStart < board.Width)
                {
                    var start = new BoardPosition(runStart, y);
                    TileKind kind = board.Get(start);

                    if (!IsMatchable(kind))
                    {
                        runStart++;
                        continue;
                    }

                    int runEnd = runStart + 1;

                    while (runEnd < board.Width &&
                           board.Get(new BoardPosition(runEnd, y)) == kind)
                    {
                        runEnd++;
                    }

                    int runLength = runEnd - runStart;

                    if (runLength >= 3)
                    {
                        for (int x = runStart; x < runEnd; x++)
                        {
                            matches.Add(new BoardPosition(x, y));
                        }
                    }

                    runStart = runEnd;
                }
            }
        }

        private static void FindVertical(
            BoardState board,
            HashSet<BoardPosition> matches)
        {
            for (int x = 0; x < board.Width; x++)
            {
                int runStart = 0;

                while (runStart < board.Height)
                {
                    var start = new BoardPosition(x, runStart);
                    TileKind kind = board.Get(start);

                    if (!IsMatchable(kind))
                    {
                        runStart++;
                        continue;
                    }

                    int runEnd = runStart + 1;

                    while (runEnd < board.Height &&
                           board.Get(new BoardPosition(x, runEnd)) == kind)
                    {
                        runEnd++;
                    }

                    int runLength = runEnd - runStart;

                    if (runLength >= 3)
                    {
                        for (int y = runStart; y < runEnd; y++)
                        {
                            matches.Add(new BoardPosition(x, y));
                        }
                    }

                    runStart = runEnd;
                }
            }
        }

        private static void FindSquares(
            BoardState board,
            HashSet<BoardPosition> matches)
        {
            for (int y = 0; y + 1 < board.Height; y++)
            {
                for (int x = 0; x + 1 < board.Width; x++)
                {
                    var a = new BoardPosition(x, y);
                    TileKind kind = board.Get(a);

                    if (!IsMatchable(kind))
                    {
                        continue;
                    }

                    var b = new BoardPosition(x + 1, y);
                    var c = new BoardPosition(x, y + 1);
                    var d = new BoardPosition(x + 1, y + 1);

                    if (board.Get(b) == kind &&
                        board.Get(c) == kind &&
                        board.Get(d) == kind)
                    {
                        matches.Add(a);
                        matches.Add(b);
                        matches.Add(c);
                        matches.Add(d);
                    }
                }
            }
        }

        public static bool IsMatchable(TileKind kind)
        {
            return kind != TileKind.Empty &&
                   kind != TileKind.Wild;
        }
    }
}
