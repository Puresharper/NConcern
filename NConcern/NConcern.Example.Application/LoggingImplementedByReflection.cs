using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;

namespace NConcern.Example.Application
{
    public class LoggingImplementedByReflection : Aspect
    {
        static private string Display(MethodInfo method, object[] arguments)
        {
            var _parameters = method.GetParameters();
            var _display = new List<string>();
            for (var _index = 0; _index < _parameters.Length; _index++)
            {
                var _parameter = _parameters[_index];
                var _argument = arguments[0];
                _display.Add(string.Concat(_parameter.Name, "=", _argument == null ? "[null]" : _argument.ToString()));
            }
            return string.Concat(method.Name, "(", string.Join(", ", _display), ")");
        }

        override protected IEnumerable<Advice> Advise<T>(MethodInfo method)
        {
            if (Logging.Managed(method))
            {
                yield return new Before((_Instance, _Arguments) => Console.WriteLine(LoggingImplementedByReflection.Display(method, _Arguments)));
            }
        }
    }
}
