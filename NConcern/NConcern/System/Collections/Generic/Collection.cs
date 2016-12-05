using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace System.Collections.Generic
{
    internal sealed partial class Collection<T> : IEnumerable<T>
    {
        static public implicit operator T[] (Collection<T> collection)
        {
            var _list = collection.m_List;
            var _length = _list.Count;
            var _array = new T[_length];
            for (var _index = 0; _index < _length; _index++) { _array[_index] = _list[_index]; }
            return _array;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private IList<T> m_List;

        public Collection()
        {
            this.m_List = new T[0];
        }

        public Collection(IEnumerable<T> enumerable)
        {
            this.m_List = new Enumerable<T>(enumerable);
        }

        public Collection(IList<T> list)
        {
            this.m_List = list;
        }

        public Collection(T[] array)
        {
            this.m_List = array;
        }

        public Collection(Collection<T> collection)
        {
            this.m_List = collection.m_List;
        }

        public T this[int index]
        {
            get { return this.m_List[index]; }
            set { this.m_List[index] = value; }
        }

        public int Count
        {
            get { return this.m_List.Count; }
        }

        public int Index(T value)
        {
            return this.m_List.IndexOf(value);
        }

        public IEnumerator<T> Enumerator()
        {
            return this.m_List.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return this.m_List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.m_List.GetEnumerator();
        }
    }

    [DebuggerDisplay("Count = {this.Count, nq}")]
    [DebuggerTypeProxy(typeof(Collection<>.Debugger))]
    internal sealed partial class Collection<T>
    {
        private sealed class Debugger
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private Collection<T> m_Collection;

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            private T[] m_View;

            public Debugger(Collection<T> collection)
            {
                this.m_Collection = collection;
            }

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public T[] View
            {
                get
                {
                    if (this.m_View == null) { this.m_View = this.m_Collection.ToArray(); }
                    return this.m_View;
                }
            }
        }
    }
}