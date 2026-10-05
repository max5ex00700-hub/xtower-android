using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public static class LevelValidator
    {
        public static List<string> Validate(LevelDefinition level)
        {
            if (level == null)
            {
                throw new ArgumentNullException(nameof(level));
            }

            var errors = new List<string>();
            BoardState board = level.CreateBoard();

            if (MatchFinder.FindMatches(board).Count > 0)
            {
                errors.Add(
                    "Initial board contains one or more automatic matches.");
            }

            if (!BoardMoveFinder.HasLegalMove(board))
            {
                errors.Add(
                    "Initial board has no legal player move.");
            }

            return errors;
        }

        public static void ValidateOrThrow(LevelDefinition level)
        {
            List<string> errors = Validate(level);

            if (errors.Count == 0)
            {
                return;
            }

            throw new ArgumentException(
                "Invalid level definition: " +
                string.Join(" ", errors),
                nameof(level));
        }
    }
}
