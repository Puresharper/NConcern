using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.ServiceModel;
using NConcern.Example.Logging;

namespace NConcern.Example.Linq
{
    public class Logging : IAspect
    {
        public IEnumerable<IAdvice> Advise(MethodInfo method)
        {
            var _trace = Expression.Parameter(Metadata<Trace>.Type);
            var _exception = Expression.Parameter(Metadata<Exception>.Type);
            if (method.ReturnType == Metadata.Void)
            {
                yield return Advice.Linq.Around((_Instance, _Arguments, _Body) =>
                {
                    return Expression.Block
                    (
                        new ParameterExpression[] { _trace },
                        Expression.Assign
                        (
                            _trace, 
                            Expression.New
                            (
                                Metadata.Constructor(() => new Trace(Argument<MethodInfo>.Value, Argument<string[]>.Value)), 
                                Expression.Constant(method),
                                Expression.NewArrayInit(Metadata<string>.Type, _Arguments.Select(_Argument => Expression.Call(_Argument, Metadata<object>.Method(_Object => _Object.ToString()))))
                            )
                         ),
                        Expression.TryCatch
                        (
                            Expression.Block
                            (
                                _Body,
                                Expression.Call(_trace, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<string>.Value)), Expression.Constant(null, Metadata<string>.Type))
                            ),
                            Expression.Catch
                            (
                                _exception, 
                                Expression.Call(_trace, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<Exception>.Value)), _exception),
                                Expression.Rethrow()
                            )
                        )
                    );
                });
            }
            else
            {
                var _return = Expression.Parameter(method.ReturnType);
                yield return Advice.Linq.Around((_Instance, _Arguments, _Body) =>
                {
                    return Expression.Block
                    (
                        new ParameterExpression[] { _return, _trace },
                        Expression.Assign
                        (
                            _trace,
                            Expression.New
                            (
                                Metadata.Constructor(() => new Trace(Argument<MethodInfo>.Value, Argument<string[]>.Value)),
                                Expression.Constant(method),
                                Expression.NewArrayInit(Metadata<string>.Type, _Arguments.Select(_Argument => Expression.Call(_Argument, Metadata<object>.Method(_Object => _Object.ToString()))))
                            )
                         ),
                        Expression.TryCatch
                        (
                            Expression.Block
                            (
                                Expression.Assign(_return, _Body),
                                Expression.Call(_trace, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<string>.Value)), Expression.Call(_return, Metadata<object>.Method(_Object => _Object.ToString()))),
                                _return
                            ),
                            Expression.Catch
                            (
                                _exception, 
                                Expression.Block
                                (
                                    Expression.Call(_trace, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<Exception>.Value)), _exception),
                                    Expression.Rethrow(method.ReturnType)
                                )
                            )
                        )
                    );
                });
            }
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            //define a joinpoint
            var _operationContractJoinpoint = new Func<MethodInfo, bool>(_Method => _Method.IsDefined(typeof(OperationContractAttribute), true));

            //instantiate a calculator
            var _calculator = new Calculator();

            //weave logging for all operation contract
            Aspect.Weave<Logging>(_operationContractJoinpoint);

            //invoke an operation contract (logging is enabled)
            var _return = _calculator.Divide(15, 3);

            //release logging for all operation contract
            Aspect.Release<Logging>(_operationContractJoinpoint);

            //invoke an operation contract (logging is disabled)
            _return = _calculator.Divide(15, 3);

            //enable back logging aspect.
            Aspect.Weave<Logging>(_operationContractJoinpoint);

            //invoke an operation with an exception (divide by zero)
            try { _return = _calculator.Divide(15, 0); }
            catch { }
        }
    }
}
