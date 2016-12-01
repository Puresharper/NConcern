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
    /// Run before method execution.
    /// </summary>
    public sealed class Before : Advice
    {
        static private readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        private readonly Action<ILGenerator> m_Generation;
        private readonly Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression> m_Expression;
        private readonly FieldInfo m_Delegation;
        private readonly FieldInfo m_Reflection;

        /// <summary>
        /// Emit code to run before method execution.
        /// </summary>
        /// <param name="advise">Delegate to emit code to run after method execution.</param>
        public Before(Action<ILGenerator> advise)
            : base(Advice.Styles.Generation)
        {
            this.m_Generation = advise;
        }

        /// <summary>
        /// Define expression representing code to run before method execution.
        /// </summary>
        /// <param name="advise">Delegate to provide expression to run before method execution.</param>
        public Before(Func<ParameterExpression, IEnumerable<ParameterExpression>, Expression> advise)
            : base(Advice.Styles.Expression)
        {
            this.m_Expression = advise;
        }

        /// <summary>
        /// Define code to run before method execution.
        /// </summary>
        /// <param name="advise">Delegate to run before method execution.</param>
        public Before(Action advise)
            : base(Advice.Styles.Delegation)
        {
            this.m_Delegation = Before.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        /// <summary>
        /// Define code to run before method execution.
        /// </summary>
        /// <param name="advise">Delegate to run before method execution.</param>
        public Before(Action<object, object[]> advise)
            : base(Advice.Styles.Reflection)
        {
            this.m_Reflection = Before.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        override internal Aspect.Activity<T> Override<T>(Aspect.Activity<T> activity)
        {
            switch (this.Style)
            {
                case Advice.Styles.Generation:
                        var _method = new DynamicMethod(string.Empty, activity.Method.ReturnType, activity.Signature, activity.Method.DeclaringType, true);
                        var _body = _method.GetILGenerator();
                        _body.Emit(this.m_Generation);
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Ret);
                        _method.Prepare();
                        return activity.Override(_method.Pointer());
                case Advice.Styles.Expression:
                        var _signature = activity.Signature;
                        var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                        var _advice = _signature.Instance == null ? this.m_Expression(null, _parameters) : this.m_Expression(_parameters[0], _parameters.Skip(1));
                        if (_advice == null) { return activity; }
                        var _type = this.Style;
                        _method = new DynamicMethod(string.Empty, activity.Method.ReturnType, _signature, activity.Method.DeclaringType, true);
                        _body = _method.GetILGenerator();
                        _body.Emit(_signature, false);
                        _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                        _body.Emit(activity);
                        _body.Emit(OpCodes.Ret);
                        _method.Prepare();
                        return activity.Override(_method.Pointer());
                case Advice.Styles.Delegation:
                    _method = new DynamicMethod(string.Empty, activity.Method.ReturnType, activity.Signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                    _body.Emit(OpCodes.Call, Metadata<Action>.Method(_Action => _Action.Invoke()));
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                case Advice.Styles.Reflection:
                    _method = new DynamicMethod(string.Empty, activity.Method.ReturnType, activity.Signature, activity.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                    _body.Emit(activity.Signature, true);
                    _body.Emit(OpCodes.Call, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value)));
                    _body.Emit(activity);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return activity.Override(_method.Pointer());
                default: throw new NotSupportedException();
            }
        }
    }
}
