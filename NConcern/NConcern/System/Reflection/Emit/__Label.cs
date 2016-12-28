using System;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection.Emit
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __Label
    {
        static private readonly Func<Label, int> m_Value = Expression.Lambda<Func<Label, int>>(Expression.Field(Parameter<Label>.Expression, "m_Label"), Parameter<Label>.Expression).Compile();

        static public int Value(this Label label)
        {
            return __Label.m_Value(label);
        }
    }
}
