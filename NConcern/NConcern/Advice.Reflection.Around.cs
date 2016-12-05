using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advice
    {
        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked instead of the advised method : Action(Action = [delegate used to produce advised method body])</param>
        /// <returns>Advice</returns>
        static public Advice<T> Around<T>(this Advice.IReflection<T> reflection, Action<ILGenerator, Action> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                advice(_body, () => _body.Emit(activity));
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }
    }
}