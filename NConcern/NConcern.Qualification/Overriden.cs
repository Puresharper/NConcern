using System;

namespace NConcern.Qualification
{
    public class Overriden : Virtual
    {
        override public int Method(int x, int y)
        {
            return base.Method(x, y);
        }
    }
}
