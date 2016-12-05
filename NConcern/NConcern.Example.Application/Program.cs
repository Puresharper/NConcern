using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern.Example.Application
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            Console.WriteLine("ADD!!!");
            return a + b;
        }

        static public string Concat(string a, string b)
        {
            Console.WriteLine("CONCAT!!!");
            return a + b;
        }

        public void Empty()
        {
            Console.WriteLine("F!!!");
        }
    }

    public class Basic : Aspect
    {
        override protected IEnumerable<Advice<T>> Advise<T>(MethodInfo method)
        {
            if (method.GetBaseDefinition().DeclaringType == Metadata<T>.Type)
            {
                yield return Advice<T>.Basic.Before(() => Console.Write("[1] Basic.Before"));
                yield return Advice<T>.Basic.After(() => Console.WriteLine("[2] Basic.After"));
                yield return Advice<T>.Basic.After((_Instance, _Arguments) => Console.WriteLine("[3] Basic.After : {0}.{1}({2})", _Instance, method.Name, string.Join(", ", _Arguments)));
                yield return Advice<T>.Basic.After.Returning(() => Console.WriteLine("[4] Basic.After Returning"));
                yield return Advice<T>.Basic.After.Returning((_Instance, _Arguments) => Console.WriteLine("[3] Basic.After : {0}.{1}({2})", _Instance, method.Name, string.Join(", ", _Arguments)));
                yield return Advice<T>.Basic.After.Returning((_Instance, _Arguments, _Return) => Console.WriteLine("[3] Basic.After : {0}.{1}({2}) = {3}", _Instance, method.Name, string.Join(", ", _Arguments), _Return));
                yield return Advice<T>.Basic.After.Throwing(() => Console.WriteLine("[4] Basic.After Returning"));
                yield return Advice<T>.Basic.After.Throwing((_Instance, _Arguments) => Console.WriteLine("[3] Basic.After : {0}.{1}({2})", _Instance, method.Name, string.Join(", ", _Arguments)));
                yield return Advice<T>.Basic.After.Throwing((_Instance, _Arguments, _Exception) => Console.WriteLine("[3] Basic.After : {0}.{1}({2}) = {3}", _Instance, method.Name, string.Join(", ", _Arguments), _Exception));
                yield return Advice<T>.Basic.Around(_Proceed =>
                {
                    Console.Write("[1] Basic.Around.Before");
                    _Proceed();
                    Console.Write("[1] Basic.Around.After");
                });
                yield return Advice<T>.Basic.Around((_Instance, _Arguments, _Proceed) =>
                {
                    Console.WriteLine("[3] Basic.Around.Before : {0}.{1}({2})", _Instance, method.Name, string.Join(", ", _Arguments));
                    _Proceed();
                    Console.WriteLine("[3] Basic.Around.After : {0}.{1}({2})", _Instance, method.Name, string.Join(", ", _Arguments));
                });
                yield return Advice<T>.Basic.Around((_Instance, _Arguments, _Proceed) =>
                {
                    Console.WriteLine("[3] Basic.Around.Before : {0}.{1}({2})", _Instance, method.Name, string.Join(", ", _Arguments));
                    var _return = _Proceed();
                    Console.WriteLine("[3] Basic.Around.After : {0}.{1}({2}) = {3}", _Instance, method.Name, string.Join(", ", _Arguments), _return);
                    return _return;
                });
            }
        }
    }

    public class Linq : Aspect
    {
        override protected IEnumerable<Advice<T>> Advise<T>(MethodInfo method)
        {
            if (method.GetBaseDefinition().DeclaringType == Metadata<T>.Type)
            {
                yield return Advice<T>.Linq.Before(Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[1] Linq.Before")));
                yield return Advice<T>.Linq.Before((_Instance, _Arguments) => Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[2] Linq.Before")));
                yield return Advice<T>.Linq.After(Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[1] Linq.After")));
                yield return Advice<T>.Linq.After((_Instance, _Arguments) => Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[2] Linq.After")));
                yield return Advice<T>.Linq.After.Returning(Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[1] Linq.After.Returning")));
                yield return Advice<T>.Linq.After.Returning((_Instance, _Arguments) => Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[2] Linq.After.Returning")));
                yield return Advice<T>.Linq.After.Returning((_Instance, _Arguments, _Return) => Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[2] Linq.After.Returning")));
                yield return Advice<T>.Linq.After.Throwing(Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[1] Linq.After.Throwing")));
                yield return Advice<T>.Linq.After.Throwing((_Instance, _Arguments) => Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[2] Linq.After.Throwing")));
                yield return Advice<T>.Linq.After.Throwing((_Instance, _Arguments, _Exception) => Expression.Call(Metadata.Method(() => Console.WriteLine(Argument<string>.Value)), Expression.Constant("[2] Linq.After.Throwing")));
                yield return Advice<T>.Linq.Around(_Body => _Body);
                yield return Advice<T>.Linq.Around((_Instance, _Arguments, _Body) => _Body);
            }
        }
    }

    public class Reflection : Aspect
    {
        override protected IEnumerable<Advice<T>> Advise<T>(MethodInfo method)
        {
            if (method.GetBaseDefinition().DeclaringType == Metadata<T>.Type)
            {
                yield return Advice<T>.Reflection.Before(_ILGenerator => _ILGenerator.EmitWriteLine("Hello World"));
                yield return Advice<T>.Reflection.After(_ILGenerator => _ILGenerator.EmitWriteLine("Hello World"));
                yield return Advice<T>.Reflection.After.Returning(_ILGenerator => _ILGenerator.EmitWriteLine("Hello World"));
                yield return Advice<T>.Reflection.After.Throwing(_ILGenerator => _ILGenerator.EmitWriteLine("Hello World"));
                yield return Advice<T>.Reflection.Around((_ILGenerator, _Body) => _Body());
            }
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            var _basic = new Basic();
            _basic.Manage<Calculator>();

            var _linq = new Linq();
            _linq.Manage<Calculator>();

            var _reflection = new Linq();
            _reflection.Manage<Calculator>();
            
            var calculator = new Calculator();

            for (var i = 0; i < 1000000000; i++)
            {
                calculator.Empty();
                Console.WriteLine();
                calculator.Add(2, 8);
                Console.WriteLine();
                Calculator.Concat("2", "8");
            }
            Console.ReadLine();
        }
    }
}
