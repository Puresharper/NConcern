using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection
{
    /// <summary>
    /// Metadata.
    /// </summary>
    static public class Metadata
    {
        /// <summary>
        /// Void
        /// </summary>
        static public readonly Type Void = typeof(void);

        /// <summary>
        /// Obtain constructor from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Constructor</returns>
        static public ConstructorInfo Constructor<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as NewExpression).Constructor;
        }

        /// <summary>
        /// Obtain static field from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Field</returns>
        static public FieldInfo Field<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        /// <summary>
        /// Obtain static property from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>PropertyInfo</returns>
        static public PropertyInfo Property<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }

        /// <summary>
        /// Obtain static ethod from linq expression.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method(Expression<Action> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Obtain static method from linq expression.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method<T>(Expression<Func<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }
    }

    /// <summary>
    /// Metadata
    /// </summary>
    /// <typeparam name="T"></typeparam>
    static public partial class Metadata<T>
    {
        /// <summary>
        /// Type.
        /// </summary>
        static public readonly Type Type = typeof(T);

        /// <summary>
        /// Obtain field from linq expression.
        /// </summary>
        /// <typeparam name="X">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Field</returns>
        static public FieldInfo Field<X>(Expression<Func<T, X>> expression)
        {
            return (expression.Body as MemberExpression).Member as FieldInfo;
        }

        /// <summary>
        /// Obtain property from linq expression.
        /// </summary>
        /// <typeparam name="X">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Property</returns>
        static public PropertyInfo Property<X>(Expression<Func<T, X>> expression)
        {
            return (expression.Body as MemberExpression).Member as PropertyInfo;
        }

        /// <summary>
        /// Obtain method from linq expression.
        /// </summary>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method(Expression<Action<T>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }

        /// <summary>
        /// Obtain method from linq expression.
        /// </summary>
        /// <typeparam name="X">Type</typeparam>
        /// <param name="expression">Expression</param>
        /// <returns>Method</returns>
        static public MethodInfo Method<X>(Expression<Func<T, X>> expression)
        {
            return (expression.Body as MethodCallExpression).Method;
        }
    }
}
