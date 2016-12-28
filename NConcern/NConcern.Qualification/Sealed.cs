using System;

namespace NConcern.Qualification
{
    public sealed class Sealed
    {
        public int Method(int x, int y)
        {
            return Interception.Sequence();
        }
    }
}
