using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NConcern.Debug
{
    public class Calculator
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public async Task<int> AddAsync(int a, int b)
        {
            return await Task.Run(() => this.Add(a, b));
        }

        public int AddFailed(int a, int b)
        {
            throw new InvalidOperationException("bug");
        }

        public async Task<int> AddFailedAsync(int a, int b)
        {
            return await Task.Run(() => this.AddFailed(a, b));
        }
    }

    public class Resource1 : IDisposable
    {
        public Resource1()
        {
            Console.WriteLine("resource 1 : begin");
        }

        public void Dispose()
        {
            Console.WriteLine("resource 1 : end");
        }
    }

    public class Resource2 : IDisposable
    {
        public Resource2()
        {
            Console.WriteLine("resource 2 : begin");
        }

        public void Dispose()
        {
            Console.WriteLine("resource 2 : end");
        }
    }

    public class ResourceTechnicalAspect<T> : IAspect
        where T : IDisposable, new()
    {
        public IEnumerable<IAdvice> Advise(MethodBase method)
        {
            yield return Advice.Basic.Around(_Body =>
            {
                using (new T())
                {
                    _Body();
                }
            });
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            Aspect.Weave<ResourceTechnicalAspect<Resource1>>(Metadata<Calculator>.Method(_Calculator => _Calculator.Add(Argument<int>.Value, Argument<int>.Value)));
            Aspect.Weave<ResourceTechnicalAspect<Resource2>>(Metadata<Calculator>.Method(_Calculator => _Calculator.AddAsync(Argument<int>.Value, Argument<int>.Value)));

            Aspect.Weave<ResourceTechnicalAspect<Resource1>>(Metadata<Calculator>.Method(_Calculator => _Calculator.AddFailed(Argument<int>.Value, Argument<int>.Value)));
            Aspect.Weave<ResourceTechnicalAspect<Resource2>>(Metadata<Calculator>.Method(_Calculator => _Calculator.AddFailedAsync(Argument<int>.Value, Argument<int>.Value)));

            //Create calculator.
            var _calculator = new Calculator();

            //Call synchronous method.
            var _result = _calculator.Add(2, 3);

            //Call async method.
            int _result1 = _calculator.AddAsync(2, 3).Result;

            //Call synchronous method.
            try
            {
                var _result2 = _calculator.AddFailed(2, 3);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //Call async method.
            try
            {
                int _result3 = _calculator.AddFailedAsync(2, 3).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
