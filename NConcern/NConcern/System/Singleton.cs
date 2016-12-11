using System;

namespace System
{
    static internal class Singleton<T>
        where T : class, new()
    {
        static public readonly T Value = new T();
    }
}
