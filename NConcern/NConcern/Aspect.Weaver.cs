using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace NConcern
{
    abstract public partial class Aspect
    {
        public class Weaver : IDisposable
        {
            static private IEnumerable<Type> Resolve(Assembly assembly)
            {
                return assembly.GetTypes();
            }

            static private IEnumerable<Type> Resolve(Type type)
            {
                yield return type;
                foreach (var _type in type.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).SelectMany(Weaver.Resolve)) { yield return _type; }
            }

            static public IDisposable Weave(IEnumerable<Assembly> domain)
            {
                return new Weaver(domain);
            }

            private readonly Aspect[] m_Aspectization;

            private Weaver(IEnumerable<Assembly> domain)
            {
                var _domain = domain.SelectMany(Weaver.Resolve).ToArray();
                var _arguments = new object[0];
                var _aspectization = this.m_Aspectization = _domain.Where(_Type => Metadata<Aspect>.Type.IsAssignableFrom(_Type) && !_Type.IsAbstract && _Type.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null) != null).Select(_Type => Activator.CreateInstance(_Type, true) as Aspect).ToArray();
                foreach (var _type in _domain.Except(_domain.Where(_Type => Metadata<Aspect>.Type.IsAssignableFrom(_Type))))
                {
                    if (_type.IsClass && _type.Assembly.GetCustomAttributes(Metadata<DebuggableAttribute>.Type, true).Cast<DebuggableAttribute>().Any(_Debuggable => _Debuggable.IsJITOptimizerDisabled && _Debuggable.IsJITTrackingEnabled))
                    {
                        foreach (var _aspect in _aspectization)
                        {
                            _aspect.Manage(_type);
                        }
                    }
                }
            }

            public void Dispose()
            {
                foreach (var _aspect in this.m_Aspectization.Reverse()) { _aspect.Dispose(); }
            }
        }
    }
}
