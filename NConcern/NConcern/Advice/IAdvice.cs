using System;
using System.Reflection;

namespace NConcern
{
    /// <summary>
    /// Advice : a way to decorate a method for a specific concern
    /// </summary>
    public interface IAdvice
    {
        /// <summary>
        /// Decorate a method for a specific concern.
        /// </summary>
        /// <param name="method">Base method to decorate</param>
        /// <param name="pointer">Pointer to base method</param>
        /// <returns>Replacing method</returns>
        MethodBase Decorate(MethodBase method, IntPtr pointer);
    }
}
