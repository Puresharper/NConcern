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
    public partial class Arround : Advice
    {
        static internal readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        private readonly Action<ILGenerator, Action<ILGenerator>> m_Generation;
        private readonly Func<IEnumerable<ParameterExpression>, Expression, Expression> m_Expression;
        private readonly FieldInfo m_Delegation;
        private readonly FieldInfo m_Reflection;

        public Arround(Action<ILGenerator, Action<ILGenerator>> advise)
            : base(Advice.Styles.Generation)
        {
            this.m_Generation = advise;
        }

        public Arround(Func<IEnumerable<ParameterExpression>, Expression, Expression> advise)
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

        override internal Junction Override(Junction junction)
        {
            switch (this.Style)
            {
                case Advice.Styles.Generation:
                    var _type = junction.Type;
                    var _method = new DynamicMethod(string.Empty, _type, junction.Signature, junction.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    this.m_Generation(_body, _Body => _Body.Emit(junction));    
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return new Junction(junction, _method.Pointer());
                case Advice.Styles.Expression:
                    var _signature = junction.Signature;
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    _type = junction.Type;
                    _method = new DynamicMethod(string.Empty, _type, _signature, junction.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(junction);
                    _body.Emit(OpCodes.Ret);
                    var _advice = this.m_Expression(_parameters, Expression.Call(_method, _parameters));
                    if (_advice == null) { return junction; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    _method = new DynamicMethod(string.Empty, _type, _signature, junction.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(_signature, false);
                    _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return new Junction(junction, _method.Pointer());
                case Advice.Styles.Delegation:
                    _type = junction.Type;
                    _signature = junction.Signature;
                    var _operation = new Closure.Operation(junction.Pointer, _signature, junction.Method.ReturnType);
                    _method = new DynamicMethod(string.Empty, _type, _signature, junction.Method.DeclaringType, true);
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
                    return new Junction(junction, _method.Pointer());
                case Advice.Styles.Reflection:
                    throw new NotSupportedException();
                default: throw new NotSupportedException();
            }
        }
    }

    public class Arround<T> : Arround
        where T : Attribute
    {
        public Arround(Action<ILGenerator, Action<ILGenerator>> advise)
            : base(advise)
        {
        }

        public Arround(Func<IEnumerable<ParameterExpression>, Expression, Expression> advise)
            : base(advise)
        {
        }

        public Arround(Action<Action> advise)
            : base(advise)
        {
        }

        public Arround(Func<object, object[], Func<object, object[], object>, object> advise)
            : base(advise)
        {
        }

        internal override Junction Override(Junction junction)
        {
            if (junction.Method.Attributed<T>()) { return junction; }
            return base.Override(junction);
        }
    }
}
