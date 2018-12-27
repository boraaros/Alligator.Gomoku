using Alligator.Solver;
using System;
using System.Collections.Generic;

namespace Alligator.Gomoku
{
    public class Rules : IRules<IPosition, Ply>
    {
        public IPosition InitialPosition()
        {
            return new Position();
        }

        public IEnumerable<Ply> LegalMovesAt(IPosition position)
        {
            if (position.IsEmpty())
            {
                return new List<Ply> { new Ply(Position.BoardSize / 2, Position.BoardSize / 2) };
            }

            if (IsGoal(position))
            {
                return new List<Ply>();
            }

            return position.RelevantEmptyCells();
        }

        public bool IsGoal(IPosition position)
        {
            return position.Winner != Stone.Empty;
        }
    }
}