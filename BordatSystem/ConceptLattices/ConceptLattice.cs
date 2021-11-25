using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BordatSystem.ConceptLattices
{
    class ConceptLattice : IEquatable<ConceptLattice>
    {
        public readonly SortedSetExtension<char> Attributes;
        public readonly SortedSetExtension<int> Objects;

        public ConceptLattice(IEnumerable<int> objects, IEnumerable<char> attributes)
        {
            Attributes = new SortedSetExtension<char>(attributes);
            Objects = new SortedSetExtension<int>(objects);
        }

        public List<int> GetObjectsCopy()
        {
            return new List<int>(Objects);
        }

        public List<char> GetAttributesCopy()
        {
            return new List<char>(Attributes);
        }

        public bool Equals(ConceptLattice other)
        {
            if (other == null)
                return false;

            if (other.Objects.Count != Objects.Count)
                return false;

            if (other.Attributes.Count != Attributes.Count)
                return false;

            foreach (var obj in Objects)
            {
                if (!other.Objects.Contains(obj))
                    return false;
            }

            foreach (var attribute in Attributes)
            {
                if (!other.Attributes.Contains(attribute))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            var hash = Objects.Aggregate(0, (current, o) => current ^ o);
            return Attributes.Aggregate(hash, (current, a) => current ^ a);
        }

        public override string ToString()
        {
            return $"({Objects}, {Attributes})";
        }
    }
}
