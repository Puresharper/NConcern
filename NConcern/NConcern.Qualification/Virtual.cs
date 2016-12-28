using System;

namespace NConcern.Qualification
{
    public class Virtual
    {
        virtual public int Method(int x, int y)
        {
            return Interception.Sequence();
        }
    }
}
