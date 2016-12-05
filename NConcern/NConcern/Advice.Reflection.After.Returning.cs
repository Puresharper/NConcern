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
        /// <param name="reflection">Reflection</param>
        /// <param name="advice">Delegate used to emit code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Reflection<T>.IAfter reflection, Action<ILGenerator> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(activity);
                _body.Emit(advice);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }
    }
}