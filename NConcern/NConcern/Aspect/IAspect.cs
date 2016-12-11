using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NConcern
{
    public interface IAspect
    {
        IEnumerable<IAdvice> Advise(MethodInfo method);
    }
}
