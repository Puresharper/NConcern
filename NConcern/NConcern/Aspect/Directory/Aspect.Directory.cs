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
            static private readonly Dictionary<MethodBase, Aspect.Directory.Entry> m_Dictionary = new Dictionary<MethodBase, Entry>();

            static private Aspect.Directory.Entry Obtain(MethodBase method)
            {
                var _method = method;
                if (_method.DeclaringType != _method.ReflectedType)
                {
                    if (_method is MethodInfo) { _method = (_method as MethodInfo).GetBaseDefinition(); }
                    _method = _method.DeclaringType.FindMembers(MemberTypes.Method, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly, (_Method, _Criteria) => _Method is ConstructorInfo || _Method is MethodInfo && (_Method as MethodInfo).GetBaseDefinition() == _method, null).Single() as MethodBase;
                }
                Aspect.Directory.Entry _entry;
                if (Aspect.Directory.m_Dictionary.TryGetValue(method, out _entry)) { return _entry; }
                _entry = new Aspect.Directory.Entry(_method.DeclaringType, _method, new Aspect.Activity(_method.DeclaringType, _method));
                Aspect.Directory.m_Dictionary.Add(_method, _entry);
                return _entry;
            }

            static public IEnumerable<MethodBase> Index()
            {
                return Aspect.Directory.m_Dictionary.Values.Where(_Entry => _Entry.Count() > 0).Select(_Entry => _Entry.Method).ToArray();
            }

            static public IEnumerable<MethodBase> Index<T>()
                where T : class, IAspect, new()
            {
                return Aspect.Directory.m_Dictionary.Values.Where(_Entry => _Entry.Contains(Singleton<T>.Value)).Select(_Entry => _Entry.Method).ToArray();
            }

            static public IEnumerable<Type> Index(MethodBase method)
            {
                var _entry = Aspect.Directory.Obtain(method);
                return _entry.Select(_Aspect => _Aspect.GetType()).ToArray();
            }

            static public void Add<T>(MethodBase method)
                where T : class, IAspect, new()
            {
                Aspect.Directory.Obtain(method).Add(Singleton<T>.Value);
            }

            static public void Remove(MethodBase method)
            {
                var _entry = Aspect.Directory.Obtain(method);
                foreach (var _aspect in _entry.ToArray()) { _entry.Remove(_aspect); }
            }

            static public void Remove<T>(MethodBase method)
                where T : class, IAspect, new()
            {
                Aspect.Directory.Obtain(method).Remove(Singleton<T>.Value);
            }
        }
    }
}