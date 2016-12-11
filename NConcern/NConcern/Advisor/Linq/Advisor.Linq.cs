using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advisor
    {
        public interface ILinq
        {
            /// <summary>
            /// After
            /// </summary>
            Advisor.Linq.IAfter After { get; }

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

        public partial class Linq : ILinq
        {
            private readonly Advisor.Linq.IAfter m_After = new Advisor.Linq.After();

            Advisor.Linq.IAfter Advisor.ILinq.After
            {
                get { return this.m_After; }
            }
        }
    }
}