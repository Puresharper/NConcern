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
    public partial class After : Advice
    {
        static internal readonly ModuleBuilder m_Module = AppDomain.CurrentDomain.DefineDynamicModule();

        private readonly Action<ILGenerator> m_Generation;
        private readonly Func<IEnumerable<ParameterExpression>, Expression> m_Expression;
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
        public After(Func<IEnumerable<ParameterExpression>, Expression> advise)
            : base(Advice.Styles.Expression)
        {
            this.m_Expression = advise;
        }

        /// <summary>
        /// Define code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to run after method execution.</param>
        public After(Action advise)
            : base(Advice.Styles.Delegation)
        {
            this.m_Delegation = After.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        /// <summary>
        /// Define code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to run after method execution.</param>
        public After(Action<object, object[]> advise)
            : base(Advice.Styles.Reflection)
        {
            this.m_Reflection = After.m_Module.DefineField(Metadata<Action>.Type.Name, advise);
        }

        override internal Junction Override(Junction junction)
        {
            switch (this.Style)
            {
                case Advice.Styles.Generation:
                    var _type = junction.Type;
                    var _method = new DynamicMethod(string.Empty, _type, junction.Signature, junction.Method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.BeginFinallyBlock();
                        _body.Emit(this.m_Generation);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginFinallyBlock();
                        _body.Emit(this.m_Generation);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return new Junction(junction, _method.Pointer());
                case Advice.Styles.Expression:
                    var _signature = junction.Signature;
                    var _parameters = new Collection<ParameterExpression>(_signature.Select(_Type => Expression.Parameter(_Type)).ToArray());
                    var _advice = this.m_Expression(_parameters);
                    if (_advice == null) { return junction; }
                    if (_advice.Type != Metadata.Void) { throw new NotSupportedException(); }
                    _type = junction.Type;
                    _method = new DynamicMethod(string.Empty, _type, _signature, junction.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.BeginFinallyBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginFinallyBlock();
                        _body.Emit(_signature, false);
                        _body.Emit(Expression.Lambda(_advice, _parameters).CompileToMethod().Pointer(), Metadata.Void, _signature);
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return new Junction(junction, _method.Pointer());
                case Advice.Styles.Delegation:
                    _type = junction.Type;
                    _method = new DynamicMethod(string.Empty, _type, junction.Signature, junction.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.BeginFinallyBlock();
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(OpCodes.Call, Metadata<Action>.Method(_Action => _Action.Invoke()));
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginFinallyBlock();
                        _body.Emit(OpCodes.Ldsfld, this.m_Delegation);
                        _body.Emit(OpCodes.Call, Metadata<Action>.Method(_Action => _Action.Invoke()));
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return new Junction(junction, _method.Pointer());
                case Advice.Styles.Reflection:
                    _type = junction.Type;
                    _method = new DynamicMethod(string.Empty, _type, junction.Signature, junction.Method.DeclaringType, true);
                    _body = _method.GetILGenerator();
                    if (_type == Metadata.Void)
                    {
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.BeginFinallyBlock();
                        _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                        _body.Emit(junction.Signature, true);
                        _body.Emit(OpCodes.Call, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value)));
                        _body.EndExceptionBlock();
                    }
                    else
                    {
                        _body.DeclareLocal(_type);
                        _body.BeginExceptionBlock();
                        _body.Emit(junction);
                        _body.Emit(OpCodes.Stloc_0);
                        _body.BeginFinallyBlock();
                        _body.Emit(OpCodes.Ldsfld, this.m_Reflection);
                        _body.Emit(junction.Signature, true);
                        _body.Emit(OpCodes.Call, Metadata<Action<object, object[]>>.Method(_Action => _Action.Invoke(Argument<object>.Value, Argument<object[]>.Value)));
                        _body.EndExceptionBlock();
                        _body.Emit(OpCodes.Ldloc_0);
                    }
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return new Junction(junction, _method.Pointer());
                default: throw new NotSupportedException();
            }
        }
    }

    public partial class After<T> : After
        where T : Attribute
    {
        /// <summary>
        /// Emit code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to emit code to run after method execution.</param>
        public After(Action<ILGenerator> advise)
            : base(advise)
        {
        }

        /// <summary>
        /// Define expression representing code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to provide expression to run after method execution.</param>
        public After(Func<IEnumerable<ParameterExpression>, Expression> advise)
            : base(advise)
        {
        }

        /// <summary>
        /// Define code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to run after method execution.</param>
        public After(Action advise)
            : base(advise)
        {
        }

        /// <summary>
        /// Define code to run after method execution.
        /// </summary>
        /// <param name="advise">Delegate to run after method execution.</param>
        public After(Action<object, object[]> advise)
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
