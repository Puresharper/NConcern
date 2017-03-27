using System;
using System.Linq;
using System.Reflection;

namespace NConcern
{
    static public partial class Aspect
    {
        internal class Activity
        {
            internal readonly Activity Authority;
            public readonly Type Type;
            public readonly MethodBase Method;
            public readonly Signature Signature;
            internal readonly Func<MethodInfo, MethodInfo> Update;

            public Activity(Type type, MethodBase method)
                : this(null, type, method, method.Signature(), _Method => _Method)
            {
            }

            public Activity(Activity authority, Type type, MethodBase method, Signature signature, Func<MethodInfo, MethodInfo> update)
            {
                this.Authority = authority;
                this.Type = type;
                this.Method = method;
                this.Signature = signature;
                this.Update = update;
            }
        }
    }
}