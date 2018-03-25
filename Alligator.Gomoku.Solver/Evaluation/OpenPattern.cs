using System;
using System.Collections.Generic;

namespace Alligator.Gomoku.Evaluation
{
    public class OpenPattern : IPattern
    {
        private const int Length = 4;

        public PatternCounts Search(IList<Stone> line, out IList<Ply> tacticals)
        {
            var result = new PatternCounts();
            tacticals = new List<Ply>();

            for (int i = 0; i < line.Count - Length - 1; i++)
            {
                if (line[i] != Stone.Empty || line[i + Length + 1] != Stone.Empty)
                {
                    continue;
                }
                int count = 0;
                int j;
                for (j = i + 1; j < i + Length + 1; j++)
                {
                    if (line[j] == Stone.Black)
                    {
                        if (count >= 0)
                        {
                            count++;
                        }
                        else
                        {
                            count = 0;
                            break;
                        }
                    }
                    else if (line[j] == Stone.White)
                    {
                        if (count <= 0)
                        {
                            count--;
                        }
                        else
                        {
                            count = 0;
                            break;
                        }
                    }
                    else
                    {
                        continue;
                    }
                    
                }
                i = j - 1;
                switch (count)
                {
                    case 4:
                        result.BlackFour++;
                        break;
                    case 3:
                        result.BlackThree++;
                        break;
                    case 2:
                        result.BlackPair++;
                        break;
                    case -4:
                        result.WhiteFour++;
                        break;
                    case -3:
                        result.WhiteThree++;
                        break;
                    case -2:
                        result.WhitePair++;
                        break;
                }
            }
            return result;
        }
    }
}
