using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;

namespace NConcern
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static public partial class Advisor
    {
        static internal readonly ModuleBuilder Module = AppDomain.CurrentDomain.DefineDynamicModule();

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }
    }
}