using System;
using System.Collections;

namespace System.Collections.Generic
{
    internal sealed partial class Enumerable<T> : IList<T>
    {
        static public implicit operator T[] (Enumerable<T> enumerable)
        {
            var _list = enumerable.m_List;
            var _length = enumerable.Count;
            var _array = new T[_length];
            for (var _index = 0; _index < _length; _index++) { _array[_index] = _list[_index]; }
            return _array;
        }

        private readonly IEnumerable<T> m_Enumerable;
        private readonly IList<T> m_List;
        private readonly IEnumerator<T> m_Enumerator;
        private bool m_Activated;

        public Enumerable(IEnumerable<T> enumerable)
        {
            this.m_Enumerable = enumerable;
            if (enumerable is IList<T>) 
            { 
                this.m_Activated = true;
                this.m_List = enumerable as IList<T>;
            }
            else
            {
                this.m_Enumerator = this.m_Enumerable.GetEnumerator();
                this.m_List = new List<T>();
            }
        }

        public T this[int index]
        {
            get 
            {
                if (this.m_Activated) { return this.m_List[index]; }
                var _enumerator = new Enumerable<T>.Enumerator(this);
                var _index = 0;
                while (_enumerator.MoveNext())
                {
                    _index++;
                    if (_index == index) { return _enumerator.Current; }
                }
                throw new IndexOutOfRangeException();
            }
            set { throw new NotImplementedException(); }
        }

        public int Count
        {
            get
            {
                if (this.m_Activated) { return this.m_List.Count; }
                var _enumerator = new Enumerable<T>.Enumerator(this);
                while (_enumerator.MoveNext()) { }
                return this.m_List.Count;
            }
        }

        public bool Contains(T item)
        {
            if (this.m_Activated) { return this.m_List.Contains(item); }
            var _enumerator = new Enumerable<T>.Enumerator(this);
            while (_enumerator.MoveNext()) { if (object.Equals(_enumerator.Current, item)) { return true; } }
            return false;
        }

        public void CopyTo(T[] buffer, int index)
        {
            if (this.m_Activated) { this.m_List.CopyTo(buffer, index); }
            else
            {
                var _enumerator = new Enumerable<T>.Enumerator(this);
                var _index = index;
                while (_enumerator.MoveNext()) { buffer[_index++] = _enumerator.Current; }
            }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (this.m_Activated) 
            {
                if (this.m_List != null) { return this.m_List.GetEnumerator(); }
                else { return this.m_Enumerable.GetEnumerator(); }
            }
            else { return new Enumerable<T>.Enumerator(this); }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (this.m_Activated)
            {
                if (this.m_List != null) { return this.m_List.GetEnumerator(); }
                else { return this.m_Enumerable.GetEnumerator(); }
            }
            else { return new Enumerable<T>.Enumerator(this); }
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }
    }
}
