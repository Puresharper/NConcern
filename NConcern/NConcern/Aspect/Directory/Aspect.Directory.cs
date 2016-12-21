using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace NConcern
{
    static public partial class Aspect
    {
        static private partial class Directory
        {
            static private readonly Dictionary<MethodInfo, Aspect.Directory.Entry> m_Dictionary = new Dictionary<MethodInfo, Entry>();

            static private Aspect.Directory.Entry Obtain(MethodInfo method)
            {
                var _method = method;
                if (_method.DeclaringType != _method.ReflectedType)
                {
                    _method = _method.GetBaseDefinition();
                    _method = _method.DeclaringType.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, (_Method, _Criteria) => _Method is MethodInfo && (_Method as MethodInfo).GetBaseDefinition() == _method, null).Single() as MethodInfo;
                }
                Aspect.Directory.Entry _entry;
                if (Aspect.Directory.m_Dictionary.TryGetValue(method, out _entry)) { return _entry; }
                Aspect.Directory.Populate(method.ReflectedType);
                return Aspect.Directory.m_Dictionary[method];
            }

            unsafe static private void Populate(Type type)
            {
                var _dictionary = Aspect.Directory.m_Dictionary;
                switch (IntPtr.Size)
                {
                    case 4:
                        {
                            var _address = (int*)(type.TypeHandle.Value.ToInt32() + 0x28);
                            _address = (int*)(*_address);
                            foreach (var _method in type.Methods())
                            {
                                _method.Prepare();
                                if (_method.IsVirtual)
                                {
                                    var _pointer = _method.Pointer().ToInt32();
                                    var _index = 0;
                                    while (true)
                                    {
                                        if (*(_address + _index) == _pointer)
                                        {
                                            _dictionary.Add(_method, new Entry(type, _method, new IntPtr(_address + _index), new Aspect.Activity(type, _method)));
                                            break;
                                        }
                                        _index++;
                                    }
                                }
                                else { _dictionary.Add(_method, new Entry(type, _method, new IntPtr((int*)_method.Handle().Value.ToPointer() + 2), new Aspect.Activity(type, _method))); }
                            }
                            break;
                        }
                    case 8:
                        {
                            {
                                var _address = (long*)(type.TypeHandle.Value.ToInt64() + 0x40);
                                _address = (long*)(*_address);
                                foreach (var _method in type.Methods())
                                {
                                    _method.Prepare();
                                    if (_method.IsVirtual)
                                    {
                                        var _pointer = _method.Pointer().ToInt64();
                                        var _index = 0;
                                        while (true)
                                        {
                                            if (*(_address + _index) == _pointer)
                                            {
                                                _dictionary.Add(_method, new Entry(type, _method, new IntPtr(_address + _index), new Aspect.Activity(type, _method)));
                                                break;
                                            }
                                            _index++;
                                        }
                                    }
                                    else { _dictionary.Add(_method, new Entry(type, _method, new IntPtr((long*)_method.Handle().Value.ToPointer() + 1), new Aspect.Activity(type, _method))); }
                                }
                                break;
                            }
                        }
                    default: throw new NotSupportedException();
                }
            }

            static public IEnumerable<MethodInfo> Index()
            {
                return Aspect.Directory.m_Dictionary.Values.Where(_Entry => _Entry.Count() > 0).Select(_Entry => _Entry.Method).ToArray();
            }

            static public IEnumerable<MethodInfo> Index<T>()
                where T : class, IAspect, new()
            {
                return Aspect.Directory.m_Dictionary.Values.Where(_Entry => _Entry.Contains(Singleton<T>.Value)).Select(_Entry => _Entry.Method).ToArray();
            }

            static public IEnumerable<Type> Index(MethodInfo method)
            {
                var _entry = Aspect.Directory.Obtain(method);
                return _entry.Select(_Aspect => _Aspect.GetType()).ToArray();
            }

            static public void Add<T>(MethodInfo method)
                where T : class, IAspect, new()
            {
                Aspect.Directory.Obtain(method).Add(Singleton<T>.Value);
            }

            static public void Remove(MethodInfo method)
            {
                var _entry = Aspect.Directory.Obtain(method);
                foreach (var _aspect in _entry.ToArray()) { _entry.Remove(_aspect); }
            }

            static public void Remove<T>(MethodInfo method)
                where T : class, IAspect, new()
            {
                Aspect.Directory.Obtain(method).Remove(Singleton<T>.Value);
            }
        }
    }
}