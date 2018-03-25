using Alligator.Gomoku.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alligator.Gomoku
{
    public class Position : IPosition
    {
        private readonly Stone[,] board = new Stone[BoardSize, BoardSize];
        private Stone next = Stone.Black;
        private Stone winner;
        private IList<Ply> history;

        private const int HashParamsLength = 2 * BoardSize * BoardSize;
        private readonly IHashing hashing = new ZobristHashing(HashParamsLength);

        public const int BoardSize = 15;

        private int[,] relevantEmptyCells = new int[BoardSize, BoardSize];
        private const int radius = 2;

        private readonly PatternManager patternManager = new PatternManager();


        public Position()
        {
            history = new List<Ply>();
        }

        public Position(IList<Ply> history)
            : this()
        {
            foreach (var ply in history)
            {
                Do(ply);
            }
        }

        public IEnumerable<Ply> RelevantEmptyCells()
        {
            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (relevantEmptyCells[i, j] > 0)
                    {
                        yield return new Ply(i, j);
                    }
                }
            }
        }

        public int Score()
        {
            return patternManager.Evaluate(next);
        }

        public void Do(Ply ply)
        {
            if (ply == null)
            {
                throw new ArgumentNullException("ply");
            }
            if (IsEnded)
            {
                throw new InvalidOperationException("Position is closed");
            }
            if (HasWinner)
            {
                throw new InvalidOperationException(string.Format("Position has winner, but isn't closed"));
            }
            if (board[ply.Row, ply.Column] != Stone.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot do ply, because target cell isn't empty: [{0},{1}]",
                    ply.Row, ply.Column));
            }
            board[ply.Row, ply.Column] = next;
            hashing.Modify(2 * (BoardSize * ply.Row + ply.Column) + (next == Stone.Black ? 0 : 1));
            history.Add(ply);
            
            if (IsWinningMove(ply))
            {
                winner = next;
                ChangeNext();
            }
            else
            {
                ChangeNext();
                UpdateScore(ply.Row, ply.Column);
            }
            

            for (int i = Math.Max(0, ply.Row - radius); i < Math.Min(BoardSize, ply.Row + radius + 1); i++)
            {
                for (int j = Math.Max(0, ply.Column - radius); j < Math.Min(BoardSize, ply.Column + radius + 1); j++)
                {
                    if (GetStone(i, j) == 0 && relevantEmptyCells[i, j] == 0)
                    {
                        relevantEmptyCells[i, j] = history.Count;
                    }
                }
            }
            relevantEmptyCells[ply.Row, ply.Column] *= -1;

        }

        private void UpdateScore(int i, int j)
        {
            var list1 = Enumerate(new Ply(i, 0), Direction.Right).ToList();
            list1.Insert(0, board[i, 0]);

            patternManager.Update(list1, LineType.Horizontal, i);

            var list2 = Enumerate(new Ply(0, j), Direction.Down).ToList();
            list2.Insert(0, board[0, j]);

            patternManager.Update(list2, LineType.Vertical, j);

            if (i > j)
            {
                var list3 = Enumerate(new Ply(i - j, 0), Direction.DownRight).ToList();
                list3.Insert(0, board[i - j, 0]);

                patternManager.Update(list3, LineType.Diagonal, Position.BoardSize - 1 + i - j);
            }
            else
            {
                var list4 = Enumerate(new Ply(0, j - i), Direction.DownRight).ToList();
                list4.Insert(0, board[0, j - i]);

                patternManager.Update(list4, LineType.Diagonal, Position.BoardSize - 1 + i - j);
            }

            if (i + j < Position.BoardSize)
            {
                var list4 = Enumerate(new Ply(i + j, 0), Direction.UpRight).ToList();
                list4.Insert(0, board[i + j, 0]);

                patternManager.Update(list4, LineType.AntiDiagonal, i + j);
            }
            else
            {
                var list5 = Enumerate(new Ply(Position.BoardSize - 1, i + j - (Position.BoardSize - 1)), Direction.UpRight).ToList();
                list5.Insert(0, board[Position.BoardSize - 1, i + j - (Position.BoardSize - 1)]);

                patternManager.Update(list5, LineType.AntiDiagonal, i + j);
            }  
        }

        public void Undo()
        {
            if (history.Count == 0)
            {
                throw new InvalidOperationException("Cannot undo ply from start position");
            }

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    if (relevantEmptyCells[i, j] == history.Count)
                    {
                        relevantEmptyCells[i, j] = 0;
                    }
                }
            }
            relevantEmptyCells[history[history.Count - 1].Row, history[history.Count - 1].Column] *= -1;
        


            var ply = history.Last();
            if (board[ply.Row, ply.Column] == Stone.Empty)
            {
                throw new InvalidOperationException(string.Format("Cannot undo ply, because target cell is already empty: [{0},{1}]",
                    ply.Row, ply.Column));
            }
            board[ply.Row, ply.Column] = Stone.Empty;  
            history.Remove(ply);
            ChangeNext();
            hashing.Modify(2 * (BoardSize * ply.Row + ply.Column) + (next == Stone.Black ? 0 : 1));

            if (winner != Stone.Empty)
            {
                winner = Stone.Empty;
            }
            else
            {
                UpdateScore(ply.Row, ply.Column);
            }
        }

        public bool HasWinner
        {
            get { return winner != Stone.Empty; }
        }

        public ulong Identifier
        {
            get { return hashing.HashCode; }
        }

        public bool IsEnded
        {
            get { return HasWinner || IsFullBoard(); }
        }

        private bool IsFullBoard()
        {
            return history.Count == BoardSize * BoardSize;
        }

        public object Clone()
        {
            return new Position(history);
        }

        public Stone GetStone(int row, int column)
        {
            return board[row, column];
        }

        private bool IsWinningMove(Ply cell)
        {
            var cellValue = board[cell.Row, cell.Column];
            int verticalCount = 1;
            foreach (var value in Enumerate(cell, Direction.Up))
            {
                if (value != cellValue) break;
                verticalCount++;
            }
            foreach (var value in Enumerate(cell, Direction.Down))
            {
                if (value != cellValue) break;
                verticalCount++;
            }
            if (verticalCount >= 5)
            {
                return true;
            }
            int horizontalCount = 1;
            foreach (var value in Enumerate(cell, Direction.Left))
            {
                if (value != cellValue) break;
                horizontalCount++;
            }
            foreach (var value in Enumerate(cell, Direction.Right))
            {
                if (value != cellValue) break;
                horizontalCount++;
            }
            if (horizontalCount >= 5)
            {
                return true;
            }
            int diagonalCount = 1;
            foreach (var value in Enumerate(cell, Direction.UpLeft))
            {
                if (value != cellValue) break;
                diagonalCount++;
            }
            foreach (var value in Enumerate(cell, Direction.DownRight))
            {
                if (value != cellValue) break;
                diagonalCount++;
            }
            if (diagonalCount >= 5)
            {
                return true;
            }
            int reverseDiagonalCount = 1;
            foreach (var value in Enumerate(cell, Direction.DownLeft))
            {
                if (value != cellValue) break;
                reverseDiagonalCount++;
            }
            foreach (var value in Enumerate(cell, Direction.UpRight))
            {
                if (value != cellValue) break;
                reverseDiagonalCount++;
            }
            if (reverseDiagonalCount >= 5)
            {
                return true;
            }
            return false;
        }

        public IEnumerable<Stone> Enumerate(Ply from, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    for (int i = from.Row - 1; i >= 0; i--)
                    {
                        yield return board[i, from.Column];
                    }
                    yield break;
                case Direction.Down:
                    for (int i = from.Row + 1; i < BoardSize; i++)
                    {
                        yield return board[i, from.Column];
                    }
                    yield break;
                case Direction.Left:
                    for (int i = from.Column - 1; i >= 0; i--)
                    {
                        yield return board[from.Row, i];
                    }
                    yield break;
                case Direction.Right:
                    for (int i = from.Column + 1; i < BoardSize; i++)
                    {
                        yield return board[from.Row, i];
                    }
                    yield break;
                case Direction.UpLeft:
                    for (int i = 1; i <= Math.Min(from.Row, from.Column); i++)
                    {
                        yield return board[from.Row - i, from.Column - i];
                    }
                    yield break;
                case Direction.DownRight:
                    for (int i = 1; i < Math.Min(BoardSize - from.Row, BoardSize - from.Column); i++)
                    {
                        yield return board[from.Row + i, from.Column + i];
                    }
                    yield break;
                case Direction.DownLeft:
                    for (int i = 1; i < Math.Min(BoardSize - from.Row, from.Column + 1); i++)
                    {
                        yield return board[from.Row + i, from.Column - i];
                    }
                    yield break;
                case Direction.UpRight:
                    for (int i = 1; i < Math.Min(from.Row + 1, BoardSize - from.Column); i++)
                    {
                        yield return board[from.Row - i, from.Column + i];
                    }
                    yield break;
                default: throw new InvalidOperationException(string.Format("Unknown direction: {0}!", direction));
            }
        }

        private void ChangeNext()
        {
            next = 3 - next;
        }

        public bool IsEmpty()
        {
            return history.Count == 0;
        }


        public Stone Next()
        {
            return next;
        }


        public int StoneCount
        {
            get { return history.Count; }
        }

        public bool IsQuiet => true;

        public IList<Ply> History => history;
    }
}
