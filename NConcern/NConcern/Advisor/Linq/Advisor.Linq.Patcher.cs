using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    static public partial class Advisor
    {
        public partial class Linq
        {
            internal sealed class Patcher : ExpressionVisitor
            {
                static private MethodInfo Method(MethodInfo method, IntPtr pointer)
                {
                    var _type = method.ReturnType;
                    var _signature = method.Signature();
                    var _method = new DynamicMethod(string.Empty, _type, _signature, method.DeclaringType, true);
                    var _body = _method.GetILGenerator();
                    _body.Emit(_signature, false);
                    _body.Emit(pointer, method.ReturnType, _signature);
                    _body.Emit(OpCodes.Ret);
                    _method.Prepare();
                    return _method;
                }

                static public Expression Patch(Expression expression, MethodInfo method, IntPtr pointer)
                {
                    return new Patcher(method, pointer).Visit(expression);
                }

                private readonly MethodInfo m_Method;
                private readonly IntPtr m_Pointer;

                private Patcher(MethodInfo method, IntPtr pointer)
                {
                    this.m_Method = method;
                    this.m_Pointer = pointer;
                }

                override protected Expression VisitMethodCall(MethodCallExpression node)
                {
                    if (node.Method == this.m_Method)
                    {
                        if (this.m_Method.IsStatic) { return Expression.Call(Advisor.Linq.Patcher.Method(this.m_Method, this.m_Pointer), node.Arguments); }
                        return Expression.Call(Advisor.Linq.Patcher.Method(this.m_Method, this.m_Pointer), new Expression[] { node.Object }.Concat(node.Arguments));
                    }
                    return base.VisitMethodCall(node);
                }
            }
        }
    }
}