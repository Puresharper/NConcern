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
            public readonly Type Type;
            public readonly MethodInfo Method;
            public readonly Signature Signature;
            internal readonly IntPtr Pointer;
            
            protected Activity(Type type, MethodInfo method, Signature signature, IntPtr pointer)
            {
                this.Type = type;
                this.Method = method;
                this.Signature = signature;
                this.Pointer = pointer;
            }

            abstract public Activity Incorporate(Aspect aspect);
        }

        internal sealed class Activity<T> : Activity
            where T : class
        {
            public Activity(MethodInfo method)
                : base(Metadata<T>.Type, method, method.Signature(), method.Pointer())
            {
            }

            private Activity(Activity<T> activity, IntPtr pointer)
                : base(activity.Type, activity.Method, activity.Signature, pointer)
            {
            }

            public Aspect.Activity<T> Override(IntPtr pointer)
            {
                return new Aspect.Activity<T>(this, pointer);
            }

            override public Activity Incorporate(Aspect aspect)
            {
                var _advising = aspect.Advise<T>(this.Method);
                if (_advising == null) { return this; }
                var _activity = this;
                foreach (var _advice in _advising.Reverse())
                {
                    if (_advice == null) { continue; }
                    _activity = _advice.Override(_activity);
                }
                return _activity;
            }
        }
    }
}