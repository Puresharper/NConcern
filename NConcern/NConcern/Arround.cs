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
    public sealed partial class Arround : Advice
    {
        static internal readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        private readonly Action<ILGenerator, Action<ILGenerator>> m_Generation;
        private readonly Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression, Expression> m_Expression;
        private readonly FieldInfo m_Delegation;
        private readonly FieldInfo m_Reflection;

        public Arround(Action<ILGenerator, Action<ILGenerator>> advise)
            : base(Advice.Styles.Generation)
        {
            this.m_Generation = advise;
        }

        public Arround(Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression, Expression> advise)
            : base(Advice.Styles.Expression)
        {
            this.m_Expression = advise;
        }

        public Arround(Action<Action> advise)
            : base(Advice.Styles.Delegation)
        {
            this.m_Delegation = Arround.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        public Arround(Func<object, object[], Func<object, object[], object>, object> advise)
            : base(Advice.Styles.Reflection)
        {
            this.m_Reflection = Arround.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        override internal Aspect.Activity<T> Override<T>(Aspect.Activity<T> activity)
        {
            switch (this.Style)
            {
                case Advice.Styles.Generation:
                    var _type = activity.Type;
                    var _method = new DynamicMethod(string.Empty, _type, activity.Signature, activity.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    this.m_Generation(_body, _Body => _Body.Emit(activity));    
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Expression:
                    var _signature = activity.Signature;
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    _type = activity.Type;
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
                    _type = activity.Type;
                    _signature = activity.Signature;
                    var _operation = new Closure.Operation(activity.Pointer, _signature, activity.Method.ReturnType);
                    _method = new DynamicMethod(string.Empty, _type, _signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Newobj, _operation.Constructor);
                        _body.Emit(OpCodes.Ldftn, _operation.Method);
                        _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                        _body.Emit(OpCodes.Call, Metadata<Action<Action>>.Method(_Action => _Action.Invoke(Argument<Action>.Value)));
                    }
                    else
                    {
                        _body.DeclareLocal(_operation.Type);
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(_signature, false);
                        _body.Emit(OpCodes.Newobj, _operation.Constructor);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldftn, _operation.Method);
                        _body.Emit(OpCodes.Newobj, Metadata<Action>.Type.GetConstructors().Single());
                        _body.Emit(OpCodes.Call, Metadata<Action<Action>>.Method(_Action => _Action.Invoke(Argument<Action>.Value)));
                        _body.Emit(OpCodes.Ldloc_0);
                        _body.Emit(OpCodes.Ldfld, _operation.Value);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Reflection:
                    throw new NotSupportedException();
                default: throw new NotSupportedException();
            }
        }
    }
}
