using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked instaed of the advised method : Func(Expression = [expression of advised method body]) return an expression to invoke instead of the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Around<T>(this Advice.ILinq<T> linq, Func<Expression, Expression> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _signature = activity.Signature;
                var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(activity);
                _body.Emit(OpCodes.Ret);
                var _advice = _signature.Instance == null ? advice(Expression.Call(_method, _parameters)) : advice(Expression.Call(_method, _parameters));
                if (_advice == null) { return activity; }
                _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs before and after the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked instaed of the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable<Expression> = [enumerable of expression of argument used to call advised method], Expression = [expression of advised method body]) return an expression to invoke instead of the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Around<T>(this Advice.ILinq<T> linq, Func<Expression, IEnumerable<Expression>, Expression, Expression> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _type = activity.Method.ReturnType;
                var _signature = activity.Signature;
                var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(activity);
                _body.Emit(OpCodes.Ret);
                var _advice = _signature.Instance == null ? advice(null, _parameters, Expression.Call(_method, _parameters)) : advice(_parameters[0], _parameters.Skip(1), Expression.Call(_method, _parameters));
                if (_advice == null) { return activity; }
                _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }
    }
}