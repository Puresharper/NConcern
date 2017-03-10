using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    static public partial class Advisor
    {
        /// <summary>
        /// Basic.
        /// </summary>
        public interface IBasic
        {
            /// <summary>
            /// GetHashCode.
            /// </summary>
            /// <returns>HashCode</returns>
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            int GetHashCode();

            /// <summary>
            /// ToString.
            /// </summary>
            /// <returns>String</returns>
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            string ToString();

            /// <summary>
            /// GetType.
            /// </summary>
            /// <returns>Type</returns>
            [DebuggerHidden]
            [EditorBrowsable(EditorBrowsableState.Never)]
            Type GetType();
        }
    }
}