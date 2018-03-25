using System;
using System.Collections.Generic;

namespace Alligator.Gomoku.Evaluation
{
    public interface IPattern
    {
        PatternCounts Search(IList<Stone> line, out IList<Ply> tacticals);
    }
}
