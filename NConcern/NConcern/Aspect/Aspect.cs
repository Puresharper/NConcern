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
            return _domain.SelectMany(Aspect.Explore);
        }

        /// <summary>
        /// Get all methods managed by at least one aspect.
        /// </summary>
        /// <returns>Enumerable of methods managed by at least one aspect</returns>
        static public IEnumerable<MethodBase> Lookup()
        {
            lock (Aspect.m_Resource)
            {
                return Aspect.Directory.Index();
            }
        }

        /// <summary>
        /// Get all methods managed by the aspect.
        /// </summary>
        /// <typeparam name="T">Aspect</typeparam>
        /// <returns>Enumerable of methods managed by the aspect</returns>
        static public IEnumerable<MethodBase> Lookup<T>()
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
        static public IEnumerable<Type> Enumerate(MethodBase method)
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
        static public void Weave<T>(MethodBase method)
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
        static public void Weave<T>(Func<MethodBase, bool> pattern)
            where T : class, IAspect, new()
        {
            lock (Aspect.m_Resource)
            {
                foreach (var _type in Aspect.Explore().SelectMany(Aspect.Explore))
                {
                    if (_type.ContainsGenericParameters) { continue; }
                    foreach (var _constructor in _type.GetConstructors())
                    {
                        if (_constructor.IsStatic) { continue; }
                        if (_constructor.IsAbstract) { continue; }
                        if (pattern(_constructor)) { Aspect.Directory.Add<T>(_constructor); }
                    }
                    foreach (var _method in _type.Methods())
                    {
                        if (_method.IsAbstract) { continue; }
                        if (pattern(_method)) { Aspect.Directory.Add<T>(_method); }
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
                Aspect.Weave<T>(_Method =>
                {
                    if (_Method.IsAbstract) { return false; }
                    if (_Method.IsDefined(type, true) || _Method.DeclaringType.IsDefined(type, true)) { return true; }
                    var _property = _Method.Property();
                    if (_property != null && _property.IsDefined(type, true)) { return true; }
                    var _method = _Method.GetBaseDefinition();
                    if (_method.IsDefined(type, true) || _method.DeclaringType.IsDefined(type, true)) { return true; }
                    _property = _method.Property();
                    if (_property != null && _property.IsDefined(type, true)) { return true; }
                    var _entry = _Method.DeclaringType.GetInterfaces().Select(_Interface => _Method.DeclaringType.GetInterfaceMap(_Interface)).SelectMany(_Map => _Map.TargetMethods.Zip(_Map.InterfaceMethods, (_Interface, _Implementation) => new { Interface = _Interface, Implementation = _Implementation })).FirstOrDefault(_Entry => _Entry.Implementation == _Method);
                    if (_entry != null)
                    {
                        if (_entry.Interface.IsDefined(type, true) || _entry.Interface.DeclaringType.IsDefined(type, true)) { return true; }
                        _property = _entry.Interface.Property();
                        if (_property != null && _property.IsDefined(type, true)) { return true; }
                    }
                    return false;
                });
            }
            else if (type.IsInterface)
            {
                Aspect.Weave<T>(_Method =>
                {
                    if (type.IsAssignableFrom(_Method.DeclaringType)) { return _Method.DeclaringType.GetInterfaceMap(type).TargetMethods.Contains(_Method); }
                    return false;
                });
            }
            else if (type.IsClass)
            {
                Aspect.Weave<T>(_Method =>
                {
                    if (_Method.IsPublic && type.IsAssignableFrom(_Method.DeclaringType)) { return true; }
                    return false;
                });
            }
            else { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Release all aspects from a specific method.
        /// </summary>
        /// <param name="method">Method</param>
        static public void Release(MethodBase method)
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
        static public void Release(Func<MethodBase, bool> pattern)
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
                Aspect.Release(_Method =>
                {
                    if (_Method.IsAbstract) { return false; }
                    if (_Method.IsDefined(type, true) || _Method.DeclaringType.IsDefined(type, true)) { return true; }
                    var _property = _Method.Property();
                    if (_property != null && _property.IsDefined(type, true)) { return true; }
                    var _method = _Method.GetBaseDefinition();
                    if (_method.IsDefined(type, true) || _method.DeclaringType.IsDefined(type, true)) { return true; }
                    _property = _method.Property();
                    if (_property != null && _property.IsDefined(type, true)) { return true; }
                    var _entry = _Method.DeclaringType.GetInterfaces().Select(_Interface => _Method.DeclaringType.GetInterfaceMap(_Interface)).SelectMany(_Map => _Map.TargetMethods.Zip(_Map.InterfaceMethods, (_Interface, _Implementation) => new { Interface = _Interface, Implementation = _Implementation })).FirstOrDefault(_Entry => _Entry.Implementation == _Method);
                    if (_entry != null)
                    {
                        if (_entry.Interface.IsDefined(type, true) || _entry.Interface.DeclaringType.IsDefined(type, true)) { return true; }
                        _property = _entry.Interface.Property();
                        if (_property != null && _property.IsDefined(type, true)) { return true; }
                    }
                    return false;
                });
            }
            else if (type.IsInterface)
            {
                Aspect.Release(_Method =>
                {
                    if (type.IsAssignableFrom(_Method.DeclaringType)) { return _Method.DeclaringType.GetInterfaceMap(type).TargetMethods.Contains(_Method); }
                    return false;
                });
            }
            else if (type.IsClass)
            {
                Aspect.Release(_Method =>
                {
                    if (_Method.IsPublic && type.IsAssignableFrom(_Method.DeclaringType)) { return true; }
                    return false;
                });
            }
            else { throw new NotSupportedException(); }
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
        static public void Release<T>(MethodBase method)
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
        static public void Release<T>(Func<MethodBase, bool> pattern)
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
        /// <param name="type">Custom attribute type</param>
        static public void Release<T>(Type type)
            where T : class, IAspect, new()
        {
            if (Metadata<Attribute>.Type.IsAssignableFrom(type))
            {
                Aspect.Release<T>(_Method =>
                {
                    if (_Method.IsAbstract) { return false; }
                    if (_Method.IsDefined(type, true) || _Method.DeclaringType.IsDefined(type, true)) { return true; }
                    var _property = _Method.Property();
                    if (_property != null && _property.IsDefined(type, true)) { return true; }
                    var _method = _Method.GetBaseDefinition();
                    if (_method.IsDefined(type, true) || _method.DeclaringType.IsDefined(type, true)) { return true; }
                    _property = _method.Property();
                    if (_property != null && _property.IsDefined(type, true)) { return true; }
                    var _entry = _Method.DeclaringType.GetInterfaces().Select(_Interface => _Method.DeclaringType.GetInterfaceMap(_Interface)).SelectMany(_Map => _Map.TargetMethods.Zip(_Map.InterfaceMethods, (_Interface, _Implementation) => new { Interface = _Interface, Implementation = _Implementation })).FirstOrDefault(_Entry => _Entry.Implementation == _Method);
                    if (_entry != null)
                    {
                        if (_entry.Interface.IsDefined(type, true) || _entry.Interface.DeclaringType.IsDefined(type, true)) { return true; }
                        _property = _entry.Interface.Property();
                        if (_property != null && _property.IsDefined(type, true)) { return true; }
                    }
                    return false;
                });
            }
            else if (type.IsInterface)
            {
                Aspect.Release<T>(_Method =>
                {
                    if (type.IsAssignableFrom(_Method.DeclaringType)) { return _Method.DeclaringType.GetInterfaceMap(type).TargetMethods.Contains(_Method); }
                    return false;
                });
            }
            else if (type.IsClass)
            {
                Aspect.Release<T>(_Method =>
                {
                    if (_Method.IsPublic && type.IsAssignableFrom(_Method.DeclaringType)) { return true; }
                    return false;
                });
            }
            else { throw new NotSupportedException(); }
        }
    }
}