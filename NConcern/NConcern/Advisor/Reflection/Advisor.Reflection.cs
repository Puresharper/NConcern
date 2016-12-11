using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advisor
    {
        public interface IReflection
        {
            /// <summary>
            /// After
            /// </summary>
            Advisor.Reflection.IAfter After { get; }

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

        public partial class Reflection : Advisor.IReflection
        {
            private readonly Advisor.Reflection.IAfter m_After = new Advisor.Reflection.After();

            Advisor.Reflection.IAfter Advisor.IReflection.After
            {
                get { return this.m_After; }
            }
        }
    }
}