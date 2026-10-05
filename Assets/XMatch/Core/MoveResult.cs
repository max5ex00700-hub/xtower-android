using System;

namespace XMatch.Core
{
    public sealed class MoveResult
    {
        private MoveResult(
            bool accepted,
            BoardPosition from,
            BoardPosition to,
            CascadeResult cascades)
        {
            Accepted = accepted;
            From = from;
            To = to;
            Cascades = cascades ?? throw new ArgumentNullException(nameof(cascades));
        }

        public bool Accepted { get; }
        public BoardPosition From { get; }
        public BoardPosition To { get; }
        public CascadeResult Cascades { get; }

        public static MoveResult Rejected(
            BoardPosition from,
            BoardPosition to)
        {
            return new MoveResult(
                false,
                from,
                to,
                new CascadeResult(Array.Empty<CascadeStep>()));
        }

        public static MoveResult AcceptedMove(
            BoardPosition from,
            BoardPosition to,
            CascadeResult cascades)
        {
            return new MoveResult(true, from, to, cascades);
        }
    }
}
