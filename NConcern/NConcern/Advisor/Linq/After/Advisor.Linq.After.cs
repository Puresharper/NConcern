using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        public partial class Linq
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
                int Advisor.Linq.IAfter.GetHashCode()
                {
                    return this.GetHashCode();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string Advisor.Linq.IAfter.ToString()
                {
                    return this.ToString();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type Advisor.Linq.IAfter.GetType()
                {
                    return this.GetType();
                }
            }
        }

        /// <summary>
        /// After
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <returns>After</returns>
        static public Advisor.Linq.IAfter After(this Advisor.ILinq linq)
        {
            return null;
        }

        /// <summary>
        /// Create an advice that runs after the advised method regardless of its outcome.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice"></param>
        /// <returns>Advice</returns>
        static public IAdvice After(this Advisor.ILinq linq, Expression advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                if (advice == null) { return null; }
                if (advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.BeginFinallyBlock();
                    _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
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
                    _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method regardless of its outcome.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable(Expression) = [enumerable of expression of argument used to call advised method]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice After(this Advisor.ILinq linq, Func<Expression, IEnumerable<Expression>, Expression> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                var _advice = _signature.Instance == null ? advice(null, _parameters) : advice(_parameters[0], _parameters.Skip(1));
                if (_advice == null) { return null; }
                if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.BeginFinallyBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
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
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
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