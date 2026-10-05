using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class PowerUpResolver
    {
        public static SpecialComboKind GetComboKind(
            PowerUpKind a,
            PowerUpKind b)
        {
            if (a == PowerUpKind.None ||
                b == PowerUpKind.None)
            {
                return SpecialComboKind.None;
            }

            if (a == PowerUpKind.ColorOrb &&
                b == PowerUpKind.ColorOrb)
            {
                return SpecialComboKind.DoubleOrb;
            }

            if (a == PowerUpKind.ColorOrb ||
                b == PowerUpKind.ColorOrb)
            {
                PowerUpKind other =
                    a == PowerUpKind.ColorOrb
                        ? b
                        : a;

                switch (other)
                {
                    case PowerUpKind.RowBlast:
                        return SpecialComboKind.OrbRow;
                    case PowerUpKind.ColumnBlast:
                        return SpecialComboKind.OrbColumn;
                    case PowerUpKind.Bomb:
                        return SpecialComboKind.OrbBomb;
                    case PowerUpKind.Seeker:
                        return SpecialComboKind.OrbSeeker;
                }
            }

            if (a == PowerUpKind.RowBlast &&
                b == PowerUpKind.RowBlast)
            {
                return SpecialComboKind.DoubleRow;
            }

            if (a == PowerUpKind.ColumnBlast &&
                b == PowerUpKind.ColumnBlast)
            {
                return SpecialComboKind.DoubleColumn;
            }

            if ((a == PowerUpKind.RowBlast &&
                 b == PowerUpKind.ColumnBlast) ||
                (a == PowerUpKind.ColumnBlast &&
                 b == PowerUpKind.RowBlast))
            {
                return SpecialComboKind.CrossBlast;
            }

            if (a == PowerUpKind.Bomb &&
                b == PowerUpKind.Bomb)
            {
                return SpecialComboKind.DoubleBomb;
            }

            if ((a == PowerUpKind.Bomb &&
                 b == PowerUpKind.RowBlast) ||
                (b == PowerUpKind.Bomb &&
                 a == PowerUpKind.RowBlast))
            {
                return SpecialComboKind.RowBomb;
            }

            if ((a == PowerUpKind.Bomb &&
                 b == PowerUpKind.ColumnBlast) ||
                (b == PowerUpKind.Bomb &&
                 a == PowerUpKind.ColumnBlast))
            {
                return SpecialComboKind.ColumnBomb;
            }

            if (a == PowerUpKind.Seeker &&
                b == PowerUpKind.Seeker)
            {
                return SpecialComboKind.SeekerPair;
            }

            if (a == PowerUpKind.Seeker ||
                b == PowerUpKind.Seeker)
            {
                return SpecialComboKind.SeekerWithSpecial;
            }

            return SpecialComboKind.None;
        }

        public static void ExpandTriggeredPowerUps(
            BoardState board,
            HashSet<BoardPosition> clearSet)
        {
            ExpandTriggeredPowerUps(
                board,
                clearSet,
                suppressed: null);
        }

        private static void ExpandTriggeredPowerUps(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            HashSet<BoardPosition> suppressed)
        {
            if (board == null)
            {
                throw new ArgumentNullException(nameof(board));
            }

            if (clearSet == null)
            {
                throw new ArgumentNullException(nameof(clearSet));
            }

            var queue =
                new Queue<BoardPosition>(clearSet);

            while (queue.Count > 0)
            {
                BoardPosition position =
                    queue.Dequeue();

                if (!board.IsInside(position) ||
                    board.Get(position) == TileKind.Empty ||
                    (suppressed != null &&
                     suppressed.Contains(position)))
                {
                    continue;
                }

                PowerUpKind power =
                    board.GetPowerUp(position);

                switch (power)
                {
                    case PowerUpKind.RowBlast:
                        AddRow(
                            board,
                            clearSet,
                            queue,
                            position.Y);
                        break;

                    case PowerUpKind.ColumnBlast:
                        AddColumn(
                            board,
                            clearSet,
                            queue,
                            position.X);
                        break;

                    case PowerUpKind.Bomb:
                        AddBomb(
                            board,
                            clearSet,
                            queue,
                            position,
                            1);
                        break;

                    case PowerUpKind.ColorOrb:
                        AddMostCommonColor(
                            board,
                            clearSet,
                            queue);
                        break;

                    case PowerUpKind.Seeker:
                        AddSeekerTarget(
                            board,
                            clearSet,
                            queue,
                            position);
                        break;
                }
            }
        }

        public static HashSet<BoardPosition> CreatePowerUpSwapClear(
            BoardState board,
            BoardPosition from,
            BoardPosition to,
            TileKind fromKindBeforeSwap,
            TileKind toKindBeforeSwap,
            PowerUpKind fromPowerBeforeSwap,
            PowerUpKind toPowerBeforeSwap)
        {
            var clearSet =
                new HashSet<BoardPosition>
                {
                    from,
                    to
                };

            bool fromOrb =
                fromPowerBeforeSwap ==
                PowerUpKind.ColorOrb;
            bool toOrb =
                toPowerBeforeSwap ==
                PowerUpKind.ColorOrb;

            if (fromOrb && toOrb)
            {
                AddAll(board, clearSet);
                return clearSet;
            }

            if (fromOrb || toOrb)
            {
                TileKind targetKind =
                    fromOrb
                        ? toKindBeforeSwap
                        : fromKindBeforeSwap;

                PowerUpKind pairedPower =
                    fromOrb
                        ? toPowerBeforeSwap
                        : fromPowerBeforeSwap;

                if (MatchFinder.IsMatchable(
                        targetKind))
                {
                    if (pairedPower ==
                        PowerUpKind.None)
                    {
                        AddColor(
                            board,
                            clearSet,
                            targetKind);
                    }
                    else
                    {
                        AddOrbSpecialPattern(
                            board,
                            clearSet,
                            targetKind,
                            pairedPower);
                    }
                }

                var suppressed =
                    new HashSet<BoardPosition>();

                if (fromOrb)
                {
                    suppressed.Add(to);
                }

                if (toOrb)
                {
                    suppressed.Add(from);
                }

                ExpandTriggeredPowerUps(
                    board,
                    clearSet,
                    suppressed);

                return clearSet;
            }

            SpecialComboKind combo =
                GetComboKind(
                    fromPowerBeforeSwap,
                    toPowerBeforeSwap);

            if (combo != SpecialComboKind.None)
            {
                ApplySpecialCombo(
                    board,
                    clearSet,
                    from,
                    to,
                    combo,
                    fromPowerBeforeSwap,
                    toPowerBeforeSwap);
            }

            ExpandTriggeredPowerUps(
                board,
                clearSet);

            return clearSet;
        }

        private static void ApplySpecialCombo(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            BoardPosition from,
            BoardPosition to,
            SpecialComboKind combo,
            PowerUpKind fromPower,
            PowerUpKind toPower)
        {
            var queue =
                new Queue<BoardPosition>();

            switch (combo)
            {
                case SpecialComboKind.DoubleRow:
                    AddRow(
                        board,
                        clearSet,
                        queue,
                        from.Y);
                    AddRow(
                        board,
                        clearSet,
                        queue,
                        to.Y);
                    break;

                case SpecialComboKind.DoubleColumn:
                    AddColumn(
                        board,
                        clearSet,
                        queue,
                        from.X);
                    AddColumn(
                        board,
                        clearSet,
                        queue,
                        to.X);
                    break;

                case SpecialComboKind.CrossBlast:
                    AddRow(
                        board,
                        clearSet,
                        queue,
                        from.Y);
                    AddRow(
                        board,
                        clearSet,
                        queue,
                        to.Y);
                    AddColumn(
                        board,
                        clearSet,
                        queue,
                        from.X);
                    AddColumn(
                        board,
                        clearSet,
                        queue,
                        to.X);
                    break;

                case SpecialComboKind.RowBomb:
                {
                    int centerY =
                        (from.Y + to.Y) / 2;

                    for (int dy = -1;
                         dy <= 1;
                         dy++)
                    {
                        int y = centerY + dy;

                        if (y >= 0 &&
                            y < board.Height)
                        {
                            AddRow(
                                board,
                                clearSet,
                                queue,
                                y);
                        }
                    }
                    break;
                }

                case SpecialComboKind.ColumnBomb:
                {
                    int centerX =
                        (from.X + to.X) / 2;

                    for (int dx = -1;
                         dx <= 1;
                         dx++)
                    {
                        int x = centerX + dx;

                        if (x >= 0 &&
                            x < board.Width)
                        {
                            AddColumn(
                                board,
                                clearSet,
                                queue,
                                x);
                        }
                    }
                    break;
                }

                case SpecialComboKind.DoubleBomb:
                    AddBomb(
                        board,
                        clearSet,
                        queue,
                        from,
                        2);
                    AddBomb(
                        board,
                        clearSet,
                        queue,
                        to,
                        2);
                    break;

                case SpecialComboKind.SeekerPair:
                    AddFirstTargets(
                        board,
                        clearSet,
                        6);
                    break;

                case SpecialComboKind.SeekerWithSpecial:
                {
                    PowerUpKind other =
                        fromPower ==
                        PowerUpKind.Seeker
                            ? toPower
                            : fromPower;

                    ApplySinglePowerPattern(
                        board,
                        clearSet,
                        queue,
                        to,
                        other);

                    AddFirstTargets(
                        board,
                        clearSet,
                        3);
                    break;
                }
            }
        }

        private static void AddOrbSpecialPattern(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            TileKind targetKind,
            PowerUpKind pairedPower)
        {
            var queue =
                new Queue<BoardPosition>();

            var matching =
                new List<BoardPosition>();

            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                for (int x = 0;
                     x < board.Width;
                     x++)
                {
                    var position =
                        new BoardPosition(x, y);

                    if (board.Get(position) ==
                        targetKind)
                    {
                        matching.Add(position);
                    }
                }
            }

            for (int i = 0;
                 i < matching.Count;
                 i++)
            {
                BoardPosition position =
                    matching[i];

                Add(
                    board,
                    clearSet,
                    queue,
                    position);

                ApplySinglePowerPattern(
                    board,
                    clearSet,
                    queue,
                    position,
                    pairedPower);
            }
        }

        private static void ApplySinglePowerPattern(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition position,
            PowerUpKind power)
        {
            switch (power)
            {
                case PowerUpKind.RowBlast:
                    AddRow(
                        board,
                        clearSet,
                        queue,
                        position.Y);
                    break;

                case PowerUpKind.ColumnBlast:
                    AddColumn(
                        board,
                        clearSet,
                        queue,
                        position.X);
                    break;

                case PowerUpKind.Bomb:
                    AddBomb(
                        board,
                        clearSet,
                        queue,
                        position,
                        1);
                    break;

                case PowerUpKind.Seeker:
                    AddSeekerTarget(
                        board,
                        clearSet,
                        queue,
                        position);
                    break;
            }
        }

        private static void AddFirstTargets(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            int count)
        {
            for (int y = 0;
                 y < board.Height &&
                 count > 0;
                 y++)
            {
                for (int x = 0;
                     x < board.Width &&
                     count > 0;
                     x++)
                {
                    var position =
                        new BoardPosition(x, y);

                    if (clearSet.Contains(position) ||
                        board.Get(position) ==
                        TileKind.Empty)
                    {
                        continue;
                    }

                    clearSet.Add(position);
                    count--;
                }
            }
        }

        private static void AddRow(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            int y)
        {
            for (int x = 0;
                 x < board.Width;
                 x++)
            {
                Add(
                    board,
                    clearSet,
                    queue,
                    new BoardPosition(x, y));
            }
        }

        private static void AddColumn(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            int x)
        {
            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                Add(
                    board,
                    clearSet,
                    queue,
                    new BoardPosition(x, y));
            }
        }

        private static void AddBomb(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition center,
            int radius)
        {
            for (int y =
                     center.Y - radius;
                 y <= center.Y + radius;
                 y++)
            {
                for (int x =
                         center.X - radius;
                     x <= center.X + radius;
                     x++)
                {
                    Add(
                        board,
                        clearSet,
                        queue,
                        new BoardPosition(x, y));
                }
            }
        }

        private static void AddMostCommonColor(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue)
        {
            var counts =
                new Dictionary<TileKind, int>();

            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                for (int x = 0;
                     x < board.Width;
                     x++)
                {
                    TileKind kind =
                        board.Get(
                            new BoardPosition(
                                x,
                                y));

                    if (!MatchFinder.IsMatchable(
                            kind))
                    {
                        continue;
                    }

                    int count;
                    counts.TryGetValue(
                        kind,
                        out count);
                    counts[kind] =
                        count + 1;
                }
            }

            TileKind bestKind =
                TileKind.Empty;
            int bestCount = 0;

            foreach (
                KeyValuePair<TileKind, int> pair
                in counts)
            {
                if (pair.Value > bestCount ||
                    (pair.Value == bestCount &&
                     (int)pair.Key <
                     (int)bestKind))
                {
                    bestKind = pair.Key;
                    bestCount = pair.Value;
                }
            }

            if (bestKind !=
                TileKind.Empty)
            {
                AddColor(
                    board,
                    clearSet,
                    queue,
                    bestKind);
            }
        }

        private static void AddColor(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            TileKind kind)
        {
            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                for (int x = 0;
                     x < board.Width;
                     x++)
                {
                    var position =
                        new BoardPosition(x, y);

                    if (board.Get(position) ==
                        kind)
                    {
                        clearSet.Add(position);
                    }
                }
            }
        }

        private static void AddColor(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            TileKind kind)
        {
            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                for (int x = 0;
                     x < board.Width;
                     x++)
                {
                    var position =
                        new BoardPosition(x, y);

                    if (board.Get(position) ==
                        kind)
                    {
                        Add(
                            board,
                            clearSet,
                            queue,
                            position);
                    }
                }
            }
        }

        private static void AddSeekerTarget(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition origin)
        {
            BoardPosition? best = null;
            int bestDistance =
                int.MaxValue;

            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                for (int x = 0;
                     x < board.Width;
                     x++)
                {
                    var candidate =
                        new BoardPosition(x, y);

                    if (clearSet.Contains(
                            candidate) ||
                        board.Get(candidate) ==
                        TileKind.Empty)
                    {
                        continue;
                    }

                    int distance =
                        Math.Abs(
                            candidate.X -
                            origin.X) +
                        Math.Abs(
                            candidate.Y -
                            origin.Y);

                    if (distance <
                        bestDistance)
                    {
                        best = candidate;
                        bestDistance =
                            distance;
                    }
                }
            }

            if (best.HasValue)
            {
                Add(
                    board,
                    clearSet,
                    queue,
                    best.Value);
            }
        }

        private static void AddAll(
            BoardState board,
            HashSet<BoardPosition> clearSet)
        {
            for (int y = 0;
                 y < board.Height;
                 y++)
            {
                for (int x = 0;
                     x < board.Width;
                     x++)
                {
                    var position =
                        new BoardPosition(x, y);

                    if (board.Get(position) !=
                        TileKind.Empty)
                    {
                        clearSet.Add(position);
                    }
                }
            }
        }

        private static void Add(
            BoardState board,
            HashSet<BoardPosition> clearSet,
            Queue<BoardPosition> queue,
            BoardPosition position)
        {
            if (!board.IsInside(position) ||
                board.Get(position) ==
                TileKind.Empty)
            {
                return;
            }

            if (clearSet.Add(position))
            {
                queue.Enqueue(position);
            }
        }
    }
}
