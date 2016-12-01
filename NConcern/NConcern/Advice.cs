using System;
using System.ComponentModel;
using System.Diagnostics;

namespace NConcern
{
    /// <summary>
    /// Code to complete a method execution.
    /// </summary>
    abstract public class Advice
    {
        /// <summary>
        /// Styles of advice.
        /// </summary>
        public enum Styles
        {
            Generation = 0,
            Expression = 1,
            Delegation = 2,
            Reflection = 4
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        /// <summary>
        /// Style of advice.
        /// </summary>
        public readonly Styles Style;

        internal Advice(Styles style)
        {
            this.Style = style;
        }

        abstract internal Aspect.Activity<T> Override<T>(Aspect.Activity<T> activity)
            where T : class;

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        sealed override public int GetHashCode()
        {
            return base.GetHashCode();
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        sealed override public string ToString()
        {
            return base.ToString();
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public Type GetType()
        {
            return base.GetType();
        }
    }
}