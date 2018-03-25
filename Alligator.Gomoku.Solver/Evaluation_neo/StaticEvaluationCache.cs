using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alligator.Gomoku.Evaluation_neo
{
    class StaticEvaluationCache
    {
        private LineStaticEvaluator lse;

        private IDictionary<int, PatternResult> cache;

        public StaticEvaluationCache()
        {
            lse = new LineStaticEvaluator();
            cache = new Dictionary<int, PatternResult>();
        }

        public void Build()
        {

        }

        private IEnumerable<string> AllCombinations(string list)
        {
            yield break;
        }

    }

    class PatternResult
    {
        public int Value;
        public IList<int> CounterMoves;

        public PatternResult(int value, IList<int> counterMoves)
        {
            Value = value;
            CounterMoves = counterMoves;
        }
    }
}
