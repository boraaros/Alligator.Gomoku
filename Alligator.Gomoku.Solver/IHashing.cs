using System;

namespace Alligator.Gomoku
{
    public interface IHashing
    {
        ulong HashCode { get; }
        void Modify(params int[] indices);
    }
}
