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
            public readonly MethodInfo Method;
            public readonly Signature Signature;
            internal readonly MethodInfo Implementation;
            internal readonly IntPtr Pointer;

            public Activity(Type type, MethodInfo method)
                : this(null, type, method, method.Signature(), method)
            {
            }

            public Activity(Activity authority, Type type, MethodInfo method, Signature signature, MethodInfo implementation)
            {
                this.Authority = authority;
                this.Type = type;
                this.Method = method;
                this.Signature = signature;
                this.Implementation = implementation;
                this.Pointer = implementation.Pointer();
            }

            private Activity(Activity authority, MethodInfo method)
                : this(authority, authority.Type, authority.Method, authority.Signature, method)
            {
            }

            public Activity Incorporate(IAspect aspect)
            {
                var _advising = aspect.Advise(this.Method);
                if (_advising == null) { return this; }
                var _activity = this;
                foreach (var _advice in _advising.Reverse())
                {
                    if (_advice == null) { continue; }
                    _activity = new Activity(_activity, _advice.Decorate(_activity.Method, _activity.Pointer));
                }
                return _activity;
            }
        }
    }
}