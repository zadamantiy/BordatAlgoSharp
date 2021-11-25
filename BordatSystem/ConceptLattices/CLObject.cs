using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BordatSystem.ConceptLattices
{
    class CLObject
    {
        public OrderedSet<char> Attributes;
        public int Id;

        public CLObject(IEnumerable<char> attributes, int id)
        {
            Attributes = new OrderedSet<char>() { attributes };
            Id = id;
        }

        public CLObject Copy()
        {
            return new CLObject(Attributes, Id);
        }
    }
}
