using System;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    internal sealed class Junction
    {
        public readonly MethodInfo Method;
        public readonly Type Type;
        internal readonly IntPtr Pointer;
        public readonly Signature Signature;

        public Junction(Junction junction, IntPtr pointer)
        {
            this.Method = junction.Method;
            this.Type = junction.Type;
            this.Pointer = pointer;
            this.Signature = junction.Signature;
        }

        public Junction(MethodInfo method, Type type, Signature signature)
        {
            this.Method = method;
            this.Type = type;
            this.Pointer = method.Pointer();
            this.Signature = signature;
        }
    }
}