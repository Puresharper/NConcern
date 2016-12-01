using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using System.Text;

namespace NConcern.Example.Application
{
    public class LoggingImplementedByExpression : Aspect
    {
        override protected IEnumerable<Advice> Advise<T>(MethodInfo method)
        {
            if (Logging.Managed(method))
            {
                var _parameters = method.GetParameters();
                var _append = Metadata<StringBuilder>.Method(_StringBuilder => _StringBuilder.Append(Argument<string>.Value));
                var _display = Metadata<object>.Method(_Object => _Object.ToString());
                var _log = Metadata.Method(() => Console.WriteLine(Argument<string>.Value));
                yield return new Before(new Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression>((_Instance, _Arguments) =>
                {
                    var _builder = Expression.Parameter(typeof(StringBuilder));
                    var _body = new List<Expression>();

                    //Instantiate string builder.
                    _body.Add(Expression.Assign(_builder, Expression.New(_builder.Type)));

                    //Apend method name with open parenthesis.
                    _body.Add(Expression.Call(_builder, _append, Expression.Constant(method.Name + "(")));

                    //foreach parameter, builder.Append("parameterName=" + argument == null ? "null" : argument.ToString());
                    for (var _index = 0; _index < _parameters.Length; _index++)
                    {
                        var _argument = _Arguments.ElementAt(_index);
                        if (_index == 0) { _body.Add(Expression.Call(_builder, _append, Expression.Constant(string.Concat(_parameters[_index].Name, "=")))); }
                        else { _body.Add(Expression.Call(_builder, _append, Expression.Constant(string.Concat(", ", _parameters[_index].Name, "=")))); }
                        _body.Add(Expression.Call(_builder, _append, Expression.Condition(Expression.IsTrue(_argument), Expression.Call(_argument, _display), Expression.Constant("null"))));
                    }

                    //Append parenthesis.
                    _body.Add(Expression.Call(_builder, _append, Expression.Constant(")")));

                    //Write into console builder.ToString()
                    _body.Add(Expression.Call(_log, Expression.Call(_builder, _display)));

                    //Return statement into a block expression.
                    return Expression.Block(new ParameterExpression[] { _builder }, _body );
                }));
            }
        }
    }
}
