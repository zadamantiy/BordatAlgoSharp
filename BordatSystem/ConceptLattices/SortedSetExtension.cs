using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BordatSystem.ConceptLattices
{
    class SortedSetExtension<T> : SortedSet<T>
    {
        public SortedSetExtension() {}

        public SortedSetExtension(IEnumerable<T> input)
        {
            foreach (var obj in input)
                Add(obj);
        }

        public void Add(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var obj in this)
            {
                sb.Append(obj);
                if (!obj.Equals(this.Last()))
                    sb.Append(", ");
            }

            if (Count == 0)
                sb.Append("∅");

            sb.Append("}");
            return sb.ToString();
        }
    }
}
