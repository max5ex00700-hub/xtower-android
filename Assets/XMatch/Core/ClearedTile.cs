using System;

namespace XMatch.Core
{
    public readonly struct ClearedTile
    {
        public ClearedTile(TileKind kind, BoardPosition position)
        {
            if (kind == TileKind.Empty)
            {
                throw new ArgumentException(
                    "A cleared tile cannot be TileKind.Empty.",
                    nameof(kind));
            }

            Kind = kind;
            Position = position;
        }

        public TileKind Kind { get; }
        public BoardPosition Position { get; }
    }
}
