using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace NConcern
{
    static public partial class Advice
    {
        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Action(advised methd body) to invoke instead of advised method : Action(Action = [delegate to invoke advised method body])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Around<T>(this Advice.IBasic<T> basic, Action<Action> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _signature = activity.Signature;
                var _routine = new Closure<T>.Routine(activity.Pointer, _signature, activity.Method.ReturnType);
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Newobj, _routine.Constructor);
                    _body.Emit(OpCodes.Ldftn, _routine.Method);
                    _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, advice.Method);
                }
                else
                {
                    _body.DeclareLocal(_routine.Type);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
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
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked instead of the advised method : Action(object = [target instance of advised method call (null if advised method is static)], object[] = [boxed arguments used to call advised method], Action = [delegate that invokes the advised method body])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Around<T>(this Advice.IBasic<T> basic, Action<object, object[], Action> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _signature = activity.Signature;
                var _routine = new Closure<T>.Routine(activity.Pointer, _signature, activity.Method.ReturnType);
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
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
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
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
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked instead of the advised method : Func(object = [target instance of advised method call (null if advised method is static)], object[] = [boxed arguments used to call advised method], Func() = [delegate that invokes advised method body (return boxed return value or null if return type is void)]) return boxed return value (null if return type is void)</param>
        /// <returns>Advice</returns>
        static public Advice<T> Around<T>(this Advice.IBasic<T> basic, Func<object, object[], Func<object>, object> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _signature = activity.Signature;
                var _function = new Closure<T>.Function(activity.Pointer, _signature, activity.Method.ReturnType);
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
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
                return activity.Override(_method);
            });
        }
    }
}