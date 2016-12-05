using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    public sealed class Advice<T>
        where T : class
    {
        static internal readonly ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();

        /// <summary>
        /// Basic
        /// </summary>
        static public readonly Advice.IBasic<T> Basic = new Advice.Basic<T>();

        /// <summary>
        /// Linq
        /// </summary>
        static public readonly Advice.ILinq<T> Linq = new Advice.Linq<T>();

        /// <summary>
        /// Reflection
        /// </summary>
        static public readonly Advice.IReflection<T> Reflection = new Advice.Reflection<T>();

        /// <summary>
        /// Create an advise by specifing the method to invoke instead of advised method (if advised method is called, it must be done using an unsafe call to avoid a stack overflow).
        /// </summary>
        /// <param name="method">Method to call instread of advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Create(MethodInfo method)
        {
            return new Advice<T>(_Activity =>
            {
                if (_Activity.Method.ReturnType == method.ReturnType && _Activity.Signature.SequenceEqual(method.Signature())) { return _Activity.Override(method); }
                throw new InvalidOperationException();
            });
        }

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        internal readonly Func<Aspect.Activity<T>, Aspect.Activity<T>> Advise;

        internal Advice(Func<Aspect.Activity<T>, Aspect.Activity<T>> advise)
        {
            this.Advise = advise;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static public partial class Advice
    {
        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }
}