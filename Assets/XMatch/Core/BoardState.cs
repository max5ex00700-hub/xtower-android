using System;

namespace XMatch.Core
{
    public sealed class BoardState
    {
        private readonly TileKind[,] cells;
        private readonly PowerUpKind[,] powerUps;

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
            powerUps = new PowerUpKind[width, height];
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

        public PowerUpKind GetPowerUp(BoardPosition position)
        {
            EnsureInside(position);
            return powerUps[position.X, position.Y];
        }

        public void Set(BoardPosition position, TileKind tile)
        {
            EnsureInside(position);
            cells[position.X, position.Y] = tile;

            if (tile == TileKind.Empty)
            {
                powerUps[position.X, position.Y] = PowerUpKind.None;
            }
        }

        public void SetPowerUp(
            BoardPosition position,
            PowerUpKind powerUp)
        {
            EnsureInside(position);

            if (cells[position.X, position.Y] == TileKind.Empty &&
                powerUp != PowerUpKind.None)
            {
                throw new InvalidOperationException(
                    "An empty cell cannot hold a power-up.");
            }

            powerUps[position.X, position.Y] = powerUp;
        }

        public void SetCell(
            BoardPosition position,
            TileKind tile,
            PowerUpKind powerUp)
        {
            Set(position, tile);
            SetPowerUp(position, powerUp);
        }

        public void Swap(BoardPosition a, BoardPosition b)
        {
            EnsureInside(a);
            EnsureInside(b);

            TileKind tempKind = cells[a.X, a.Y];
            cells[a.X, a.Y] = cells[b.X, b.Y];
            cells[b.X, b.Y] = tempKind;

            PowerUpKind tempPower = powerUps[a.X, a.Y];
            powerUps[a.X, a.Y] = powerUps[b.X, b.Y];
            powerUps[b.X, b.Y] = tempPower;
        }

        public BoardState Clone()
        {
            var clone = new BoardState(Width, Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    clone.cells[x, y] = cells[x, y];
                    clone.powerUps[x, y] = powerUps[x, y];
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
