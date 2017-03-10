using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Reflection
{
    /// <summary>
    /// CIL Instruction.
    /// </summary>
    [DebuggerDisplay("{this.ToString(), nq}")]
    public class Instruction
    {
        /// <summary>
        /// Code.
        /// </summary>
        public readonly OpCode Code;

        /// <summary>
        /// Type.
        /// </summary>
        public readonly Type Type;

        /// <summary>
        /// Value.
        /// </summary>
        public readonly object Value;

        /// <summary>
        /// Create an instruction without value.
        /// </summary>
        /// <param name="code"></param>
        public Instruction(OpCode code)
        {
            this.Code = code;
            this.Type = null;
            this.Value = null;
        }

        internal Instruction(OpCode code, Type type, object value)
        {
            this.Code = code;
            this.Type = type;
            this.Value = value;
        }

        virtual internal void Push(ILGenerator body)
        {
            body.Emit(this.Code);
        }

        /// <summary>
        /// Instruction illustration.
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            if (this.Type == null) {return this.Code.ToString(); }
            if (this.Value == null) { return string.Concat(this.Code.ToString(), ", ", this.Type.Name, " = [NULL]"); }
            if (this.Type == Metadata<Label>.Type) { return string.Concat(this.Code.ToString(), ", ", this.Type.Name, " = [", ((Label)this.Value).Value().ToString(), "]"); }
            return string.Concat(this.Code.ToString(), ", ", this.Type.Name, " = [", this.Value.ToString(), "]");
        }
    }

    /// <summary>
    /// Instruction
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public sealed class Instruction<T> : Instruction
    {
        static private class Initialization
        {
            static public Action<ILGenerator, Instruction> Push()
            {
                if (Metadata<T>.Type == Metadata<byte>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (byte)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<sbyte>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (sbyte)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<short>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (short)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<int>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (int)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<long>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (long)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<float>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (float)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<double>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (double)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<string>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as string)); }
                if (Metadata<T>.Type == Metadata<LocalBuilder>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as LocalBuilder)); }
                if (Metadata<T>.Type == Metadata<Label>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, (Label)_Instruction.Value)); }
                if (Metadata<T>.Type == Metadata<Label[]>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as Label[])); }
                if (Metadata<T>.Type == Metadata<Type>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as Type)); }
                if (Metadata<T>.Type == Metadata<FieldInfo>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as FieldInfo)); }
                if (Metadata<T>.Type == Metadata<MethodInfo>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as MethodInfo)); }
                if (Metadata<T>.Type == Metadata<ConstructorInfo>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as ConstructorInfo)); }
                if (Metadata<T>.Type == Metadata<SignatureHelper>.Type) { return new Action<ILGenerator, Instruction>((_Body, _Instruction) => _Body.Emit(_Instruction.Code, _Instruction.Value as SignatureHelper)); }
                throw new NotSupportedException();
            }
        }

        static private readonly Action<ILGenerator, Instruction> m_Push = Initialization.Push();

        /// <summary>
        /// Value.
        /// </summary>
        new public readonly T Value;

        /// <summary>
        /// Create an instruction.
        /// </summary>
        /// <param name="code">Code</param>
        /// <param name="value">Value</param>
        public Instruction(OpCode code, T value)
            : base(code, Metadata<T>.Type, value)
        {
            this.Value = value;
        }

        override internal void Push(ILGenerator body)
        {
            Instruction<T>.m_Push(body, this);
        }
    }
}
