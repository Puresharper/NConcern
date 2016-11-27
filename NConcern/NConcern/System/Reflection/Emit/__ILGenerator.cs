using System;
using System.ComponentModel;
using NConcern;

namespace System.Reflection.Emit
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class ___ILGenerator
    {
        static public void Emit(this ILGenerator body, Junction junction)
        {
            var _signature = junction.Signature;
            body.Emit(_signature, false);
            body.Emit(junction.Pointer, junction.Type, _signature);
        }

        static public void Emit(this ILGenerator body, Action<ILGenerator> instruction)
        {
            instruction(body);
        }

        static public void Emit(this ILGenerator body, IntPtr function, Type type, Signature signature)
        {
            switch (IntPtr.Size)
            {
                case 4: body.Emit(OpCodes.Ldc_I4, function.ToInt32()); break;
                case 8: body.Emit(OpCodes.Ldc_I8, function.ToInt64()); break;
                default: throw new NotSupportedException();
            }
            body.EmitCalli(OpCodes.Calli, CallingConventions.Standard, type, signature, null);
        }

        static public void Emit(this ILGenerator body, Signature signature, bool reflective)
        {
            if (reflective)
            {
                if (signature.Instance == null)
                {
                    switch (signature.Parameters.Count)
                    {
                        case 0: body.Emit(OpCodes.Ldc_I4_0); break;
                        case 1: body.Emit(OpCodes.Ldc_I4_1); break;
                        case 2: body.Emit(OpCodes.Ldc_I4_2); break;
                        case 3: body.Emit(OpCodes.Ldc_I4_3); break;
                        case 4: body.Emit(OpCodes.Ldc_I4_4); break;
                        case 5: body.Emit(OpCodes.Ldc_I4_5); break;
                        case 6: body.Emit(OpCodes.Ldc_I4_6); break;
                        case 7: body.Emit(OpCodes.Ldc_I4_7); break;
                        case 8: body.Emit(OpCodes.Ldc_I4_8); break;
                        default: body.Emit(OpCodes.Ldc_I4, signature.Parameters.Count); break;
                    }
                    body.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    for (var _index = 0; _index < signature.Parameters.Count; _index++)
                    {
                        var _parameter = signature.Parameters[_index];
                        body.Emit(OpCodes.Dup);
                        switch (_index)
                        {
                            case 0:
                                body.Emit(OpCodes.Ldc_I4_0);
                                body.Emit(OpCodes.Ldarg_0);
                                break;
                            case 1:
                                body.Emit(OpCodes.Ldc_I4_1);
                                body.Emit(OpCodes.Ldarg_1);
                                break;
                            case 2:
                                body.Emit(OpCodes.Ldc_I4_2);
                                body.Emit(OpCodes.Ldarg_2);
                                break;
                            case 3:
                                body.Emit(OpCodes.Ldc_I4_3);
                                body.Emit(OpCodes.Ldarg_3);
                                break;
                            case 4:
                                body.Emit(OpCodes.Ldc_I4_4);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 5:
                                body.Emit(OpCodes.Ldc_I4_5);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 6:
                                body.Emit(OpCodes.Ldc_I4_6);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 7:
                                body.Emit(OpCodes.Ldc_I4_7);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 8:
                                body.Emit(OpCodes.Ldc_I4_8);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            default:
                                body.Emit(OpCodes.Ldc_I4, _index);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                        }
                        if (_parameter.IsValueType) { body.Emit(OpCodes.Box, _parameter); }
                        else { body.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                        body.Emit(OpCodes.Stelem_Ref);
                    }
                }
                else
                {
                    body.Emit(OpCodes.Ldarg_0);
                    if (signature.Instance != Metadata<object>.Type)
                    {
                        if (signature.Instance.IsValueType) { body.Emit(OpCodes.Box, signature.Instance); }
                        else { body.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                    }
                    switch (signature.Parameters.Count)
                    {
                        case 0: body.Emit(OpCodes.Ldc_I4_0); break;
                        case 1: body.Emit(OpCodes.Ldc_I4_1); break;
                        case 2: body.Emit(OpCodes.Ldc_I4_2); break;
                        case 3: body.Emit(OpCodes.Ldc_I4_3); break;
                        case 4: body.Emit(OpCodes.Ldc_I4_4); break;
                        case 5: body.Emit(OpCodes.Ldc_I4_5); break;
                        case 6: body.Emit(OpCodes.Ldc_I4_6); break;
                        case 7: body.Emit(OpCodes.Ldc_I4_7); break;
                        case 8: body.Emit(OpCodes.Ldc_I4_8); break;
                        default: body.Emit(OpCodes.Ldc_I4, signature.Parameters.Count); break;
                    }
                    body.Emit(OpCodes.Newarr, Metadata<object>.Type);
                    for (var _index = 0; _index < signature.Parameters.Count; _index++)
                    {
                        var _parameter = signature.Parameters[_index];
                        body.Emit(OpCodes.Dup);
                        switch (_index)
                        {
                            case 0:
                                body.Emit(OpCodes.Ldc_I4_0);
                                body.Emit(OpCodes.Ldarg_1);
                                break;
                            case 1:
                                body.Emit(OpCodes.Ldc_I4_1);
                                body.Emit(OpCodes.Ldarg_2);
                                break;
                            case 2:
                                body.Emit(OpCodes.Ldc_I4_2);
                                body.Emit(OpCodes.Ldarg_3);
                                break;
                            case 3:
                                body.Emit(OpCodes.Ldc_I4_3);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 4:
                                body.Emit(OpCodes.Ldc_I4_4);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 5:
                                body.Emit(OpCodes.Ldc_I4_5);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 6:
                                body.Emit(OpCodes.Ldc_I4_6);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 7:
                                body.Emit(OpCodes.Ldc_I4_7);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            case 8:
                                body.Emit(OpCodes.Ldc_I4_8);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                            default:
                                body.Emit(OpCodes.Ldc_I4, _index);
                                body.Emit(OpCodes.Ldarg, _index);
                                break;
                        }
                        if (_parameter.IsValueType) { body.Emit(OpCodes.Box, _parameter); }
                        else { body.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                        body.Emit(OpCodes.Stelem_Ref);
                    }
                }
            }
            else
            {
                for (var _index = 0; _index < signature.Length; _index++)
                {
                    switch (_index)
                    {
                        case 0: body.Emit(OpCodes.Ldarg_0); break;
                        case 1: body.Emit(OpCodes.Ldarg_1); break;
                        case 2: body.Emit(OpCodes.Ldarg_2); break;
                        case 3: body.Emit(OpCodes.Ldarg_3); break;
                        default: body.Emit(OpCodes.Ldarg, _index); break;
                    }
                }
            }
        }
    }
}
