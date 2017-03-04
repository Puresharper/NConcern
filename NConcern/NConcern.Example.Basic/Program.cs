using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using NConcern.Example.Logging;

namespace NConcern.Example.Basic
{
    public class Logging : IAspect
    {
        public IEnumerable<IAdvice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Around(new Func<object, object[], Func<object>, object>((_Instance, _Arguments, _Body) =>
            {
                var _trace = new Trace(method, _Arguments.Select(_Argument => _Argument.ToString()).ToArray());
                try
                {
                    var _return = _Body();
                    _trace.Dispose(_return.ToString());
                    return _return;
                }
                catch (Exception exception)
                {
                    _trace.Dispose(exception);
                    throw;
                }
            }));
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            //define a joinpoint
            var _operationContractJoinpoint = new Func<MethodBase, bool>(_Method => _Method.IsDefined(typeof(OperationContractAttribute), true));

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
