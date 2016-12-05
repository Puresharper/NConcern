using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advice
    {
        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Throwing<T>(this Advice.Basic<T>.IAfter basic, Action advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(activity);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Throwing<T>(this Advice.Basic<T>.IAfter basic, Action<object, object[]> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _signature = activity.Signature;
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(activity);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                }
                else
                {
                    _body.DeclareLocal(_type);
                    _body.BeginExceptionBlock();
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Stloc_0);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Pop);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_0);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it exits by throwing an exception.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method], Exception = [exception thrown])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Throwing<T>(this Advice.Basic<T>.IAfter basic, Action<object, object[], Exception> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _signature = activity.Signature;
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<Exception>.Type);
                if (_type == Metadata.Void)
                {
                    _body.BeginExceptionBlock();
                    _body.Emit(activity);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
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
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Stloc_1);
                    _body.BeginCatchBlock(Metadata<Exception>.Type);
                    _body.Emit(OpCodes.Stloc_0);
                    if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Ldloc_0);
                    _body.Emit(OpCodes.Call, advice.Method);
                    _body.Emit(OpCodes.Rethrow);
                    _body.EndExceptionBlock();
                    _body.Emit(OpCodes.Ldloc_1);
                }
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }
    }
}