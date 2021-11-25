using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace BordatSystem.ConceptLattices
{
    public class OrderedSet<T> : ICollection<T>
    {
        private readonly IDictionary<T, LinkedListNode<T>> m_Dictionary;
        private readonly LinkedList<T> m_LinkedList;

        public OrderedSet()
            : this(EqualityComparer<T>.Default)
        {
        }

        public OrderedSet(IEqualityComparer<T> comparer)
        {
            m_Dictionary = new Dictionary<T, LinkedListNode<T>>(comparer);
            m_LinkedList = new LinkedList<T>();
        }

        public int Count => m_Dictionary.Count;

        public virtual bool IsReadOnly => m_Dictionary.IsReadOnly;

        void ICollection<T>.Add(T item)
        {
            Add(item);
        }

        void SortedAdd(T item)
        {
            Add(item);
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public bool Add(T item)
        {
            if (m_Dictionary.ContainsKey(item)) return false;
            var node = m_LinkedList.AddLast(item);
            m_Dictionary.Add(item, node);
            return true;
        }

        public void Clear()
        {
            m_LinkedList.Clear();
            m_Dictionary.Clear();
        }

        public bool Remove(T item)
        {
            if (item == null) return false;
            var found = m_Dictionary.TryGetValue(item, out var node);
            if (!found) return false;
            m_Dictionary.Remove(item);
            m_LinkedList.Remove(node);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_LinkedList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(T item)
        {
            return item != null && m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            m_LinkedList.CopyTo(array, arrayIndex);
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((OrderedSet<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((m_Dictionary != null ? m_Dictionary.GetHashCode() : 0) * 397) ^ (m_LinkedList != null ? m_LinkedList.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var obj in m_LinkedList)
            {
                sb.Append(obj);
                if (!obj.Equals(m_LinkedList.Last()))
                    sb.Append(", ");
            }

            if (Count == 0)
                sb.Append("∅");

            sb.Append("}");
            return sb.ToString();
        }
    }
}
