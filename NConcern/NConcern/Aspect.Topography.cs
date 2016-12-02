using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NConcern
{
    abstract public partial class Aspect
    {
        static private class Topography
        {
            static private readonly MethodInfo m_Dictionary = Metadata.Method(() => Aspect.Topography.Dictionary<object>()).GetGenericMethodDefinition();
            static private readonly object[] m_Arguments = new object[0];

            static private Dictionary<MethodInfo, Aspect.Map> Dictionary<T>()
                where T : class
            {
                return Aspect.Topography<T>.Dictionary;
            }

            static public Dictionary<MethodInfo, Aspect.Map> Dictionary(Type type)
            {
                return Aspect.Topography.m_Dictionary.MakeGenericMethod(type).Invoke(null, Aspect.Topography.m_Arguments) as Dictionary<MethodInfo, Aspect.Map>;
            }
        }

        static private class Topography<T>
            where T : class
        {
            static private class Initialization
            {
                unsafe static internal Dictionary<MethodInfo, Map> Dictionary()
                {
                    var _legacy = Metadata<T>.Type.Base().Reverse().ToArray();
                    var _dictionary = new Dictionary<MethodInfo, Map>();
                    var _index = 0;
                    switch (IntPtr.Size)
                    {
                        case 4:
                            {
                                var _address = (int*)(*(((int*)Metadata<T>.Type.TypeHandle.Value.ToPointer()) + 10));
                                foreach (var _method in _legacy.SelectMany(_Type => _Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(_Method => _Method.IsVirtual && _Method == _Method.GetBaseDefinition()).OrderBy(_Method => _Method.MetadataToken)))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Map(Metadata<T>.Type, _method, new IntPtr(_address + _index), new Aspect.Activity<T>(_method)));
                                    _index++;
                                }
                                foreach (var _method in Metadata<T>.Type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(_Method => !_Method.IsVirtual))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Map(Metadata<T>.Type, _method, new IntPtr((int*)_method.Handle().Value.ToPointer() + 2), new Aspect.Activity<T>(_method)));
                                }
                                break;
                            }
                        case 8:
                            {
                                var _address = (long*)(*(((long*)Metadata<T>.Type.TypeHandle.Value.ToPointer()) + 8));
                                foreach (var _method in _legacy.SelectMany(_Type => _Type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(_Method => _Method.IsVirtual && _Method == _Method.GetBaseDefinition()).OrderBy(_Method => _Method.MetadataToken)))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Map(Metadata<T>.Type, _method, new IntPtr(_address + _index), new Aspect.Activity<T>(_method)));
                                    _index++;
                                }
                                foreach (var _method in Metadata<T>.Type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(_Method => !_Method.IsVirtual))
                                {
                                    _method.Prepare();
                                    _dictionary.Add(_method, new Map(Metadata<T>.Type, _method, new IntPtr((long*)_method.Handle().Value.ToPointer() + 1), new Aspect.Activity<T>(_method)));
                                }
                                break;
                            }
                        default: throw new NotSupportedException();
                    }
                    return _dictionary;
                }
            }

            static public readonly Dictionary<MethodInfo, Map> Dictionary = Topography<T>.Initialization.Dictionary();
        }
    }
}