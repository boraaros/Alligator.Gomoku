using System;
using System.Collections.Generic;

namespace Alligator.Gomoku.Evaluation
{
    public class PatternCounts
    {
        public int BlackFour;
        public int WhiteFour;

        public int BlackThree;
        public int WhiteThree;

        public int BlackPair;
        public int WhitePair;

        public void Add(PatternCounts other)
        {
            BlackFour += other.BlackFour;
            WhiteFour += other.WhiteFour;
            BlackThree += other.BlackThree;
            WhiteThree += other.WhiteThree;
            BlackPair += other.BlackPair;
            WhitePair += other.WhitePair;
        }

        public void Subtract(PatternCounts other)
        {
            BlackFour -= other.BlackFour;
            WhiteFour -= other.WhiteFour;
            BlackThree -= other.BlackThree;
            WhiteThree -= other.WhiteThree;
            BlackPair -= other.BlackPair;
            WhitePair -= other.WhitePair;
        }
    }
}
