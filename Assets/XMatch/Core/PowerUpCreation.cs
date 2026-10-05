using System;

namespace XMatch.Core
{
    public readonly struct PowerUpCreation
    {
        public PowerUpCreation(
            BoardPosition position,
            TileKind kind,
            PowerUpKind powerUp)
        {
            if (kind == TileKind.Empty)
            {
                throw new ArgumentException(
                    "A power-up tile cannot be empty.",
                    nameof(kind));
            }

            if (powerUp == PowerUpKind.None)
            {
                throw new ArgumentException(
                    "Power-up creation requires a non-empty power-up kind.",
                    nameof(powerUp));
            }

            Position = position;
            Kind = kind;
            PowerUp = powerUp;
        }

        public BoardPosition Position { get; }
        public TileKind Kind { get; }
        public PowerUpKind PowerUp { get; }
    }
}
