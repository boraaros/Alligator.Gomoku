using Alligator.Solver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alligator.Gomoku
{
    public class SolverHelper : IExternalLogics<IPosition, Ply>
    {
        private readonly Stone own;

        public SolverHelper(Stone own)
        {
            this.own = own;
        }

        public IPosition CreateEmptyPosition()
        {
            return new Position();
        }

        public int StaticEvaluate(IPosition position)
        {
            if (own == Stone.Black)
            {
                return position.Score();
            }

            return -position.Score();
            
        }

        public IEnumerable<Ply> GetStrategiesFrom(IPosition position)
        {
            if (position.IsEmpty())
            {
                return new List<Ply> { new Ply(Position.BoardSize / 2, Position.BoardSize / 2) };
            }

            var x = position.RelevantEmptyCells();

            //if (position.StoneCount == 8)
            //{
            //    //return x.Where(t => (t.Row != 4 && t.Row != 5) || t.Column != 1);
            //    return x.Where(t => t.Row == 0 && t.Column == 0);
            //}

            //if (position.StoneCount == 9)
            //{
            //    return x.Where(t => (t.Row == 5 && t.Column == 1) || (t.Row == 1 && t.Column == 1));
            //    //return x.Where(t => t.Column == 1);
            //}

            //if (position.StoneCount == 8)
            //{
            //    //return x.Where(t => (t.Row != 4 && t.Row != 5) || t.Column != 1);
            //    return x.Where(t => t.Row == 0 && t.Column == 2);
            //}

            ////if (position.StoneCount == 11)
            ////{
            ////    return x.Where(t => t.Row == 4 && t.Column == 1);
            ////    //return x.Where(t => t.Row == 0 && t.Column == 0);
            ////}




            return x;
        }
    }
}
