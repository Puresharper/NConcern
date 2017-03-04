using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace System.Reflection
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __MethodBase
    {
        static private class Initialization
        {
            static public OpCode[] Single()
            {
                var _single = new OpCode[0x100];
                foreach (var _field in Metadata<OpCodes>.Type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var _code = (OpCode)_field.GetValue(null);
                    var _value = (ushort)_code.Value;
                    if (_value < 0x100) { _single[_value] = _code; }
                }
                return _single;
            }

            static public OpCode[] Double()
            {
                var _double = new OpCode[0x100];
                foreach (var _Field in Metadata<OpCodes>.Type.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                    var _code = (OpCode)_Field.GetValue(null);
                    var _value = (ushort)_code.Value;
                    if (_value < 0x100) { continue; }
                    if ((_value & 0xff00) == 0xfe00) { _double[_value & 0xff] = _code; }
                }
                return _double;
            }
        }

        static private readonly OpCode[] m_Single = Initialization.Single();
        static private readonly OpCode[] m_Double = Initialization.Double();
        static private readonly Func<DynamicMethod, RuntimeMethodHandle> m_Handle = Delegate.CreateDelegate(Metadata<Func<DynamicMethod, RuntimeMethodHandle>>.Type, Metadata<DynamicMethod>.Type.GetMethod("GetMethodDescriptor", BindingFlags.Instance | BindingFlags.NonPublic)) as Func<DynamicMethod, RuntimeMethodHandle>;

        static private IEnumerable<Instruction> Body(this MethodBase method, byte[] buffer)
        {
            if (buffer == null) { yield break; }
            var _authority = new Authority(method);
            var _stream = new MemoryStream(buffer);
            var _reader = new BinaryReader(_stream);
            var _code = OpCodes.Nop;
            while (_stream.Position < _stream.Length)
            {
                var _byte = _reader.ReadByte();
                if (_byte != 0xFE) { _code = __MethodBase.m_Single[_byte]; }
                else
                {
                    _byte = _reader.ReadByte();
                    _code = __MethodBase.m_Double[_byte];
                }
                switch (_code.OperandType)
                {
                    case OperandType.InlineNone:
                        yield return new Instruction(_code, null, null);
                        break;
                    case OperandType.ShortInlineBrTarget:
                        yield return new Instruction<Label>(_code, _reader.ReadShortLabel());
                        break;
                    case OperandType.InlineBrTarget:
                        yield return new Instruction<Label>(_code, _reader.ReadLabel());
                        break;
                    case OperandType.ShortInlineI:
                        yield return new Instruction<byte>(_code, _reader.ReadByte());
                        break;
                    case OperandType.InlineI:
                        yield return new Instruction<int>(_code, _reader.ReadInt32());
                        break;
                    case OperandType.InlineI8:
                        yield return new Instruction<long>(_code, _reader.ReadInt64());
                        break;
                    case OperandType.ShortInlineR:
                        yield return new Instruction<float>(_code, _reader.ReadSingle());
                        break;
                    case OperandType.InlineR:
                        yield return new Instruction<double>(_code, _reader.ReadDouble());
                        break;
                    case OperandType.ShortInlineVar:
                        yield return new Instruction<byte>(_code, _reader.ReadByte());
                        break;
                    case OperandType.InlineVar:
                        yield return new Instruction<short>(_code, _reader.ReadInt16());
                        break;
                    case OperandType.InlineString:
                        yield return new Instruction<string>(_code, _authority.String(_reader.ReadInt32()));
                        break;
                    case OperandType.InlineSig:
                        yield return new Instruction<byte[]>(_code, _authority.Signature(_reader.ReadInt32()));
                        break;
                    case OperandType.InlineField:
                        yield return new Instruction<FieldInfo>(_code, _authority.Field(_reader.ReadInt32(), method.DeclaringType == null ? new Type[0] : method.DeclaringType.GetGenericArguments(), method.IsGenericMethod ? method.GetGenericArguments() : new Type[0]));
                        break;
                    case OperandType.InlineMethod:
                        var _method = _authority.Method(_reader.ReadInt32(), method.DeclaringType == null ? new Type[0] : method.DeclaringType.GetGenericArguments(), method.IsGenericMethod ? method.GetGenericArguments() : new Type[0]);
                        if (_method is ConstructorInfo) { yield return new Instruction<ConstructorInfo>(_code, (ConstructorInfo)_method); }
                        else { yield return new Instruction<MethodInfo>(_code, (MethodInfo)_method); }
                        break;
                    case OperandType.InlineType:
                        yield return new Instruction<Type>(_code, _authority.Type(_reader.ReadInt32(), method.DeclaringType == null ? new Type[0] : method.DeclaringType.GetGenericArguments(), method.IsGenericMethod ? method.GetGenericArguments() : new Type[0]));
                        break;
                    case OperandType.InlineTok:
                        var _member = (MemberInfo)_authority.Member(_reader.ReadInt32(), method.DeclaringType == null ? new Type[0] : method.DeclaringType.GetGenericArguments(), method.IsGenericMethod ? method.GetGenericArguments() : new Type[0]);
                        if (_member is FieldInfo) { yield return new Instruction<FieldInfo>(_code, (FieldInfo)_member); }
                        else if (_member is ConstructorInfo) { yield return new Instruction<ConstructorInfo>(_code, (ConstructorInfo)_member); }
                        else if (_member is MethodInfo) { yield return new Instruction<MethodInfo>(_code, (MethodInfo)_member); }
                        else if (_member is Type) { yield return new Instruction<Type>(_code, (Type)_member); }
                        break;
                    case OperandType.InlineSwitch:
                        yield return new Instruction<Label[]>(OpCodes.Switch, Enumerable.Range(0, Convert.ToInt32(_reader.ReadUInt32())).Select(_Iterator => _reader.ReadLabel()).ToArray());
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        static internal byte[] GetBodyAsByteArray(this MethodBase method)
        {
            method.Prepare();
            if (method is DynamicMethod) { return (method as DynamicMethod).GetILGenerator().GetILAsByteArray(); }
            var _body = method.GetMethodBody();
            return _body == null ? null : _body.GetILAsByteArray();
        }

        static internal byte[] GetILAsByteArray(this ILGenerator body)
        {
            var fiBytes = Metadata<ILGenerator>.Type.GetField("m_ILStream", BindingFlags.Instance | BindingFlags.NonPublic);
            var fiLength = Metadata<ILGenerator>.Type.GetField("m_length", BindingFlags.Instance | BindingFlags.NonPublic);
            var il = fiBytes.GetValue(body) as byte[];
            var _buffer = new byte[(int)fiLength.GetValue(body)];
            Array.Copy(il, _buffer, _buffer.Length);
            return _buffer;
        }

        static public Collection<Instruction> Body(this MethodBase method)
        {
            return new Collection<Instruction>(method.Body(method.GetBodyAsByteArray()));
        }

        static public bool Attributed<T>(this MethodBase method)
            where T : Attribute
        {
            return System.Attribute.IsDefined(method, Metadata<T>.Type);
        }

        static public T Attribute<T>(this MethodBase method)
            where T : Attribute
        {
            return System.Attribute.GetCustomAttribute(method, Metadata<T>.Type) as T;
        }

        static public IEnumerable<T> Attributes<T>(this MethodBase method)
            where T : Attribute
        {
            return System.Attribute.GetCustomAttributes(method, Metadata<T>.Type).Cast<T>();
        }

        static public RuntimeMethodHandle Handle(this MethodBase method)
        {
            return method is DynamicMethod ? __MethodBase.m_Handle(method as DynamicMethod) : method.MethodHandle;
        }

        static public void Prepare(this MethodBase method)
        {
            RuntimeHelpers.PrepareMethod(method.Handle());
        }

        static public string Declaration(this MethodBase method)
        {
            if (method.IsGenericMethod)
            {
                return string.Concat(method.Name, "<", string.Join(", ", method.GetGenericArguments().Select(__Type.Declaration)), ">(", string.Join(", ", method.GetParameters().Select(__ParameterInfo.Declaration)), ")");
            }
            return string.Concat(method.Name, "(", string.Join(", ", method.GetParameters().Select(__ParameterInfo.Declaration)), ")");
        }

        static public IntPtr Pointer(this MethodBase method)
        {
            return method.Handle().GetFunctionPointer();
        }

        static public Signature Signature(this MethodBase method)
        {
            if (method.IsStatic) { return new Signature(method.GetParameters().Select(_Parameter => _Parameter.ParameterType)); }
            return new Signature(method.DeclaringType, method.GetParameters().Select(_Parameter => _Parameter.ParameterType));
        }

        static public PropertyInfo Property(this MethodBase method)
        {
            if (method is ConstructorInfo) { return null; }
            foreach (var _property in method.DeclaringType.Properties())
            {
                if (method == _property.GetGetMethod(true) || method == _property.GetSetMethod(true)) { return _property; }
            }
            return null;
        }

        static public MethodBase GetBaseDefinition(this MethodBase method)
        {
            return method is MethodInfo ? (method as MethodInfo).GetBaseDefinition() : method;
        }

        static public Type Type(this MethodBase method)
        {
            return method is MethodInfo ? (method as MethodInfo).ReturnType : Metadata.Void;
        }
    }
}