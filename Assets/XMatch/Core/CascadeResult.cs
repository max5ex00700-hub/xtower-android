using System;
using System.Collections.Generic;

namespace XMatch.Core
{
    public sealed class CascadeResult
    {
        public CascadeResult(IEnumerable<CascadeStep> steps)
        {
            if (steps == null)
            {
                throw new ArgumentNullException(nameof(steps));
            }

            var copy = new List<CascadeStep>(steps);
            Steps = copy.AsReadOnly();

            int totalCleared = 0;

            for (int i = 0; i < copy.Count; i++)
            {
                totalCleared += copy[i].Cleared.Count;
            }

            TotalCleared = totalCleared;
        }

        public IReadOnlyList<CascadeStep> Steps { get; }
        public int CascadeCount => Steps.Count;
        public int TotalCleared { get; }
        public bool IsStable => Steps.Count == 0;
    }
}
