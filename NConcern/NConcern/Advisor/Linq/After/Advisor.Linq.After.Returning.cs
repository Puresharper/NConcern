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
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression (void) of code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Returning(this Advisor.Linq.IAfter linq, Expression advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                if (advice == null) { return _Method; }
                if (advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable<Expression> = [enumerable of expression of argument used to call advised method]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Returning(this Advisor.Linq.IAfter linq, Func<Expression, IEnumerable<Expression>, Expression> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _parameters = _signature.Select(_Type => Expression.Parameter(_Type)).ToArray();
                var _advice = _signature.Instance == null ? advice(null, _parameters) : advice(_parameters[0], _parameters.Skip(1));
                if (_advice == null) { return _Method; }
                if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                var _type = _Method.Type();
                var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(_signature, false);
                _body.Emit(_Pointer, _type, _signature);
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return _method;
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call], IEnumerable<Expression> = [enumerable of expression of argument used to call advised method], Expression = [expression of return value (null if return type is void)]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public IAdvice Returning(this Advisor.Linq.IAfter linq, Func<Expression, IEnumerable<Expression>, Expression, Expression> advice)
        {
            return new Advice((_Method, _Pointer) =>
            {
                var _signature = _Method.Signature();
                var _type = _Method.Type();
                var _parameters = _signature.Select(_Type => Expression.Parameter(_Type)).ToArray();
                if (_type == Metadata.Void)
                {
                    var _advice = _signature.Instance == null ? advice(null, _parameters, null) : advice(_parameters[0], _parameters.Skip(1), null);
                    if (_advice == null) { return _Method; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return _method;
                }
                else
                {
                    var _return = Expression.Parameter(_type);
                    var _advice = _signature.Instance == null ? advice(null, _parameters, _return) : advice(_parameters[0], _parameters.Skip(1), _return);
                    if (_advice == null) { return _Method; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    var _method = new DynamicMethod(string.Empty, _type, _signature, _Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    _body.Emit(_signature, false);
                    _body.Emit(_Pointer, _type, _signature);
                    _body.Emit(OpCodes.Dup);
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, new ParameterExpression[] { _return }.Concat(_parameters)).CompileToMethod());
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return _method;
                }
            });
        }
    }
}