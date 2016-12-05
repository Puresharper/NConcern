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
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Before<T>(this Advice.IReflection<T> reflection, Action<ILGenerator> advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _method = new DynamicMethod(string.Empty, _Activity.Method.ReturnType, _Activity.Signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(advice);
                _body.Emit(_Activity);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }
    }
}