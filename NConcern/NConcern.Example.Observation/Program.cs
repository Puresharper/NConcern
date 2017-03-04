using System;
using System.Collections.Generic;
using System.Reflection;

namespace NConcern.Example.Observation
{
    public class Observation<T> : IAspect
        where T : class
    {
        public class Argument : EventArgs
        {
            public readonly MethodBase Method;

            public Argument(MethodBase method)
            {
                this.Method = method;
            }
        }

        static Observation()
        {
            Aspect.Weave<Observation<T>>(_Method => _Method.ReflectedType == Metadata<T>.Type);
        }

        static public event EventHandler<Argument> Method;

        public IEnumerable<IAdvice> Advise(MethodBase method)
        {
            yield return Advice.Basic.After((_Instance, _Arguments) =>
            {
                if (Observation<T>.Method != null) { Observation<T>.Method(_Instance, new Observation<T>.Argument(method)); }
            });
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            //Attach and observation on all methods of Calculator.
            Observation<Calculator>.Method += new EventHandler<Observation<Calculator>.Argument>((_Sender, _Argument) => Console.WriteLine(_Argument.Method.Name));

            //Call a method of calculator.
            var _calculator = new Calculator();
            _calculator.Add(1, 2);
        }
    }
}
