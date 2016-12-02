using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    /// <summary>
    /// Run after method execution.
    /// </summary>
    public sealed partial class After : Advice
    {
        static internal readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        private readonly Action<ILGenerator> m_Generation;
        private readonly Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression> m_Expression;
        private readonly FieldInfo m_Delegation;
        private readonly FieldInfo m_Reflection;

        /// <summary>
        /// Emit code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to emit code to run after method execution.</param>
        public After(Action<ILGenerator> advise)
            : base(Advice.Styles.Generation)
        {
            this.m_Generation = advise;
        }

        /// <summary>
        /// Define expression representing code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to provide expression to run after method execution.</param>
        public After(Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression> advise)
            : base(Advice.Styles.Expression)
        {
            this.m_Expression = advise;
        }

        /// <summary>
        /// Define code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to run after method execution.</param>
        public After(Action<Exception> advise)
            : base(Advice.Styles.Delegation)
        {
            this.m_Delegation = After.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        /// <summary>
        /// Define code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to run after method execution.</param>
        public After(Action<object, object[], object, Exception> advise)
            : base(Advice.Styles.Reflection)
        {
            this.m_Reflection = After.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        override internal Aspect.Activity<T> Override<T>(Aspect.Activity<T> activity)
        {
            switch (this.Style)
            {
                case Advice.Styles.Generation:
                    var _type = activity.Method.ReturnType;
                    var _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.BeginFinallyBlock();
                        _body.Emit(this.m_Generation);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginFinallyBlock();
                        _body.Emit(this.m_Generation);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Expression:
                    var _signature = activity.Signature;
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    var _advice = _signature.Instance == null ? this.m_Expression(null, _parameters) : this.m_Expression(_parameters[0], _parameters.Skip(1));
                    if (_advice == null) { return activity; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    _type = activity.Method.ReturnType;
                    _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.BeginFinallyBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginFinallyBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Delegation:
                    _type = activity.Method.ReturnType;
                    _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.DeclareLocal(Metadata<Exception>.Type);
                    _body.DeclareLocal(Metadata<bool>.Type);
                    var _complete = _body.DefineLabel();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Ldc_I4_1);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Call, Metadata<Action<Exception>>.Method(_Action => _Action.Invoke(Argument<Exception>.Value)));
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Brtrue, _complete);
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Call, Metadata<Action<Exception>>.Method(_Action => _Action.Invoke(Argument<Exception>.Value)));
                        _body.MarkLabel(_complete);
                        _body.Emit(OpCodes.Rethrow);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Stloc_2);
                        _body.Emit(OpCodes.Ldc_I4_1);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Call, Metadata<Action<Exception>>.Method(_Action => _Action.Invoke(Argument<Exception>.Value)));
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Brtrue, _complete);
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Call, Metadata<Action<Exception>>.Method(_Action => _Action.Invoke(Argument<Exception>.Value)));
                        _body.MarkLabel(_complete);
                        _body.Emit(OpCodes.Rethrow);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_2);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Reflection:
                    _type = activity.Method.ReturnType;
                    _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.DeclareLocal(Metadata<Exception>.Type);
                    _body.DeclareLocal(Metadata<bool>.Type);
                    _complete = _body.DefineLabel();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Ldc_I4_1);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                        _body.Emit(activity.Signature, true);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Call, Metadata<Action<object, object[], object, Exception>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value, Argument<object>.Value, Argument<Exception>.Value)));
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Brtrue, _complete);
                        _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                        _body.Emit(activity.Signature, true);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Call, Metadata<Action<object, object[], object, Exception>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value, Argument<object>.Value, Argument<Exception>.Value)));
                        _body.MarkLabel(_complete);
                        _body.Emit(OpCodes.Rethrow);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Stloc_2);
                        _body.Emit(OpCodes.Ldc_I4_1);
                        _body.Emit(OpCodes.Stloc_1);
                        _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                        _body.Emit(activity.Signature, true);
                        _body.Emit(OpCodes.Ldloc_2);
                        if (_type.IsValueType) { _body.Emit(OpCodes.Box, _type); }
                        else { _body.Emit(OpCodes.Castclass, Metadata<object>.Type); }
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Call, Metadata<Action<object, object[], object, Exception>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value, Argument<object>.Value, Argument<Exception>.Value)));
                        _body.BeginCatchBlock(Metadata<Exception>.Type);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Ldloc_1);
                        _body.Emit(OpCodes.Brtrue, _complete);
                        _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                        _body.Emit(activity.Signature, true);
                        _body.Emit(OpCodes.Ldnull);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Call, Metadata<Action<object, object[], object, Exception>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value, Argument<object>.Value, Argument<Exception>.Value)));
                        _body.MarkLabel(_complete);
                        _body.Emit(OpCodes.Rethrow);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_2);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                default: throw new NotSupportedException();
            }
        }
    }
}
