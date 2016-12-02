using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime;

namespace NConcern
{
    /// <summary>
    /// Run arround method execution.
    /// </summary>
    public sealed partial class Around : Advice
    {
        static internal readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        private readonly Action<ILGenerator, Action<ILGenerator>> m_Generation;
        private readonly Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression, Expression> m_Expression;
        private readonly FieldInfo m_Delegation;
        private readonly FieldInfo m_Reflection;

        public Around(Action<ILGenerator, Action<ILGenerator>> advise)
            : base(Advice.Styles.Generation)
        {
            this.m_Generation = advise;
        }

        public Around(Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression, Expression> advise)
            : base(Advice.Styles.Expression)
        {
            this.m_Expression = advise;
        }

        public Around(Action<Action> advise)
            : base(Advice.Styles.Delegation)
        {
            this.m_Delegation = Around.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        public Around(Func<object, object[], Func<object, object[], object>, object> advise)
            : base(Advice.Styles.Reflection)
        {
            this.m_Reflection = Around.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        override internal Aspect.Activity<T> Override<T>(Aspect.Activity<T> activity)
        {
            switch (this.Style)
            {
                case Advice.Styles.Generation:
                    var _type = activity.Method.ReturnType;
                    var _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    this.m_Generation(_body, _Body => _Body.Emit(activity));    
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Expression:
                    var _signature = activity.Signature;
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    _type = activity.Method.ReturnType;
                    _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Ret);
                    var _advice = _signature.Instance == null ? this.m_Expression(null, _parameters, Expression.Call(_method, _parameters)) : this.m_Expression(_parameters[0], _parameters.Skip(1), Expression.Call(_method, _parameters));
                    if (_advice == null) { return activity; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(_signature, false);
                    _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Delegation:
                    _type = activity.Method.ReturnType;
                    _signature = activity.Signature;
                    var _routine = new Closure.Routine(activity.Pointer, _signature, activity.Method.ReturnType);
                    _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Newobj, _routine.Constructor);
                        _body.Emit(OpCodes.Ldftn, _routine.Method);
                        _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                        _body.Emit(OpCodes.Call, Metadata<Action<Action>>.Method(_Action => _Action.Invoke(Argument<Action>.Value)));
                    }
                    else
                    {
                        _body.DeclareLocal(_routine.Type);
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Newobj, _routine.Constructor);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldftn, _routine.Method);
                        _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                        _body.Emit(OpCodes.Call, Metadata<Action<Action>>.Method(_Action => _Action.Invoke(Argument<Action>.Value)));
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldfld, _routine.Value);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Reflection:
                    _type = activity.Method.ReturnType;
                    _signature = activity.Signature;
                    var _function = new Closure.Function(activity.Pointer, _signature, activity.Method.ReturnType);
                    _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                    _body.Emit(_signature, true);
                    _body.Emit(OpCodes.Newobj, _function.Constructor);
                    _body.Emit(OpCodes.Ldftn, _function.Method);
                    _body.Emit(OpCodes.Newobj, Metadata<Func<object, object[], object>>.Type.GetConstructors().Single());
                    _body.Emit(OpCodes.Call, Metadata<Func<object, object[], Func<object, object[], object>, object>>.Method(_Function => _Function.Invoke(Argument<object>.Value, Argument<object[]>.Value, Argument<Func<object, object[], object>>.Value)));
                    if (_type == Metadata.Void) { _body.Emit(OpCodes.Pop); }
                    else
                    {
                        if (_type.IsValueType) { _body.Emit(OpCodes.Unbox_Any, _type); }
                        else { _body.Emit(OpCodes.Castclass, _type); }
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                default: throw new NotSupportedException();
            }
        }
    }
}
