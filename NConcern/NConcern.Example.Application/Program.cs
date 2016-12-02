using System;
using System.Collections.Generic;
using System.Reflection;

namespace NConcern.Example.Application
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        static public string Concat(string a, string b)
        {
            return a + b;
        }

        public void F()
        {

        }
    }

    public class Aspect1 : Aspect
    {
        override protected IEnumerable<Advice> Advise<T>(MethodInfo method)
        {
            if (method.DeclaringType == typeof(T))
            {
                yield return new Before(() => { Console.WriteLine("Before"); });
                yield return new After.Returning(() => { Console.WriteLine("After"); });
                yield return new Around(_Action =>
                {
                    Console.WriteLine("Around > Before");
                    _Action();
                    Console.WriteLine("Around > After");
                });
                yield return new Before((Instance, Arguments) => Console.WriteLine("Before : {0}.{1}({2})", Instance, method.Name, string.Join(", ", Arguments)));
                yield return new After.Returning((Instance, Arguments, Return) => Console.WriteLine("After : {0}.{1}({2})={3}", Instance, method.Name, string.Join(", ", Arguments), Return));
                yield return new Around((Instance, Arguments, _Function) =>
                {
                    Console.WriteLine("Around > Before : {0}.{1}({2})", Instance, method.Name, string.Join(", ", Arguments));
                    var _return = _Function(Instance, Arguments);
                    Console.WriteLine("Around > After : {0}.{1}({2})={3}", Instance, method.Name, string.Join(", ", Arguments), _return);
                    return _return;
                });
            }
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            var a1 = new Aspect1();
            //a1.Manage<Calculator>();
            a1.Manage(typeof(Calculator));
            var calculator = new Calculator();
            calculator.F();
            Console.WriteLine();
            calculator.Add(2, 8);
            Console.WriteLine();
            Calculator.Concat("2", "8");
        }
    }
}
