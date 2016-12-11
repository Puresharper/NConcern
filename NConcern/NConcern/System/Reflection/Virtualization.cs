using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    static internal class Virtualization
    {
        static public IEnumerable<MethodInfo> Value(Type type)
        {
            var _legacy = type.Base().Reverse().ToArray();
            foreach (var _method in _legacy.SelectMany(_Type => _Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(_Method => _Method.IsVirtual && _Method == _Method.GetBaseDefinition()).OrderBy(_Method => _Method.MetadataToken)))
            {
                yield return type.FindMembers(MemberTypes.Method, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, (_Method, _Criteria) => _Method is MethodInfo && (_Method as MethodInfo).GetBaseDefinition() == _method.GetBaseDefinition(), null).Single() as MethodInfo;
            }
        }
    }

    static internal class Virtualization<T>
        where T : class
    {
        static public readonly IEnumerable<MethodInfo> Value = new Collection<MethodInfo>(Virtualization.Value(Metadata<T>.Type).ToArray());
    }
}
