using System;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime
{
    static internal partial class Closure
    {
        public sealed class Routine
        {
            static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

            public readonly Type Type;
            public readonly ConstructorInfo Constructor;
            public readonly FieldInfo Value;
            public readonly MethodInfo Method;

            public Routine(IntPtr method, Signature signature, Type type)
            {
                var _type = Closure.Routine.m_Module.DefineType(string.Concat(Metadata<Type>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Class | TypeAttributes.Public, Metadata<object>.Type);
                var _constructor = _type.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, signature).GetILGenerator();
                var _method = _type.DefineMethod(Metadata<Routine>.Type.Name, MethodAttributes.Public, CallingConventions.HasThis, Metadata.Void, Type.EmptyTypes).GetILGenerator();
                if (type == Metadata.Void)
                {
                    _constructor.Emit(OpCodes.Ldarg_0);
                    _constructor.Emit(OpCodes.Call, Metadata.Constructor(() => new object()));
                    for (var _index = 0; _index < signature.Length; _index++)
                    {
                        var _field = _type.DefineField(_index.ToString(), signature[_index], FieldAttributes.Public);
                        _constructor.Emit(OpCodes.Ldarg_0);
                        switch (_index)
                        {
                            case 0: _constructor.Emit(OpCodes.Ldarg_1); break;
                            case 1: _constructor.Emit(OpCodes.Ldarg_2); break;
                            case 2: _constructor.Emit(OpCodes.Ldarg_3); break;
                            default: _constructor.Emit(OpCodes.Ldarg, _index); break;
                        }
                        _constructor.Emit(OpCodes.Stfld, _field);
                        _method.Emit(OpCodes.Ldarg_0);
                        _method.Emit(OpCodes.Ldfld, _field);
                    }
                    _constructor.Emit(OpCodes.Ret);
                    _method.Emit(method, type, signature);
                    _method.Emit(OpCodes.Ret);
                    this.Type = _type.CreateType();
                    this.Constructor = this.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
                    this.Method = this.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
                }
                else
                {
                    _constructor.Emit(OpCodes.Ldarg_0);
                    _constructor.Emit(OpCodes.Call, Metadata.Constructor(() => new object()));
                    _method.Emit(OpCodes.Ldarg_0);
                    for (var _index = 0; _index < signature.Length; _index++)
                    {
                        var _field = _type.DefineField(_index.ToString(), signature[_index], FieldAttributes.Public);
                        _constructor.Emit(OpCodes.Ldarg_0);
                        switch (_index)
                        {
                            case 0: _constructor.Emit(OpCodes.Ldarg_1); break;
                            case 1: _constructor.Emit(OpCodes.Ldarg_2); break;
                            case 2: _constructor.Emit(OpCodes.Ldarg_3); break;
                            default: _constructor.Emit(OpCodes.Ldarg, _index); break;
                        }
                        _constructor.Emit(OpCodes.Stfld, _field);
                        _method.Emit(OpCodes.Ldarg_0);
                        _method.Emit(OpCodes.Ldfld, _field);
                    }
                    _constructor.Emit(OpCodes.Ret);
                    _method.Emit(method, type, signature);
                    _method.Emit(OpCodes.Stfld, _type.DefineField(Metadata<Routine>.Field(_Action => _Action.Value).Name, type, FieldAttributes.Public));
                    _method.Emit(OpCodes.Ret);
                    this.Type = _type.CreateType();
                    this.Constructor = this.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
                    this.Method = this.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
                    this.Value = this.Type.GetField(Metadata<Routine>.Field(_Action => _Action.Value).Name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
                }
            }
        }
    }
}
