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

        private readonly Func<MethodInfo, IntPtr, MethodInfo> m_Decorate;

        /// <summary>
        /// Create an advice, a way to decorate.
        /// </summary>
        /// <param name="decorate">Delegate use to decorate a method : Func(MethodInfo = [base method to decorate], IntPtr = [pointer to base method]) return replacing method</param>
        public Advice(Func<MethodInfo, IntPtr, MethodInfo> decorate)
        {
            this.m_Decorate = decorate;
        }

        /// <summary>
        /// Create an advice, a way to decorate.
        /// </summary>
        /// <param name="decorate">Delegate use to decorate a method : Func(MethodInfo = [base method to decorate]) return replacing method</param>
        public Advice(Func<MethodInfo, MethodInfo> decorate)
        {
            this.m_Decorate = new Func<MethodInfo, IntPtr, MethodInfo>((_Method, _Pointer) => decorate(_Method));
        }

        /// <summary>
        /// Create an advice with a specific replacing method.
        /// </summary>
        /// <param name="method">Replacing method</param>
        public Advice(MethodInfo method)
        {
            this.m_Decorate = new Func<MethodInfo, IntPtr, MethodInfo>((_Method, _Pointer) => method);
        }

        /// <summary>
        /// Decorate a method for a specific concern.
        /// </summary>
        /// <param name="method">Base method to decorate</param>
        /// <param name="pointer">Pointer to base method</param>
        /// <returns>Replacing method</returns>
        public MethodInfo Decorate(MethodInfo method, IntPtr pointer)
        {
            return this.m_Decorate(method, pointer);
        }
    }
}