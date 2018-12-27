using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.Gomoku
{
    public interface IPosition : IPosition<Ply>
    {
        bool IsEmpty();

        Stone Next();

        Stone GetStone(int row, int column);

        IEnumerable<Ply> RelevantEmptyCells();

        int Score();

        int StoneCount { get; }

        IList<Ply> History { get; }

        Stone Winner { get; }
    }
}
