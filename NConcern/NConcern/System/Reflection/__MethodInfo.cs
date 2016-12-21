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
    static internal class ___MethodInfo
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

        static public bool Attributed<T>(this MethodInfo method)
            where T : Attribute
        {
            return System.Attribute.IsDefined(method, Metadata<T>.Type);
        }

        static public T Attribute<T>(this MethodInfo method)
            where T : Attribute
        {
            return System.Attribute.GetCustomAttribute(method, Metadata<T>.Type) as T;
        }

        static public IEnumerable<T> Attributes<T>(this MethodInfo method)
            where T : Attribute
        {
            return System.Attribute.GetCustomAttributes(method, Metadata<T>.Type).Cast<T>();
        }

        static public RuntimeMethodHandle Handle(this MethodInfo method)
        {
            return method is DynamicMethod ? ___MethodInfo.m_Handle(method as DynamicMethod) : method.MethodHandle;
        }

        static public void Prepare(this MethodInfo method)
        {
            RuntimeHelpers.PrepareMethod(method.Handle());
        }

        static public string Declaration(this MethodInfo method)
        {
            if (method.IsGenericMethod)
            {
                return string.Concat(method.Name, "<", string.Join(", ", method.GetGenericArguments().Select(__Type.Declaration)), ">(", string.Join(", ", method.GetParameters().Select(__ParameterInfo.Declaration)), ")");
            }
            return string.Concat(method.Name, "(", string.Join(", ", method.GetParameters().Select(__ParameterInfo.Declaration)), ")");
        }

        static public IntPtr Pointer(this MethodInfo method)
        {
            return method.Handle().GetFunctionPointer();
        }

        static public Signature Signature(this MethodInfo method)
        {
            if (method.IsStatic) { return new Signature(method.GetParameters().Select(_Parameter => _Parameter.ParameterType)); }
            return new Signature(method.DeclaringType, method.GetParameters().Select(_Parameter => _Parameter.ParameterType));
        }

        static public PropertyInfo Property(this MethodInfo method)
        {
            foreach (var _property in method.DeclaringType.Properties())
            {
                if (method == _property.GetGetMethod(true) || method == _property.GetSetMethod(true)) { return _property; }
            }
            return null;
        }
    }
}