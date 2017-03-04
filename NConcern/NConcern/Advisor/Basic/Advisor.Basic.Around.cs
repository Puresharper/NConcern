using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Action(advised methd body) to invoke instead of advised method : Action(Action = [delegate to invoke advised method body])</param>
        /// <returns>Advice</returns>
        static public IAdvice Around(this Advisor.IBasic basic, Action<Action> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.Type();
                var _signature = _Method.Signature();
                var _routine = new Closure.Routine(_Pointer, _signature, _type);
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                }
                else
                {
                    _body.DeclareLocal(_routine.Type);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldfld, _routine.Value);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked instead of the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method], Action = [delegate that invokes the advised method body])</param>
        /// <returns>Advice</returns>
        static public IAdvice Around(this Advisor.IBasic basic, Action<object, object[], Action> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.Type();
                var _signature = _Method.Signature();
                var _routine = new Closure.Routine(_Pointer, _signature, _type);
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                }
                else
                {
                    _body.DeclareLocal(_routine.Type);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Ldfld, _routine.Value);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked instead of the advised method : Func(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method], Func() = [delegate that invokes advised method body (return boxed return value or null if return type is void)]) return boxed return value (null if return type is void)</param>
        /// <returns>Advice</returns>
        static public IAdvice Around(this Advisor.IBasic basic, Func<object, object[], Func<object>, object> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.Type();
                var _signature = _Method.Signature();
                var _function = new Closure.Function(_Pointer, _signature, _type);
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Newobj, _function.Constructor);
                _body.Emit(OpCodes.Ldftn, _function.Method);
                _body.Emit(OpCodes.Newobj, Metadata<Func<object>>.Type.GetConstructors().Single());
                _body.Emit(OpCodes.Call, advice.Method);
                if (_type == Metadata.Void) { _body.Emit(OpCodes.Pop); }
                else
                {
                    if (_type.IsValueType) { _body.Emit(OpCodes.Unbox_Any, _type); }
                    else { _body.Emit(OpCodes.Castclass, _type); }
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <typeparam name="T">Resource to create before and dispose after method execution</typeparam>
        /// <param name="basic">Basic</param>
        /// <returns>Advice</returns>
        static public IAdvice Around<T>(this Advisor.IBasic basic)
            where T : class, IDisposable, new()
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.Type();
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<T>.Type);
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new T()));
                    _body.Emit(OpCodes.Stloc_0);
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.BeginFinallyBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Callvirt, Metadata<IDisposable>.Method(_Disposable => _Disposable.Dispose()));
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(OpCodes.Newobj, Metadata.Constructor(() => new T()));
                    _body.Emit(OpCodes.Stloc_0);
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.Emit(OpCodes.Stloc_1);
                    _body.BeginFinallyBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Callvirt, Metadata<IDisposable>.Method(_Disposable => _Disposable.Dispose()));
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