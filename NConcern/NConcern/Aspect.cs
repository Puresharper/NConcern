using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace NConcern
{
    abstract public partial class Aspect : IDisposable
    {
        [ThreadStatic]
        static internal Aspect Current;

        static private long m_Sequence = 0L;

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool Equals(object left, object right)
        {
            return object.Equals(left, right);
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new static public bool ReferenceEquals(object left, object right)
        {
            return object.ReferenceEquals(left, right);
        }

        internal readonly long Identity;
        private readonly Resource m_Resource;
        private readonly LinkedList<Type> m_Typology;
        private readonly Dictionary<Type, Action> m_Dispose;

        /// <summary>
        /// Create an aspect.
        /// </summary>
        protected Aspect()
        {
            this.Identity = Interlocked.Increment(ref Aspect.m_Sequence);
            this.m_Resource = new Resource();
            this.m_Typology = new LinkedList<Type>();
            this.m_Dispose = new Dictionary<Type, Action>();
        }

        /// <summary>
        /// Advise a method for a specific type.
        /// </summary>
        /// <typeparam name="T">Specific target type.</typeparam>
        /// <param name="method">Method to advise.</param>
        /// <returns>Advice.</returns>
        abstract protected IEnumerable<Advice<T>> Advise<T>(MethodInfo method)
            where T : class;

        /// <summary>
        /// Attach aspect to a target class.
        /// </summary>
        /// <typeparam name="T">Type of target class.</typeparam>
        public void Manage<T>()
            where T : class
        {
            lock (this.m_Resource)
            {
                if (this.m_Dispose.ContainsKey(Metadata<T>.Type)) { return; }
                var _current = Aspect.Current;
                try
                {
                    Aspect.Current = this;
                    this.m_Typology.AddLast(Metadata<T>.Type);
                    this.m_Dispose.Add(Metadata<T>.Type, this.Dispose<T>);
                    foreach (var _method in Aspect.Topography<T>.Dictionary.Keys)
                    {
                        if (_method.IsStatic) { continue; }
                        Aspect.Topography<T>.Dictionary[_method].Add(this);
                    }
                }
                finally { Aspect.Current = _current; }
            }
        }

        /// <summary>
        /// Attach aspect to a target class.
        /// </summary>
        /// <param name="type">Type of target class.</param>
        public void Manage(Type type)
        {
            if (type.IsValueType || type.IsInterface || (type.IsAbstract && type.IsSealed)) { throw new NotSupportedException(); }
            lock (this.m_Resource)
            {
                if (this.m_Dispose.ContainsKey(type)) { return; }
                var _current = Aspect.Current;
                try
                {
                    Aspect.Current = this;
                    this.m_Typology.AddLast(type);
                    this.m_Dispose.Add(type, () => this.Dispose(type));
                    var _dictionary = Aspect.Topography.Dictionary(type);
                    foreach (var _method in _dictionary.Keys)
                    {
                        if (_method.IsStatic) { continue; }
                        _dictionary[_method].Add(this);
                    }
                }
                finally { Aspect.Current = _current; }
            }
        }

        /// <summary>
        /// Detach aspect from target class.
        /// </summary>
        /// <typeparam name="T">Type of target class.</typeparam>
        [DebuggerHidden]
        public void Dispose<T>()
            where T : class
        {
            lock (this.m_Resource)
            {
                if (this.m_Dispose.Remove(Metadata<T>.Type))
                {
                    this.m_Typology.Remove(Metadata<T>.Type);
                    foreach (var _method in Aspect.Topography<T>.Dictionary.Keys) { Aspect.Topography<T>.Dictionary[_method].Remove(this); }
                }
            }
        }

        /// <summary>
        /// Detach aspect from target class.
        /// </summary>
        /// <param name="type">Type of target class.</param>
        public void Dispose(Type type)
        {
            if (type.IsValueType || type.IsInterface || (type.IsAbstract && type.IsSealed)) { throw new NotSupportedException(); }
            lock (this.m_Resource)
            {
                if (this.m_Dispose.Remove(type))
                {
                    this.m_Typology.Remove(type);
                    var _dictionary = Aspect.Topography.Dictionary(type);
                    foreach (var _method in _dictionary.Keys) { _dictionary[_method].Remove(this); }
                }
            }
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        sealed override public bool Equals(object value)
        {
            return base.Equals(value);
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        sealed override public int GetHashCode()
        {
            return base.GetHashCode();
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        sealed override public string ToString()
        {
            return base.ToString();
        }

        [DebuggerHidden]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public Type GetType()
        {
            return base.GetType();
        }

        /// <summary>
        /// Dispose aspect and detach it from all managed class.
        /// </summary>
        public void Dispose()
        {
            lock (this.m_Resource)
            {
                foreach (var _dispose in this.m_Dispose.Values.ToArray())
                {
                    _dispose();
                }
            }
        }

        ~Aspect()
        {
            this.Dispose();
        }
    }

    [DebuggerDisplay("{this.GetType().Name, nq} [{this.m_Typology.Count, nq}]")]
    [DebuggerTypeProxy(typeof(Aspect.Debugger))]
    abstract public partial class Aspect
    {
        private class Debugger
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Aspect m_Aspect;

            public Debugger(Aspect aspect)
            {
                this.m_Aspect = aspect;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public Type[] View
            {
                get
                {
                    lock (this.m_Aspect.m_Resource)
                    {
                        return this.m_Aspect.m_Typology.Reverse().ToArray();
                    }
                }
            }
        }
    }
}
