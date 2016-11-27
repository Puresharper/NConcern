using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Reflection
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __Type
    {
        static public IEnumerable<Type> Base(this Type type)
        {
            for (var _type = type; _type != null; _type = _type.BaseType) { yield return _type; }
        }

        static public string Declaration(this Type type)
        {
            if (type.IsGenericType)
            {
                return string.Concat(type.FullName.Remove(type.FullName.IndexOf('`')), "<", string.Join(", ", type.GetGenericArguments().Select(__Type.Declaration)), ">");
            }
            return type.FullName;
        }
    }
}
