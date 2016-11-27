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

        //public Collection(IEnumerable<T> enumerable, int index)
        //{
        //    this.m_List = new Stub<T>(new Enumerable<T>(enumerable), index);
        //}

        //public Collection(IEnumerable<T> enumerable, int index, int count)
        //{
        //    this.m_List = new Segment<T>(new Enumerable<T>(enumerable), index, count);
        //}

        public Collection(IList<T> list)
        {
            this.m_List = list;
        }
        
        //public Collection(IList<T> list, int index)
        //{
        //    this.m_List = new Stub<T>(list, index);
        //}

        //public Collection(IList<T> list, int index, int count)
        //{
        //    this.m_List = new Segment<T>(list, index, count);
        //}

        public Collection(T[] array)
        {
            this.m_List = array;
        }

        //public Collection(T[] array, int index)
        //{
        //    this.m_List = new Stub<T>(array, index);
        //}

        //public Collection(T[] array, int index, int count)
        //{
        //    this.m_List = new Segment<T>(array, index, count);
        //}

        public Collection(Collection<T> collection)
        {
            this.m_List = collection.m_List;
        }

        //public Collection(Collection<T> collection, int index)
        //{
        //    this.m_List = new Stub<T>(collection.m_List, index);
        //}

        //public Collection(Collection<T> collection, int index, int count)
        //{
        //    this.m_List = new Segment<T>(collection.m_List, index, count);
        //}

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