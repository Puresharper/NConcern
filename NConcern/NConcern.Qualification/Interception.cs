using System;

namespace NConcern.Qualification
{
    static internal class Interception
    {
        static public readonly object Handle = new object();
        static private int m_Sequence;

        static public int Sequence()
        {
            return Interception.m_Sequence++;
        }

        static public bool Done;
        static public object Instance;
        static public object[] Arguments;
        static public object Return;
        static public Exception Exception;

        static public void Initialize()
        {
            Interception.m_Sequence = 0;
            Interception.Done = false;
            Interception.Instance = null;
            Interception.Arguments = null;
            Interception.Return = null;
            Interception.Exception = null;
        }
    }
}
