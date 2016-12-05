using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advice
    {
        public interface IBasic<T>
            where T : class
        {
            /// <summary>
            /// After
            /// </summary>
            Advice.Basic<T>.IAfter After { get; }

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

        public partial class Basic<T> : Advice.IBasic<T>
            where T : class
        {
            private readonly Advice.Basic<T>.IAfter m_After = new Advice.Basic<T>.After();

            Advice.Basic<T>.IAfter Advice.IBasic<T>.After
            {
                get { return this.m_After; }
            }
        }
    }
}