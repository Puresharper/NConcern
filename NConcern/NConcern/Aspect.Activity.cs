using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    abstract public partial class Aspect
    {
        abstract internal class Activity
        {
            internal readonly Activity Authority;
            public readonly Type Type;
            public readonly MethodInfo Method;
            public readonly Signature Signature;
            internal readonly MethodInfo Implementation;
            internal readonly IntPtr Pointer;
            
            protected Activity(Activity authority, Type type, MethodInfo method, Signature signature, MethodInfo implementation)
            {
                this.Authority = authority;
                this.Type = type;
                this.Method = method;
                this.Signature = signature;
                this.Implementation = implementation;
                this.Pointer = implementation.Pointer();
            }

            abstract public Activity Incorporate(Aspect aspect);
        }

        internal sealed class Activity<T> : Activity
            where T : class
        {
            public Activity(MethodInfo method)
                : base(null, Metadata<T>.Type, method, method.Signature(), method)
            {
            }

            private Activity(Activity<T> authority, MethodInfo method)
                : base(authority, authority.Type, authority.Method, authority.Signature, method)
            {
            }

            public Aspect.Activity<T> Override(MethodInfo method)
            {
                return new Aspect.Activity<T>(this, method);
            }

            override public Activity Incorporate(Aspect aspect)
            {
                var _advising = aspect.Advise<T>(this.Method);
                if (_advising == null) { return this; }
                var _activity = this;
                foreach (var _advice in _advising.Reverse())
                {
                    if (_advice == null) { continue; }
                    _activity = _advice.Advise(_activity);
                }
                return _activity;
            }
        }
    }
}