using System;

namespace Alligator.Gomoku.Evaluation
{
    public class PatternSummary
    {
        public PatternCounts OpenPatternCounts;
        public PatternCounts ClosedPatternCounts;

        public PatternSummary(PatternCounts openPatternCounts, PatternCounts closedPatternCounts)
        {
            OpenPatternCounts = openPatternCounts;
            ClosedPatternCounts = closedPatternCounts;
        }

        public void Add(PatternSummary other)
        {
            OpenPatternCounts.Add(other.OpenPatternCounts);
            ClosedPatternCounts.Add(other.ClosedPatternCounts);
        }

        public void Subtract(PatternSummary other)
        {
            if (other == null)
            {
                return;
            }

            OpenPatternCounts.Subtract(other.OpenPatternCounts);
            ClosedPatternCounts.Subtract(other.ClosedPatternCounts);
        }
    }
}
