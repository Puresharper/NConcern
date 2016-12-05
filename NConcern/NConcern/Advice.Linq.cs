using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advice
    {
        public interface ILinq<T>
            where T : class
        {
            /// <summary>
            /// After
            /// </summary>
            Advice.Linq<T>.IAfter After { get; }

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            int GetHashCode();

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            string ToString();

            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            Type GetType();
        }

        public partial class Linq<T> : ILinq<T>
            where T : class
        {
            private readonly Advice.Linq<T>.IAfter m_After = new Advice.Linq<T>.After();

            Advice.Linq<T>.IAfter Advice.ILinq<T>.After
            {
                get { return this.m_After; }
            }
        }
    }
}