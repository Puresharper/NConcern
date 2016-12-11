using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advisor
    {
        public interface IBasic
        {
            /// <summary>
            /// After
            /// </summary>
            Advisor.Basic.IAfter After { get; }

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

        public partial class Basic : Advisor.IBasic
        {
            private readonly Advisor.Basic.IAfter m_After = new Advisor.Basic.After();

            Advisor.Basic.IAfter Advisor.IBasic.After
            {
                get { return this.m_After; }
            }
        }
    }
}