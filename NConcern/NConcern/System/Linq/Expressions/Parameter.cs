using System;

namespace System.Linq.Expressions
{
    static internal class Parameter<T>
    {
        static public readonly ParameterExpression Expression = System.Linq.Expressions.Expression.Parameter(typeof(T));
    }
}
