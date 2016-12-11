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
        static public readonly Advisor.IBasic Basic = new Advisor.Basic();

        /// <summary>
        /// Linq
        /// </summary>
        static public readonly Advisor.ILinq Linq = new Advisor.Linq();

        /// <summary>
        /// Reflection
        /// </summary>
        static public readonly Advisor.IReflection Reflection = new Advisor.Reflection();

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

        public Advice(Func<MethodInfo, IntPtr, MethodInfo> decorate)
        {
            this.m_Decorate = decorate;
        }

        public Advice(Func<MethodInfo, MethodInfo> decorate)
        {
            this.m_Decorate = new Func<MethodInfo, IntPtr, MethodInfo>((_Method, _Pointer) => decorate(_Method));
        }

        public MethodInfo Decorate(MethodInfo method, IntPtr pointer)
        {
            return this.m_Decorate(method, pointer);
        }
    }
}