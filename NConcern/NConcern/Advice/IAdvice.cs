using System;
using System.Reflection;

namespace NConcern
{
    public interface IAdvice
    {
        MethodInfo Decorate(MethodInfo method, IntPtr pointer);
    }
}
