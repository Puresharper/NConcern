using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advice
    {
        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Basic<T>.IAfter basic, Action advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _type = _Activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _Activity.Signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_Activity);
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call (null if advised method is static)], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Basic<T>.IAfter basic, Action<object, object[]> advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _signature = _Activity.Signature;
                var _type = _Activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_Activity);
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked after the advised method : Action(object = [target instance of advised method call (null if advised method is static)], object[] = [boxed arguments used to call advised method], object = [return value (null if return type is void)])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Basic<T>.IAfter basic, Action<object, object[], object> advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _signature = _Activity.Signature;
                var _type = _Activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.DeclareLocal(Metadata<object>.Type);
                _body.Emit(_Activity);
                if (_type == Metadata.Void) { _body.Emit(OpCodes.Ldnull); }
                else
                {
                    _body.Emit(OpCodes.Dup);
                    if (_type.IsValueType) { _body.Emit(OpCodes.Box, _type); }
                    else if (_type != Metadata<object>.Type) { _body.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                }
                _body.Emit(OpCodes.Stloc_0);
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(OpCodes.Ldloc_0);
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }
    }
}