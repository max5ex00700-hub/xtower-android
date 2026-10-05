using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class BoardMoveFinder
    {
        public static bool HasLegalMove(BoardState board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var from = new BoardPosition(x, y);

                    if (x + 1 < board.Width)
                    {
                        var right = new BoardPosition(x + 1, y);

                        if (SwapLogic.WouldCreateMatch(board, from, right))
                        {
                            return true;
                        }
                    }

                    if (y + 1 < board.Height)
                    {
                        var up = new BoardPosition(x, y + 1);

                        if (SwapLogic.WouldCreateMatch(board, from, up))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static List<LegalMove> FindLegalMoves(BoardState board)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            var moves = new List<LegalMove>();

            for (int y = 0; y < board.Height; y++)
            {
                for (int x = 0; x < board.Width; x++)
                {
                    var from = new BoardPosition(x, y);

                    if (x + 1 < board.Width)
                    {
                        var right = new BoardPosition(x + 1, y);

                        if (SwapLogic.WouldCreateMatch(board, from, right))
                        {
                            moves.Add(new LegalMove(from, right));
                        }
                    }

                    if (y + 1 < board.Height)
                    {
                        var up = new BoardPosition(x, y + 1);

                        if (SwapLogic.WouldCreateMatch(board, from, up))
                        {
                            moves.Add(new LegalMove(from, up));
                        }
                    }
                }
            }

            return moves;
        }
    }
}
