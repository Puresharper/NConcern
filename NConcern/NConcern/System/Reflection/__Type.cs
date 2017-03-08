using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace System.Reflection
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    static internal class __Type
    {
        static private Resource m_Resource = new Resource();
        static private Dictionary<Type, IEnumerable<ConstructorInfo>> m_Constructors = new Dictionary<Type, IEnumerable<ConstructorInfo>>();
        static private Dictionary<Type, IEnumerable<MethodInfo>> m_Methods = new Dictionary<Type, IEnumerable<MethodInfo>>();
        static private Dictionary<Type, IEnumerable<PropertyInfo>> m_Properties = new Dictionary<Type, IEnumerable<PropertyInfo>>();

        static public IEnumerable<Type> Base(this Type type)
        {
            for (var _type = type; _type != null; _type = _type.BaseType) { yield return _type; }
        }

        static public string Declaration(this Type type)
        {
            if (type.IsGenericType)
            {
                return string.Concat(type.FullName.Remove(type.FullName.IndexOf('`')), "<", string.Join(", ", type.GetGenericArguments().Select(__Type.Declaration)), ">");
            }
            return type.FullName;
        }

        static public IEnumerable<ConstructorInfo> Constructors(this Type type)
        {
            IEnumerable<ConstructorInfo> _constructors;
            lock (__Type.m_Resource)
            {
                if (__Type.m_Constructors.TryGetValue(type, out _constructors)) { return _constructors; }
                _constructors = new Collection<ConstructorInfo>(type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly));
                __Type.m_Constructors.Add(type, _constructors);
                return _constructors;
            }
        }

        static public IEnumerable<MethodInfo> Methods(this Type type)
        {
            IEnumerable<MethodInfo> _methods;
            lock (__Type.m_Resource)
            {
                if (__Type.m_Methods.TryGetValue(type, out _methods)) { return _methods; }
                _methods = new Collection<MethodInfo>(type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly));
                __Type.m_Methods.Add(type, _methods);
                return _methods;
            }
        }

        static public IEnumerable<PropertyInfo> Properties(this Type type)
        {
            IEnumerable<PropertyInfo> _properties;
            lock (__Type.m_Resource)
            {
                if (__Type.m_Properties.TryGetValue(type, out _properties)) { return _properties; }
                _properties = new Collection<PropertyInfo>(type.GetProperties(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly));
                __Type.m_Properties.Add(type, _properties);
                return _properties;
            }
        }
    }
}
