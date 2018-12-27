using System;

namespace Alligator.Gomoku
{
    public class Ply
    {
        public readonly int Row;
        public readonly int Column;

        public Ply(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public override int GetHashCode()
        {
            return Position.BoardSize * Row + Column;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            var move = obj as Ply;
            if (move == null)
            {
                return false;
            }
            return Row == move.Row && Column == move.Column;
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", Row, Column);
        }
    }
}