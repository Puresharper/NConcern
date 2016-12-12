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
                Aspect.Directory.Entry _entry;
                if (Aspect.Directory.m_Dictionary.TryGetValue(method, out _entry)) { return _entry; }
                Aspect.Directory.Populate(method.ReflectedType);
                return Aspect.Directory.m_Dictionary[method];
            }

            unsafe static private void Populate(Type type)
            {
                var _dictionary = Aspect.Directory.m_Dictionary;
                var _index = 0;
                switch (IntPtr.Size)
                {
                    case 4:
                        {
                            var _address = (int*)(*(((int*)type.TypeHandle.Value.ToPointer()) + 10));
                            foreach (var _method in Virtualization.Value(type))
                            {
                                _method.Prepare();
                                _dictionary.Add(_method, new Entry(type, _method, new IntPtr(_address + _index), new Aspect.Activity(type, _method)));
                                _index++;
                            }
                            foreach (var _method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(_Method => !_Method.IsVirtual))
                            {
                                _method.Prepare();
                                _dictionary.Add(_method, new Entry(type, _method, new IntPtr((int*)_method.Handle().Value.ToPointer() + 2), new Aspect.Activity(type, _method)));
                            }
                            break;
                        }
                    case 8:
                        {
                            var _address = (long*)(*(((long*)type.TypeHandle.Value.ToPointer()) + 8));
                            foreach (var _method in Virtualization.Value(type))
                            {
                                _method.Prepare();
                                _dictionary.Add(_method, new Entry(type, _method, new IntPtr(_address + _index), new Aspect.Activity(type, _method)));
                                _index++;
                            }
                            foreach (var _method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly).Where(_Method => !_Method.IsVirtual))
                            {
                                _method.Prepare();
                                _dictionary.Add(_method, new Entry(type, _method, new IntPtr((long*)_method.Handle().Value.ToPointer() + 1), new Aspect.Activity(type, _method)));
                            }
                            break;
                        }
                    default: throw new NotSupportedException();
                }
            }

            static public IEnumerable<MethodInfo> Index()
            {
                return Aspect.Directory.m_Dictionary.Keys.ToArray();
            }

            static public IEnumerable<MethodInfo> Index<T>()
                where T : class, IAspect, new()
            {
                return Aspect.Directory.m_Dictionary.Where(_Item => _Item.Value.Contains(Singleton<T>.Value)).Select(_Item => _Item.Key).ToArray();
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

            static public void Update<T>(MethodInfo method)
                where T : class, IAspect, new()
            {
                var _entry = Aspect.Directory.Obtain(method);
                if (_entry.Contains(Singleton<T>.Value)) { _entry.Update(); }
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