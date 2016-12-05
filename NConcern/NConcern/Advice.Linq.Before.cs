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
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression of code to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Before<T>(this Advice.ILinq<T> linq, Expression advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _signature = _Activity.Signature;
                if (advice == null) { return _Activity; }
                var _type = _Activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
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
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked before the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable<Expression> = [enumerable of expression of argument used to call advised method]) return an expression(void) of code to invoke before the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Before<T>(this Advice.ILinq<T> linq, Func<Expression, IEnumerable<Expression>, Expression> advice)
            where T : class
        {
            return new Advice<T>(_Activity =>
            {
                var _signature = _Activity.Signature;
                var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                var _advice = _signature.Instance == null ? advice(null, _parameters) : advice(_parameters[0], _parameters.Skip(1));
                if (_advice == null) { return _Activity; }
                var _type = _Activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                _body.Emit(_Activity);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _Activity.Override(_method);
            });
        }
    }
}