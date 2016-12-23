using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConcern.Qualification
{
    static internal class Interception
    {
        static public bool Done;
        static public object Instance;
        static public object[] Arguments;
        static public object Return;
        static public Exception Exception;

        static public void Initialize()
        {
            Interception.Done = false;
            Interception.Instance = null;
            Interception.Arguments = null;
            Interception.Return = null;
            Interception.Exception = null;
        }
    }
}
