using System;

namespace XMatch.Core
{
    public readonly struct ClearedTile
    {
        public ClearedTile(
            TileKind kind,
            BoardPosition position)
            : this(
                kind,
                PowerUpKind.None,
                position)
        {
        }

        public ClearedTile(
            TileKind kind,
            PowerUpKind powerUp,
            BoardPosition position)
        {
            if (kind == TileKind.Empty)
            {
                throw new ArgumentException(
                    "A cleared tile cannot be TileKind.Empty.",
                    nameof(kind));
            }

            Kind = kind;
            PowerUp = powerUp;
            Position = position;
        }

        public TileKind Kind { get; }
        public PowerUpKind PowerUp { get; }
        public BoardPosition Position { get; }
    }
}
