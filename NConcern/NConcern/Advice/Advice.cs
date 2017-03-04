using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    public sealed class Advice : IAdvice
    {
        /// <summary>
        /// Basic
        /// </summary>
        static public readonly Advisor.IBasic Basic = null;

        /// <summary>
        /// Linq
        /// </summary>
        static public readonly Advisor.ILinq Linq = null;

        /// <summary>
        /// Reflection
        /// </summary>
        static public readonly Advisor.IReflection Reflection = null;

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        private readonly Func<MethodBase, IntPtr, MethodBase> m_Decorate;

        /// <summary>
        /// Create an advice, a way to decorate.
        /// </summary>
        /// <param name="decorate">Delegate use to decorate a method : Func(MethodBase = [base method to decorate], IntPtr = [pointer to base method]) return replacing method</param>
        public Advice(Func<MethodBase, IntPtr, MethodBase> decorate)
        {
            this.m_Decorate = decorate;
        }

        /// <summary>
        /// Create an advice, a way to decorate.
        /// </summary>
        /// <param name="decorate">Delegate use to decorate a method : Func(MethodBase = [base method to decorate]) return replacing method</param>
        public Advice(Func<MethodBase, MethodBase> decorate)
        {
            this.m_Decorate = new Func<MethodBase, IntPtr, MethodBase>((_Method, _Pointer) => decorate(_Method));
        }

        /// <summary>
        /// Create an advice with a specific replacing method.
        /// </summary>
        /// <param name="method">Replacing method</param>
        public Advice(MethodBase method)
        {
            this.m_Decorate = new Func<MethodBase, IntPtr, MethodBase>((_Method, _Pointer) => method);
        }

        /// <summary>
        /// Decorate a method for a specific concern.
        /// </summary>
        /// <param name="method">Base method to decorate</param>
        /// <param name="pointer">Pointer to base method</param>
        /// <returns>Replacing method</returns>
        public MethodBase Decorate(MethodBase method, IntPtr pointer)
        {
            return this.m_Decorate(method, pointer);
        }
    }
}