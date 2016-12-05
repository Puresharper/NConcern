using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advice
    {
        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Before<T>(this Advice.IBasic<T> basic, Action advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _method = new DynamicMethod(string.Empty, _Activity.Method.ReturnType, _Activity.Signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(_Activity);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked before the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Before<T>(this Advice.IBasic<T> basic, Action<object, object[]> advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _signature = _Activity.Signature;
                var _method = new DynamicMethod(string.Empty, _Activity.Method.ReturnType, _signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advice<T>.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(_Activity);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }
    }
}