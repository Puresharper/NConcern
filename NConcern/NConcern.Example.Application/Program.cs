using System;
using System.Collections.Generic;
using System.Reflection;

namespace NConcern.Example.Application
{
    public class Calculator
    {
        //public int Add(int a, int b)
        //{
        //    return a + b;
        //}

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
                yield return new Before(() => { Console.WriteLine("Aspect1 > Before"); });
                yield return new After.Returning(() => { Console.WriteLine("Aspect1 > After"); });
                yield return new Around(_Action =>
                {
                    Console.WriteLine("Aspect1 > Arround > Before");
                    _Action();
                    Console.WriteLine("Aspect1 > Arround > After");
                });
            }
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            var a1 = new Aspect1();
            a1.Manage<Calculator>();
            var calculator = new Calculator();
            calculator.F();
        }
    }
}
