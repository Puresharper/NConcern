using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Before(this Advisor.IBasic basic, Action advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <param name="basic">Basic</param>
        /// <param name="advice">Delegate to be invoked before the advised method : Action(object = [target instance of advised method call], object[] = [boxed arguments used to call advised method])</param>
        /// <returns>Advice</returns>
        static public IAdvice Before(this Advisor.IBasic basic, Action<object, object[]> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                if (advice.Target != null) { _body.Emit(OpCodes.Ldsfld, Advisor.Module.DefineField(advice.Target)); }
                _body.Emit(_signature, true);
                _body.Emit(OpCodes.Call, advice.Method);
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}