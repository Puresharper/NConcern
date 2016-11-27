using System;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;

namespace System
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __AppDomain
    {
        static public ModuleBuilder DefineDynamicModule(this AppDomain domain)
        {
            var _identity = Guid.NewGuid().ToString("N");
            return domain.DefineDynamicAssembly(new AssemblyName(string.Concat(Metadata<Assembly>.Type.Name, _identity)), AssemblyBuilderAccess.Run).DefineDynamicModule(string.Concat(Metadata<Module>.Type.Name, _identity), false);
        }
    }
}
