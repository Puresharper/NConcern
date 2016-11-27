using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection
{
    static internal class Metadata
    {
        static public readonly Type Void = typeof(void);

        static public ConstructorInfo Constructor<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as NewExpression).Constructor;
        }

        static public FieldInfo Field<X>(Expression<Func<X>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        static public PropertyInfo Property<X>(Expression<Func<X>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }

        static public MethodInfo Method(Expression<Action> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        static public MethodInfo Method<X>(Expression<Func<X>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }
    }

    static internal partial class Metadata<T>
    {
        static public readonly Type Type = typeof(T);

        static public FieldInfo Field<X>(Expression<Func<T, X>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        static public PropertyInfo Property<X>(Expression<Func<T, X>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }

        static public MethodInfo Method(Expression<Action<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        static public MethodInfo Method<X>(Expression<Func<T, X>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }
    }
}
