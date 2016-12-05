using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advice
    {
        public interface IReflection<T>
            where T : class
        {
            /// <summary>
            /// After
            /// </summary>
            Advice.Reflection<T>.IAfter After { get; }

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

        public partial class Reflection<T> : Advice.IReflection<T>
            where T : class
        {
            private readonly Advice.Reflection<T>.IAfter m_After = new Advice.Reflection<T>.After();

            Advice.Reflection<T>.IAfter Advice.IReflection<T>.After
            {
                get { return this.m_After; }
            }
        }
    }
}