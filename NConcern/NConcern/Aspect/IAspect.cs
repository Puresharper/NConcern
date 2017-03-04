using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NConcern
{
    /// <summary>
    /// Aspect : provide advice(s) for method.
    /// </summary>
    public interface IAspect
    {
        /// <summary>
        /// Advise a method.
        /// </summary>
        /// <param name="method">Method to advise</param>
        /// <returns>Enumerable of advices</returns>
        IEnumerable<IAdvice> Advise(MethodBase method);
    }
}
