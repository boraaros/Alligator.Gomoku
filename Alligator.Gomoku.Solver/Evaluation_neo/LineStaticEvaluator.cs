using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alligator.Gomoku.Evaluation_neo
{
    class LineStaticEvaluator
    {
        private IList<Pattern> patterns = new List<Pattern>
        {
            new Pattern("_XXXX_", 0, new int[] { 0, 5 }),

            new Pattern("_XXXXO", 0, new int[] { 0 }),
            new Pattern("X_XXXO", 0, new int[] { 1 }),
            new Pattern("XX_XXO", 0, new int[] { 2 }),
            new Pattern("XXX_XO", 0, new int[] { 3 }),
            new Pattern("XXXX_O", 0, new int[] { 4 }),
            new Pattern("OXXXX_", 0, new int[] { 5 }),

            new Pattern("_X_XX_", 0, new int[] { 0, 2, 5 }),
            new Pattern("_XX_X_", 0, new int[] { 0, 3, 5 }),

            new Pattern("__XXX__", 0, new int[] { 1, 5 }),
            new Pattern("O_XXX__", 0, new int[] { 5 }),
            new Pattern("__XXX_O", 0, new int[] { 1 }),
        };

        public int Evaluate(IList<StoneType> line, out ISet<int> tacticals)
        {
            var value = 0;
            tacticals = new HashSet<int>();

            var ownLine = ConvertFrom(line, 'X', 'O');
            foreach (var pattern in patterns)
            {
                foreach (var startIdx in Search(ownLine, pattern.Shape))
                {
                    value += pattern.Value;
                    foreach (var cm in pattern.CounterMoves)
                    {
                        tacticals.Add(startIdx + cm);
                    }
                }
            }

            var oppLine = ConvertFrom(line, 'O', 'X');
            foreach (var pattern in patterns)
            {
                foreach (var startIdx in Search(oppLine, pattern.Shape))
                {
                    value -= pattern.Value;
                    foreach (var cm in pattern.CounterMoves)
                    {
                        tacticals.Add(startIdx + cm);
                    }
                }
            }

            return value;
        }

        private IEnumerable<int> Search(string line, string pattern)
        {
            for (int i = 0; i <= line.Length - pattern.Length; i++)
            {
                int j;
                for (j = 0; j < pattern.Length; j++)
                {
                    if (line[i + j] != pattern[j])
                    {
                        break;
                    }
                }
                if (j == pattern.Length)
                {
                    yield return i + 1;
                }
            }
        }

        private string ConvertFrom(IList<StoneType> line, char own, char opp)
        {
            var result = "";
            result += opp;
            foreach (var stone in line)
            {
                switch (stone)
                {
                    case StoneType.Mine:
                        result += own;
                        break;
                    case StoneType.Opponents:
                        result += opp;
                        break;
                    case StoneType.Empty:
                        result += '_';
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            result += opp;
            return result;
        }
    }

    enum StoneType
    {
        Empty,
        Mine,
        Opponents
    }

    class Pattern
    {
        public string Shape;
        public int Value;
        public IList<int> CounterMoves;

        public Pattern(string shape, int value, IList<int> counterMoves)
        {
            Shape = shape;
            Value = value;
            CounterMoves = counterMoves;
        }
    }
}
