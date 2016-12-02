using System;
using System.Reflection;
using System.Reflection.Emit;

namespace System.Runtime
{
    static internal partial class Closure
    {
        public sealed class Function
        {
            static private ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();
            static private Type[] m_Signature = new Type[] { Metadata<object>.Type, Metadata<object[]>.Type };

            public readonly Type Type;
            public readonly ConstructorInfo Constructor;
            public readonly MethodInfo Method;

            public Function(IntPtr method, Signature signature, Type type)
            {
                var _type = Closure.Function.m_Module.DefineType(string.Concat(Metadata<Type>.Type.Name, Guid.NewGuid().ToString("N")), TypeAttributes.Class | TypeAttributes.Public, Metadata<object>.Type);
                var _method = _type.DefineMethod(Metadata<Routine>.Type.Name, MethodAttributes.Public, CallingConventions.HasThis, Metadata<object>.Type, Closure.Function.m_Signature).GetILGenerator();
                if (signature.Instance != null)
                {
                    _method.Emit(OpCodes.Ldarg_1);
                    _method.Emit(OpCodes.Castclass, signature.Instance);
                }
                for (var _index = 0; _index < signature.Parameters.Count; _index++)
                {
                    var _parameter = signature.Parameters[0];
                    switch (_index)
                    {
                        case 0:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_0);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 1:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_1);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 2:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_2);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 3:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_3);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 4:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_4);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 5:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_5);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 6:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_6);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 7:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_7);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        case 8:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4_8);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                        default:
                            _method.Emit(OpCodes.Ldarg_2);
                            _method.Emit(OpCodes.Ldc_I4, _index);
                            _method.Emit(OpCodes.Ldelem_Ref);
                            if (_parameter.IsValueType) { _method.Emit(OpCodes.Unbox_Any, _parameter); }
                            else { _method.Emit(OpCodes.Castclass, _parameter); }
                            break;
                    }
                }
                _method.Emit(method, type, signature);
                if (type == Metadata.Void) { _method.Emit(OpCodes.Ldnull); }
                else
                {
                    if (type.IsValueType) { _method.Emit(OpCodes.Box, type); }
                    else { _method.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                }
                _method.Emit(OpCodes.Ret);
                this.Type = _type.CreateType();
                this.Constructor = this.Type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
                this.Method = this.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)[0];
            }
        }
    }
}
