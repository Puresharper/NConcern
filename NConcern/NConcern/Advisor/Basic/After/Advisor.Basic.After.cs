using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        public partial class Basic
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
                int Advisor.Basic.IAfter.GetHashCode()
                {
                    return this.GetHashCode();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                string Advisor.Basic.IAfter.ToString()
                {
                    return this.ToString();
                }

                [DebuggerHidden]
                [EditorBrowsable(EditorBrowsableState.Never)]
                Type Advisor.Basic.IAfter.GetType()
                {
                    return this.GetType();
                }
            }
        }

        /// <summary>
        /// After
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <returns>After</returns>
        static public Advisor.Basic.IAfter After(this Advisor.IBasic basic)
        {
            return null;
        }

        /// <summary>
        /// Create an advice that runs after the advised method regardless of its outcome.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice After(this Advisor.IBasic basic, Action advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.ReturnType;
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<bool>.Type);
                var _realized = _body.DefineLabel();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.Emit(OpCodes.Ldc_I4_1);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Brtrue, _realized);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.MarkLabel(_realized);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.Emit(OpCodes.Stloc_1);
                    _body.Emit(OpCodes.Ldc_I4_1);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Brtrue, _realized);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.MarkLabel(_realized);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_1);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method regardless of its outcome.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public IAdvice After(this Advisor.IBasic basic, Action<object, object[]> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<bool>.Type);
                var _realized = _body.DefineLabel();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.Emit(OpCodes.Ldc_I4_1);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Brtrue, _realized);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.MarkLabel(_realized);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.Emit(OpCodes.Stloc_1);
                    _body.Emit(OpCodes.Ldc_I4_1);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    _body.Emit(OpCodes.Ldloc_1);
                    _body.Emit(OpCodes.Brtrue, _realized);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.MarkLabel(_realized);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_1);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}