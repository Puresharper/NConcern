using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Reflection.
        /// </summary>
        public partial class Reflection
        {
            /// <summary>
            /// Create an advice that runs after the advised method.
            /// </summary>
            public interface IAfter
            {
                /// <summary>
                /// GetHashCode.
                /// </summary>
                /// <returns>HashCode</returns>
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                int GetHashCode();

                /// <summary>
                /// ToString.
                /// </summary>
                /// <returns>String</returns>
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string ToString();

                /// <summary>
                /// GetType.
                /// </summary>
                /// <returns>Type</returns>
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type GetType();
            }

            private sealed class After : IAfter
            {
                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                int Advisor.Reflection.IAfter.GetHashCode()
                {
                    return this.GetHashCode();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string Advisor.Reflection.IAfter.ToString()
                {
                    return this.ToString();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type Advisor.Reflection.IAfter.GetType()
                {
                    return this.GetType();
                }
            }
        }

        /// <summary>
        /// After
        /// </summary>
        /// <param name="reflection">Reflection</param>
        /// <returns>After</returns>
        static public Advisor.Reflection.IAfter After(this Advisor.IReflection reflection)
        {
            return null;
        }

        /// <summary>
        /// Create an advice that runs after the advised method regardless of its outcome.
        /// </summary>
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice After(this Advisor.IReflection reflection, Action<ILGenerator> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.Type();
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.BeginFinallyBlock();
                    _body.Emit(advice);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.BeginFinallyBlock();
                    _body.Emit(advice);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}