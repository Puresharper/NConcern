using System;
using System.Diagnostics;
using System.Reflection;

namespace NConcern.Example.Logging
{
    public class Trace
    {
        private Stopwatch m_Stopwatch;
        
        public Trace(MethodInfo method, string[] arguments)
        {
            this.m_Stopwatch = new Stopwatch();
            Console.WriteLine("[{0}] >>> {1}.{2}({3})", DateTime.Now, method.DeclaringType.Name, method.Name, string.Join(", ", arguments));
            this.m_Stopwatch.Start();
        }

        public void Dispose(Exception exception)
        {
            this.m_Stopwatch.Stop();
            Console.WriteLine("[{0}] <<< duration = {1} | excepton = {2}", DateTime.Now, this.m_Stopwatch.Elapsed, exception.Message);
        }

        public void Dispose(string @return)
        {
            this.m_Stopwatch.Stop();
            Console.WriteLine("[{0}] <<< duration = {1} | return = {2}", DateTime.Now, this.m_Stopwatch.Elapsed, @return);
        }
    }
}
