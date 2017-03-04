using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace NConcern.Performance
{
    public class Logic
    {
        public void Method1()
        {
        }

        public void Method2()
        {
            
        }

        public void Method3()
        {
            MethodBase.GetCurrentMethod().Name
            Tracer.Trace();
            this.Method2();
        }

        public void Method4()
        {
            Tracer.Trace();
        }

        public void Method5()
        {
        }
    }

    public class Tracer
    {
        static public void Trace()
        {
        }
    }

    public class Aspect1 : IAspect
    {
        public IEnumerable<IAdvice> Advise(MethodInfo method)
        {
            //yield return new Advice(Metadata<Logic>.Method(l => l.Method3()));
            //yield return Advice.Linq.Before(Expression.Call(Metadata.Method(() => Tracer.Trace())));
            //yield return Advice.Reflection.Before(body =>
            //{
            //    body.Emit(OpCodes.Call, Metadata.Method(() => Tracer.Trace()));
            //});
            //yield return Advice.Linq.Around(e => e);
            yield return Advice.Basic.Before(Tracer.Trace);
        }
    }

    class Program
    {
        [MethodImpl(MethodImplOptions.NoOptimization)]
        static void Main(string[] args)
        {
            Aspect.Weave<Aspect1>(Metadata<Logic>.Method(_Logic => _Logic.Method1()));
            var _logic = new Logic();
            var _sonar = new Stopwatch();
            _sonar.Restart();
            for (var _index = 0; _index < 10000000; _index++)
            {
                _logic.Method1();
            }
            _sonar.Stop();
            Console.WriteLine(_sonar.Elapsed);
            _sonar.Restart();
            for (var _index = 0; _index < 10000000; _index++)
            {
                _logic.Method3();
            }
            _sonar.Stop();
            Console.WriteLine(_sonar.Elapsed);
            _sonar.Restart();
            for (var _index = 0; _index < 10000000; _index++)
            {
                _logic.Method4();
            }
            _sonar.Stop();
            Console.WriteLine(_sonar.Elapsed);
        }
    }
}
