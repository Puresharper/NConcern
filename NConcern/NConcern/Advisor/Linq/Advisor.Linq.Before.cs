using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Create an advice that runs before the advised method.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression of code to be invoked before the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Before(this Advisor.ILinq linq, Expression advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                if (advice == null) { return null; }
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
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
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked before the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable(Expression) = [enumerable of expression of argument used to call advised method]) return an expression(void) of code to invoke before the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Before(this Advisor.ILinq linq, Func<Expression, IEnumerable<Expression>, Expression> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                var _advice = _signature.Instance == null ? advice(null, _parameters) : advice(_parameters[0], _parameters.Skip(1));
                if (_advice == null) { return null; }
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }
    }
}