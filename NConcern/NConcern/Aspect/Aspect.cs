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
        /// Get all aspects woven in a method.
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
        /// Weave an aspect to a specific method.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="method">Method</param>
        static public void Weave<T>(MethodInfo method)
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                if (Aspect.Directory.Index(method).Contains(Metadata<T>.Type)) { Aspect.Directory.Update<T>(method); }
                else { Aspect.Directory.Add<T>(method); }
            }
        }

        /// <summary>
        /// Weave an aspect to methods matching with a specific pattern
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
                    foreach (var _method in Virtualization.Value(_type).Concat(_type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly)))
                    {
                        if (pattern(_method))
                        {
                            if (Aspect.Directory.Index(_method).Contains(Metadata<T>.Type)) { Aspect.Directory.Update<T>(_method); }
                            else { Aspect.Directory.Add<T>(_method); }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Release all aspects for a specific method.
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
        /// Release all aspects for methods matching with a specific pettern.
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
        /// Release all aspects for methods attributed with a specific custom attribute.
        /// </summary>
        /// <param name="type">Custom attribute type</param>
        static public void Release(Type type)
        {
            if (!Metadata<Attribute>.Type.IsAssignableFrom(type)) { throw new InvalidOperationException(string.Format("[{0}] is not an custome attribute type.", type.Declaration())); }
            lock (Aspect.m_Resource)
            {
                foreach (var _method in Aspect.Directory.Index().Where(_Method => _Method.IsDefined(type, true))) { Aspect.Directory.Remove(_method); }
            }
        }

        /// <summary>
        /// Release an aspect for a specific method.
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
        /// Release an aspect for a specific method.
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
        /// Release an aspect for methods matching with a specific pattern.
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
        /// Release an aspect for methods attributed with a specific custom attribute.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <param name="type">Custom attribute type/param>
        static public void Release<T>(Type type)
            where T : class, IAspect, new()
        {
            if (!Metadata<Attribute>.Type.IsAssignableFrom(type)) { throw new InvalidOperationException(string.Format("[{0}] is not an custome attribute type.", type.Declaration())); }
            lock (Aspect.m_Resource)
            {
                foreach (var _method in Aspect.Directory.Index<T>().Where(_Method => _Method.IsDefined(type, true))) { Aspect.Directory.Remove(_method); }
            }
        }
    }
}