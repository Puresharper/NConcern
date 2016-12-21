using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace NConcern
{
    /// <summary>
    /// Manage weaving process.
    /// </summary>
    static public partial class Aspect
    {
        static private readonly Resource m_Resource = new Resource();

        static private IEnumerable<Type> Explore(Assembly assembly)
        {
            return assembly.GetTypes().SelectMany(Aspect.Explore);
        }

        static private IEnumerable<Type> Explore(Type type)
        {
            foreach (var _type in type.GetNestedTypes(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly).SelectMany(Aspect.Explore)) { yield return _type; }
            yield return type;
        }

        static private IEnumerable<Type> Explore()
        {
            var _domain = AppDomain.CurrentDomain.GetAssemblies();
            return _domain.Where(Aspect.Compatible).SelectMany(Aspect.Explore);
        }

        static private bool Compatible(Assembly assembly)
        {
            var _debuggable = assembly.GetCustomAttributes(Metadata<DebuggableAttribute>.Type, true).SingleOrDefault() as DebuggableAttribute;
            if (_debuggable == null) { return false; }
            return _debuggable.IsJITOptimizerDisabled && _debuggable.IsJITTrackingEnabled;
        }

        static private bool Compatible(Type type)
        {
            return Aspect.Compatible(type.Assembly);
        }

        static private bool Compatible(MethodInfo method)
        {
            return Aspect.Compatible(method.DeclaringType);
        }

        static public IEnumerable<MethodInfo> Lookup()
        {
            lock (Aspect.m_Resource)
            {
                return Aspect.Directory.Index();
            }
        }

        /// <summary>
        /// Get all methods managed by an aspect.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <returns>Enumerable of methods managed by the aspect</returns>
        static public IEnumerable<MethodInfo> Lookup<T>()
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                return Aspect.Directory.Index<T>();
            }
        }

        /// <summary>
        /// Get all aspects woven on a method.
        /// </summary>
        /// <param name="method">Method</param>
        /// <returns>Enumerable of aspects woven in the method</returns>
        static public IEnumerable<Type> Enumerate(MethodInfo method)
        {
            lock (Aspect.m_Resource)
            {
                return Aspect.Directory.Index(method);
            }
        }

        /// <summary>
        /// Weave an aspect on a specific method.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="method">Method</param>
        static public void Weave<T>(MethodInfo method)
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                Aspect.Directory.Add<T>(method);
            }
        }

        /// <summary>
        /// Weave an aspect on methods matching with a specific pattern
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="pattern">Pattern</param>
        static public void Weave<T>(Func<MethodInfo, bool> pattern)
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                foreach (var _type in Aspect.Explore().SelectMany(Aspect.Explore))
                {
                    if (_type.ContainsGenericParameters) { continue; }
                    foreach (var _method in _type.Methods())
                    {
                        if (pattern(_method))
                        {
                            Aspect.Directory.Add<T>(_method);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Weave an aspect on methods defined as [type] or defined in [type].
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="type">Type</param>
        static public void Weave<T>(Type type)
            where T : class, IAspect, new()
        {
            if (Metadata<Attribute>.Type.IsAssignableFrom(type))
            {
                lock (Aspect.m_Resource)
                {
                    foreach (var _method in Aspect.Directory.Index<T>())
                    {
                        if (_method.IsDefined(type, true) || _method.DeclaringType.IsDefined(type, true))
                        {
                            Aspect.Directory.Add<T>(_method);
                            continue;
                        }
                        var _property = _method.Property();
                        if (_property == null) { continue; }
                        if (_property.IsDefined(type, true)) { Aspect.Directory.Add<T>(_method); }
                    }
                }
            }
            else
            {
                lock (Aspect.m_Resource)
                {
                    foreach (var _method in Aspect.Directory.Index<T>().Where(_Method => _Method.IsPublic && type.IsAssignableFrom(_Method.DeclaringType))) { Aspect.Directory.Add<T>(_method); }
                }
            }
        }

        /// <summary>
        /// Release all aspects from a specific method.
        /// </summary>
        /// <param name="method">Method</param>
        static public void Release(MethodInfo method)
        {
            lock (Aspect.m_Resource)
            {
                Aspect.Directory.Remove(method);
            }
        }

        /// <summary>
        /// Release all aspects from methods matching with a specific pettern.
        /// </summary>
        /// <param name="pattern">Pattern</param>
        static public void Release(Func<MethodInfo, bool> pattern)
        {
            lock (Aspect.m_Resource)
            {
                foreach (var _method in Aspect.Directory.Index().Where(pattern)) { Aspect.Directory.Remove(_method); }
            }
        }

        /// <summary>
        /// Release all aspects from methods defined as [type] or defined in [type].
        /// </summary>
        /// <param name="type">Custom attribute type</param>
        static public void Release(Type type)
        {
            if (Metadata<Attribute>.Type.IsAssignableFrom(type))
            {
                lock (Aspect.m_Resource)
                {
                    foreach (var _method in Aspect.Directory.Index().Where(_Method => _Method.IsDefined(type, true) || _Method.DeclaringType.IsDefined(type, true) || _Method.ReflectedType.IsDefined(type, true))) { Aspect.Directory.Remove(_method); }
                }
            }
            else
            {
                lock (Aspect.m_Resource)
                {
                    foreach (var _method in Aspect.Directory.Index().Where(_Method => type.IsAssignableFrom(_Method.ReflectedType))) { Aspect.Directory.Remove(_method); }
                }
            }
        }

        /// <summary>
        /// Release an aspect from a specific method.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        static public void Release<T>()
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                foreach (var _method in Aspect.Directory.Index<T>()) { Aspect.Directory.Remove(_method); }
            }
        }

        /// <summary>
        /// Release an aspect from a specific method.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="method">Method</param>
        static public void Release<T>(MethodInfo method)
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                Aspect.Directory.Remove<T>(method);
            }
        }

        /// <summary>
        /// Release an aspect from methods matching with a specific pattern.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="pattern">Pattern</param>
        static public void Release<T>(Func<MethodInfo, bool> pattern)
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                foreach (var _method in Aspect.Directory.Index<T>().Where(pattern)) { Aspect.Directory.Remove(_method); }
            }
        }

        /// <summary>
        /// Release an aspect from methods defined as [type] or defined in [type].
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="type">Custom attribute type/param>
        static public void Release<T>(Type type)
            where T : class, IAspect, new()
        {
            if (Metadata<Attribute>.Type.IsAssignableFrom(type))
            {
                lock (Aspect.m_Resource)
                {
                    foreach (var _method in Aspect.Directory.Index<T>().Where(_Method => _Method.IsDefined(type, true) || _Method.DeclaringType.IsDefined(type, true) || _Method.ReflectedType.IsDefined(type, true))) { Aspect.Directory.Remove<T>(_method); }
                }
            }
            else
            {
                lock (Aspect.m_Resource)
                {
                    foreach (var _method in Aspect.Directory.Index<T>().Where(_Method => type.IsAssignableFrom(_Method.ReflectedType))) { Aspect.Directory.Remove<T>(_method); }
                }
            }
        }
    }
}