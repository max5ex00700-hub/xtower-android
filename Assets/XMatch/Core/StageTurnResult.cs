using System;

namespace XMatch.Core
{
    public sealed class StageTurnResult
    {
        public StageTurnResult(
            MoveResult move,
            int movesRemaining,
            StageStatus status,
            bool needsShuffle)
        {
            Move = move ??
                throw new ArgumentNullException(nameof(move));

            MovesRemaining = movesRemaining;
            Status = status;
            NeedsShuffle = needsShuffle;
        }

        public MoveResult Move { get; }
        public bool Accepted => Move.Accepted;
        public int MovesRemaining { get; }
        public StageStatus Status { get; }
        public bool NeedsShuffle { get; }
    }
}
