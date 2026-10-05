namespace XMatch.Core
{
    public readonly struct TileMove
    {
        public TileMove(
            TileKind kind,
            BoardPosition from,
            BoardPosition to)
        {
            Kind = kind;
            From = from;
            To = to;
        }

        public TileKind Kind { get; }
        public BoardPosition From { get; }
        public BoardPosition To { get; }
    }

    public readonly struct TileSpawn
    {
        public TileSpawn(TileKind kind, BoardPosition position)
        {
            Kind = kind;
            Position = position;
        }

        public TileKind Kind { get; }
        public BoardPosition Position { get; }
    }
}
