using System;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Reflection.Emit
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __ModuleBuilder
    {
        [DebuggerHidden]
        static public FieldInfo DefineField(this ModuleBuilder module, string name, Type type)
        {
            var _type = module.DefineType(string.Concat(Metadata<Type>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Class | TypeAttributes.Sealed | TypeAttributes.Abstract | TypeAttributes.Public);
            _type.DefineField(name, type, FieldAttributes.Static | FieldAttributes.Public);
            return _type.CreateType().GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
        }

        static public FieldInfo DefineField(this ModuleBuilder module, string name, Type type, object value)
        {
            var _field = module.DefineField(name, type);
            _field.SetValue(null, value);
            return _field;
        }

        [DebuggerHidden]
        static public FieldInfo DefineField<T>(this ModuleBuilder module, string name)
        {
            return module.DefineField(name, Metadata<T>.Type);
        }

        [DebuggerHidden]
        static public FieldInfo DefineField<T>(this ModuleBuilder module, string name, T value)
        {
            return module.DefineField(name, Metadata<T>.Type, value);
        }
    }
}
