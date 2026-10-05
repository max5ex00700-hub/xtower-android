namespace XMatch.Core
{
    public readonly struct LegalMove
    {
        public LegalMove(BoardPosition from, BoardPosition to)
        {
            From = from;
            To = to;
        }

        public BoardPosition From { get; }
        public BoardPosition To { get; }

        public override string ToString()
        {
            return $"{From} -> {To}";
        }
    }
}
