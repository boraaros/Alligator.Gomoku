using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alligator.Gomoku.Evaluation
{
    public class PatternManager
    {
        private IDictionary<LineType, PatternSummary[]> result;

        private OpenPattern openPattern = new OpenPattern();
        private ClosedPattern closedPattern = new ClosedPattern();

        private PatternSummary patternSummary = new PatternSummary(new PatternCounts(), new PatternCounts());

        public PatternManager()
        {
            result = new Dictionary<LineType, PatternSummary[]>();

            result[LineType.Horizontal] = new PatternSummary[Position.BoardSize];
            result[LineType.Vertical] = new PatternSummary[Position.BoardSize];
            result[LineType.Diagonal] = new PatternSummary[2 * Position.BoardSize - 1];
            result[LineType.AntiDiagonal] = new PatternSummary[2 * Position.BoardSize - 1];
        }

        public void Update(IEnumerable<Stone> line, LineType lineType, int index)
        {
            var x = line.ToList();

            IList<Ply> tacticals;


            var openPatternCounts = openPattern.Search(x, out tacticals);

            if (openPatternCounts.BlackFour > 0 || openPatternCounts.WhiteFour > 0 
                || openPatternCounts.BlackThree > 0 || openPatternCounts.WhiteThree > 0)
            {
                //isTactical = true;
            }

            var closedPatternCounts = closedPattern.Search(x, out tacticals);

            if (closedPatternCounts.BlackFour > 0 || closedPatternCounts.WhiteFour > 0)
            {
                //isTactical = true;
            }

            patternSummary.Subtract(result[lineType][index]);
            result[lineType][index] = new PatternSummary(openPatternCounts, closedPatternCounts);
            patternSummary.Add(result[lineType][index]);
        }

        public int Evaluate(Stone next)
        {
            if (next == Stone.Black)
            {
                if (patternSummary.OpenPatternCounts.BlackFour > 0)
                {
                    return 1000000;
                }
                if (patternSummary.ClosedPatternCounts.BlackFour > 0)
                {
                    return 1000000;
                }
                if (patternSummary.OpenPatternCounts.WhiteFour > 0)
                {
                    return -1000000;
                }
                if (patternSummary.ClosedPatternCounts.WhiteFour > 1)
                {
                    return -1000000;
                }
            }
            else if (next == Stone.White)
            {
                if (patternSummary.OpenPatternCounts.WhiteFour > 0)
                {
                    return 1000000;
                }
                if (patternSummary.ClosedPatternCounts.WhiteFour > 0)
                {
                    return 1000000;
                }
                if (patternSummary.OpenPatternCounts.BlackFour > 0)
                {
                    return -1000000;
                }
                if (patternSummary.ClosedPatternCounts.BlackFour > 1)
                {
                    return -1000000;
                }
            }
            else
            {
                throw new Exception();
            }

            var blackSum = 0;

            blackSum += 1000000 * patternSummary.OpenPatternCounts.BlackFour;
            blackSum += 400 * patternSummary.OpenPatternCounts.BlackThree;
            blackSum += 50 * patternSummary.OpenPatternCounts.BlackPair;

            blackSum += 500 * patternSummary.ClosedPatternCounts.BlackFour;
            blackSum += 200 * patternSummary.ClosedPatternCounts.BlackThree;
            blackSum += 10 * patternSummary.ClosedPatternCounts.BlackPair;

            var whiteSum = 0;

            whiteSum += 1000000 * patternSummary.OpenPatternCounts.WhiteFour;
            whiteSum += 400 * patternSummary.OpenPatternCounts.WhiteThree;
            whiteSum += 50 * patternSummary.OpenPatternCounts.WhitePair;

            whiteSum += 500 * patternSummary.ClosedPatternCounts.WhiteFour;
            whiteSum += 200 * patternSummary.ClosedPatternCounts.WhiteThree;
            whiteSum += 10 * patternSummary.ClosedPatternCounts.WhitePair;

            return blackSum - whiteSum;
        }
    }
}
