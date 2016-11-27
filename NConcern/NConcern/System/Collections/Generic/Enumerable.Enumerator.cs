using System;
using System.Collections;

namespace System.Collections.Generic
{
    internal sealed partial class Enumerable<T>
    {
        private sealed class Enumerator : IEnumerator<T>
        {
            private Enumerable<T> m_Enumerable;
            private IList<T> m_Collection;
            private int m_Index;

            public Enumerator(Enumerable<T> cache)
            {
                this.m_Enumerable = cache;
                this.m_Collection = m_Enumerable.m_List;
                this.m_Index = -1;
            }

            public T Current
            {
                get { return this.m_Collection[this.m_Index]; }
            }

            object IEnumerator.Current
            {
                get { return this.m_Collection[this.m_Index]; }
            }

            public bool MoveNext()
            {
                if (this.m_Index < this.m_Collection.Count - 1)
                {
                    this.m_Index++;
                    return true;
                }
                if (this.m_Enumerable.m_Activated) { return false; }
                if (this.m_Enumerable.m_Enumerator.MoveNext())
                {
                    this.m_Index++;
                    this.m_Collection.Add(this.m_Enumerable.m_Enumerator.Current);
                    return true;
                }
                this.m_Enumerable.m_Enumerator.Dispose();
                this.m_Enumerable.m_Activated = true;
                return false;
            }

            public void Reset()
            {
                this.m_Index = -1;
            }

            public void Dispose()
            {
            }
        }
    }
}
