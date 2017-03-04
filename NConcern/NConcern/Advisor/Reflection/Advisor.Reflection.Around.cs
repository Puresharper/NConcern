using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked instead of the advised method : Action(Action = [delegate used to produce advised method body])</param>
        /// <returns>Advice</returns>
        static public IAdvice Around(this Advisor.IReflection reflection, Action<ILGenerator, Action> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _type = _Method.Type();
                var _signature = _Method.Signature();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                advice(_body, () =>
                {
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                });
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}