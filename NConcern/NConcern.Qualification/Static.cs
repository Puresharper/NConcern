using System;

namespace NConcern.Qualification
{
    static public class Static
    {
        static public int Method(int x, int y)
        {
            return Interception.Sequence();
        }
    }
}
