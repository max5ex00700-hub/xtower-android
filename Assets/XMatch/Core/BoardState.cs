using System;

namespace XMatch.Core
{
    public sealed class BoardState
    {
        private readonly TileKind[,] cells;

        public BoardState(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width));
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height));
            }

            Width = width;
            Height = height;
            cells = new TileKind[width, height];
        }

        public int Width { get; }
        public int Height { get; }

        public bool IsInside(BoardPosition position)
        {
            return position.X >= 0 &&
                   position.X < Width &&
                   position.Y >= 0 &&
                   position.Y < Height;
        }

        public TileKind Get(BoardPosition position)
        {
            EnsureInside(position);
            return cells[position.X, position.Y];
        }

        public void Set(BoardPosition position, TileKind tile)
        {
            EnsureInside(position);
            cells[position.X, position.Y] = tile;
        }

        public void Swap(BoardPosition a, BoardPosition b)
        {
            EnsureInside(a);
            EnsureInside(b);

            TileKind temp = cells[a.X, a.Y];
            cells[a.X, a.Y] = cells[b.X, b.Y];
            cells[b.X, b.Y] = temp;
        }

        public BoardState Clone()
        {
            var clone = new BoardState(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    clone.cells[x, y] = cells[x, y];
                }
            }

            return clone;
        }

        private void EnsureInside(BoardPosition position)
        {
            if (!IsInside(position))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(position),
                    $"Position {position} is outside a {Width}x{Height} board.");
            }
        }
    }
}
