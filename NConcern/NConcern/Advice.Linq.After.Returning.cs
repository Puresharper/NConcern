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
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Expression (void) of code to be invoked after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Linq<T>.IAfter linq, Expression advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _signature = activity.Signature;
                if (advice == null) { return activity; }
                if (advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(activity);
                _body.Emit(OpCodes.Call, Expression.Lambda(advice).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call (null if advised method is static)], IEnumerable<Expression> = [enumerable of expression of argument used to call advised method]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Linq<T>.IAfter linq, Func<Expression, IEnumerable<Expression>, Expression> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _signature = activity.Signature;
                var _parameters = _signature.Select(_Type => Expression.Parameter(_Type)).ToArray();
                var _advice = _signature.Instance == null ? advice(null, _parameters) : advice(_parameters[0], _parameters.Skip(1));
                if (_advice == null) { return activity; }
                if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                var _type = activity.Method.ReturnType;
                var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                var _body = _method.GetILGenerator();
                _body.Emit(activity);
                _body.Emit(_signature, false);
                _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                _body.Emit(OpCodes.Ret);
                _method.Prepare();
                return activity.Override(_method);
            });
        }

        /// <summary>
        /// Create an advice that runs after the advised method only if it completes successfully.
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="linq">Linq</param>
        /// <param name="advice">Delegate used to produce an expression of code to be invoked after the advised method : Func(Expression = [expression of target instance of advised method call (null if advised method is static)], IEnumerable<Expression> = [enumerable of expression of argument used to call advised method], Expression = [expression of return value (null if return type is void)]) return an expression(void) of code to invoke after the advised method</param>
        /// <returns>Advice</returns>
        static public Advice<T> Returning<T>(this Advice.Linq<T>.IAfter linq, Func<Expression, IEnumerable<Expression>, Expression, Expression> advice)
            where T : class
        {
            return new Advice<T>(activity =>
            {
                var _signature = activity.Signature;
                var _type = activity.Method.ReturnType;
                var _parameters = _signature.Select(_Type => Expression.Parameter(_Type)).ToArray();
                if (_type == Metadata.Void)
                {
                    var _advice = _signature.Instance == null ? advice(null, _parameters, null) : advice(_parameters[0], _parameters.Skip(1), null);
                    if (_advice == null) { return activity; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    _body.Emit(activity);
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, _parameters).CompileToMethod());
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method);
                }
                else
                {
                    var _return = Expression.Parameter(_type);
                    var _advice = _signature.Instance == null ? advice(null, _parameters, _return) : advice(_parameters[0], _parameters.Skip(1), _return);
                    if (_advice == null) { return activity; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    var _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Dup);
                    _body.Emit(_signature, false);
                    _body.Emit(OpCodes.Call, Expression.Lambda(_advice, new ParameterExpression[] { _return }.Concat(_parameters)).CompileToMethod());
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method);
                }
            });
        }
    }
}