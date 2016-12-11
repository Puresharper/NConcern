using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Throwing(this Advisor.Basic.IAfter basic, Action advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.ReturnType;
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public IAdvice Throwing(this Advisor.Basic.IAfter basic, Action<object, object[]> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method], Exception = [exception thrown])</param>
        /// <returns>Advice</returns>
        static public IAdvice Throwing(this Advisor.Basic.IAfter basic, Action<object, object[], Exception> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<Exception>.Type);
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _Method.ReturnType, _signature);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Call, advice.Method);
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
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Call, advice.Method);
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