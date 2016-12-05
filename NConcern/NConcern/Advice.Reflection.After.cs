using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advice
    {
        public partial class Reflection<T>
        {
            public interface IAfter
            {
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                int GetHashCode();

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string ToString();

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type GetType();
            }

            private sealed class After : IAfter
            {
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                int Advice.Reflection<T>.IAfter.GetHashCode()
                {
                    return this.GetHashCode();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string Advice.Reflection<T>.IAfter.ToString()
                {
                    return this.ToString();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type Advice.Reflection<T>.IAfter.GetType()
                {
                    return this.GetType();
                }
            }
        }

        /// <summary>
        /// Create an advice that runs after the advised method regardless of its outcome.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> After<T>(this Advice.IReflection<T> reflection, Action<ILGenerator> advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _type = _Activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _Activity.Signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_Activity);
                    _body.BeginFinallyBlock();
                    _body.Emit(advice);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(_Activity);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.BeginFinallyBlock();
                    _body.Emit(advice);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }
    }
}