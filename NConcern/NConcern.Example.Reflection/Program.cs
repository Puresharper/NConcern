using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.ServiceModel;
using NConcern.Example.Logging;

namespace NConcern.Example.Reflection
{
    public class Logging : IAspect
    {
        public IEnumerable<IAdvice> Advise(MethodBase method)
        {
            var _parameters = method.GetParameters();
            var _type = method is MethodInfo ? (method as MethodInfo).ReturnType : Metadata.Void;
            if (_type == Metadata.Void)
            {
                yield return Advice.Reflection.Around((_ILGenerator, _Body) =>
                {
                    var _exception = _ILGenerator.DeclareLocal(Metadata<Exception>.Type);
                    var _trace = _ILGenerator.DeclareLocal(Metadata<Trace>.Type);
                    _ILGenerator.Emit(OpCodes.Ldtoken, method);
                    _ILGenerator.Emit(OpCodes.Ldtoken, method.ReflectedType);
                    _ILGenerator.Emit(OpCodes.Call, Metadata.Method(() => MethodBase.GetMethodFromHandle(Argument<RuntimeMethodHandle>.Value, Argument<RuntimeTypeHandle>.Value)));
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _parameters.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<string>.Type);
                    for (var _index = 0; _index < _parameters.Length; _index++)
                    {
                        var _parameter = _parameters[_index];
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _index);
                        _ILGenerator.Emit(OpCodes.Ldarg, method.IsStatic ? _index : _index + 1);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        else { _ILGenerator.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                        _ILGenerator.Emit(OpCodes.Callvirt, Metadata<object>.Method(_Object => _Object.ToString()));
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Newobj, Metadata.Constructor(() => new Trace(Argument<MethodInfo>.Value, Argument<string[]>.Value)));
                    _ILGenerator.Emit(OpCodes.Stloc, _trace);
                    _ILGenerator.BeginExceptionBlock();
                    _Body();
                    _ILGenerator.Emit(OpCodes.Ldloc, _trace);
                    _ILGenerator.Emit(OpCodes.Ldnull);
                    _ILGenerator.Emit(OpCodes.Call, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<string>.Value)));
                    _ILGenerator.BeginCatchBlock(Metadata<Exception>.Type);
                    _ILGenerator.Emit(OpCodes.Stloc, _exception);
                    _ILGenerator.Emit(OpCodes.Ldloc, _trace);
                    _ILGenerator.Emit(OpCodes.Ldloc, _exception);
                    _ILGenerator.Emit(OpCodes.Call, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<Exception>.Value)));
                    _ILGenerator.Emit(OpCodes.Rethrow);
                    _ILGenerator.EndExceptionBlock();
                });
            }
            else
            {
                yield return Advice.Reflection.Around((_ILGenerator, _Body) =>
                {
                    var _exception = _ILGenerator.DeclareLocal(Metadata<Exception>.Type);
                    var _trace = _ILGenerator.DeclareLocal(Metadata<Trace>.Type);
                    var _return = _ILGenerator.DeclareLocal(_type);
                    _ILGenerator.Emit(OpCodes.Ldtoken, method);
                    _ILGenerator.Emit(OpCodes.Ldtoken, method.ReflectedType);
                    _ILGenerator.Emit(OpCodes.Call, Metadata.Method(() => MethodBase.GetMethodFromHandle(Argument<RuntimeMethodHandle>.Value, Argument<RuntimeTypeHandle>.Value)));
                    _ILGenerator.Emit(OpCodes.Ldc_I4, _parameters.Length);
                    _ILGenerator.Emit(OpCodes.Newarr, Metadata<string>.Type);
                    for (var _index = 0; _index < _parameters.Length; _index++)
                    {
                        var _parameter = _parameters[_index];
                        _ILGenerator.Emit(OpCodes.Dup);
                        _ILGenerator.Emit(OpCodes.Ldc_I4, _index);
                        _ILGenerator.Emit(OpCodes.Ldarg, method.IsStatic ? _index : _index + 1);
                        if (_parameter.ParameterType.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _parameter.ParameterType); }
                        else { _ILGenerator.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                        _ILGenerator.Emit(OpCodes.Callvirt, Metadata<object>.Method(_Object => _Object.ToString()));
                        _ILGenerator.Emit(OpCodes.Stelem_Ref);
                    }
                    _ILGenerator.Emit(OpCodes.Newobj, Metadata.Constructor(() => new Trace(Argument<MethodInfo>.Value, Argument<string[]>.Value)));
                    _ILGenerator.Emit(OpCodes.Stloc, _trace);
                    _ILGenerator.BeginExceptionBlock();
                    _Body();
                    _ILGenerator.Emit(OpCodes.Stloc, _return);
                    _ILGenerator.Emit(OpCodes.Ldloc, _trace);
                    _ILGenerator.Emit(OpCodes.Ldloc, _return);
                    if (_type.IsValueType) { _ILGenerator.Emit(OpCodes.Box, _type); }
                    else { _ILGenerator.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                    _ILGenerator.Emit(OpCodes.Callvirt, Metadata<object>.Method(_Object => _Object.ToString()));
                    _ILGenerator.Emit(OpCodes.Call, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<string>.Value)));
                    _ILGenerator.BeginCatchBlock(Metadata<Exception>.Type);
                    _ILGenerator.Emit(OpCodes.Stloc, _exception);
                    _ILGenerator.Emit(OpCodes.Ldloc, _trace);
                    _ILGenerator.Emit(OpCodes.Ldloc, _exception);
                    _ILGenerator.Emit(OpCodes.Call, Metadata<Trace>.Method(_Trace => _Trace.Dispose(Argument<Exception>.Value)));
                    _ILGenerator.Emit(OpCodes.Rethrow);
                    _ILGenerator.EndExceptionBlock();
                    _ILGenerator.Emit(OpCodes.Ldloc, _return);
                });
            }
        }
    }

    static public class Program
    {
        static void Main(string[] args)
        {
            //define a joinpoint
            var _operationContractJoinpoint = new Func<MethodBase, bool>(_Method => _Method.IsDefined(typeof(OperationContractAttribute), true));

            //instantiate a calculator
            var _calculator = new Calculator();

            //weave logging for all operation contract
            Aspect.Weave<Logging>(_operationContractJoinpoint);

            //invoke an operation contract (logging is enabled)
            var _return = _calculator.Divide(15, 3);

            //release logging for all operation contract
            Aspect.Release<Logging>(_operationContractJoinpoint);

            //invoke an operation contract (logging is disabled)
            _return = _calculator.Divide(15, 3);

            //enable back logging aspect.
            Aspect.Weave<Logging>(_operationContractJoinpoint);

            //invoke an operation with an exception (divide by zero)
            try { _return = _calculator.Divide(15, 0); }
            catch { }
        }
    }
}
