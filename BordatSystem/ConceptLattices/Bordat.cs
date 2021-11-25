using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BordatSystem.ConceptLattices
{
    class Bordat
    {
        public OrderedSet<ConceptLattice> L;
        private OrderedSet<ConceptLattice> LN;
        private Dictionary<int, CLObject> _objects;
        private Dictionary<ConceptLattice, OrderedSet<ConceptLattice>> Ch;
        private Dictionary<ConceptLattice, OrderedSet<ConceptLattice>> _upperNeighbourDictionary;
        private ConceptLattice GGStroke;
        private StringBuilder _log = new StringBuilder();

        public List<ConceptLattice> DesiredConceptLattices = new List<ConceptLattice>();

        public Bordat() {}

        private void LogLine(object obj)
        {
            _log.Append($"{obj.ToString()}\n");
        }

        private void Log(object obj)
        {
            _log.Append($"{obj.ToString()}");
        }

        public List<ConceptLattice> Proceed(IEnumerable<CLObject> objects)
        {
            _log = new StringBuilder();

            var clObjects = objects.ToList();
            _objects = new Dictionary<int, CLObject>();
            foreach (var obj in clObjects)
                _objects.Add(obj.Id, obj);

            Ch = new Dictionary<ConceptLattice, OrderedSet<ConceptLattice>>();
            _upperNeighbourDictionary = new Dictionary<ConceptLattice, OrderedSet<ConceptLattice>>();
            DesiredConceptLattices = new List<ConceptLattice>();
            var objIds = clObjects.Select(obj => obj.Id).ToList();
            GGStroke = new ConceptLattice(objIds, new List<char>());
            LogLine($"{GGStroke} is the maximal concept");
            L = new OrderedSet<ConceptLattice>();
            LogLine("L = ∅");
            LogLine($"Process({GGStroke}, {GGStroke.Attributes})");
            Process(GGStroke, new SortedSetExtension<char>(GGStroke.Attributes));
            //LogLine($"{L} is the concept set");

            var allAttributes = new SortedSetExtension<char>();
            foreach (var obj in objects)
                allAttributes.Add(obj.Attributes);

            var objectsWithSuchAttributes = new SortedSetExtension<int>();
            foreach (var obj in objects)
            {
                if (obj.Attributes.Count == allAttributes.Count)
                    objectsWithSuchAttributes.Add(obj.Id);
            }

            var clForEmptyObjSet = new ConceptLattice(objectsWithSuchAttributes, allAttributes);
            LogLine($"L = L ⋃ {clForEmptyObjSet}");
            L.Add(clForEmptyObjSet);
            LogLine($"{L} is the concept set");
            return L.ToList();
        }

        private void Process(ConceptLattice AB, SortedSetExtension<char> inC)
        {
            var A = AB.Objects;
            var B = AB.Attributes;
            var C = new SortedSetExtension<char>(inC);
            //L := L v {(A, B)}
            LogLine($"L = L ⋃ {{A, B}}");
            LogLine($"L = {L} ⋃ {AB}");
            L.Add(AB);
            //LN := LowerNeighbours ((A, B))
            LogLine($"LowerNeighbours({AB})");
            LowerNeighbours(AB);
            LogLine($"LN = {LN}");
            //For each (D, E) IN LN
            LogLine($"For each (D, E) ∈ LN");
            foreach (var DE in LN)
            {
                LogLine($"(D, E) = {DE}");
                var cIntersectionE = Intersection(C, DE.Attributes);
                LogLine($"C ⋂ E = {cIntersectionE}");
                var CintersectionEequalB = SetsAreEqual(cIntersectionE, B);
                LogLine($"If C ⋂ E = B");
                LogLine($"{cIntersectionE} = {B} → {CintersectionEequalB}");
                if (CintersectionEequalB)
                {
                    //C = cIntersectionE;
                    LogLine($"C = C ⋃ E");
                    LogLine($"C = {C} ⋃ {DE.Attributes}");
                    C.Add(DE.Attributes);
                    LogLine($"C = {C}");
                    LogLine($"Process({DE}, {C})");
                    Process(DE, C);
                    /*
                    Log($"Ch((A, B)) = Ch((A, B)) ⋃ {{DE}}");
                    AddToCh(AB, DE);
                    LogLine($" = {Ch[AB]} ⋃ {DE}");
                    */
                }
                else
                {
                    /*
                    LogLine($"Else");
                    LogLine($"//Here we can call Find({GGStroke}, {DE})");
                    //find(GGStroke, DE);
                    //AddToCh(AB, DE);
                    */
                }
                //LogLine($"{AB} is an upper neighbour of {DE}");
                //AddToUpperNeighbour(DE, AB);
            }
            LogLine("//End of Process");
        }

        private void find(ConceptLattice AB, ConceptLattice CD)
        {
            var tmp = Ch[AB];
            ConceptLattice EF = null;
            foreach (var concept in Ch[AB])
            {
                EF = concept;
                if (SetContainsSet(EF.Attributes, CD.Attributes))
                    break;
            }
            if (!SetsAreEqual(EF.Attributes, CD.Attributes))
                find(EF, CD);
            else
                DesiredConceptLattices.Add(EF);
        }

        private void LowerNeighbours(ConceptLattice AB)
        {
            //LN := ∅
            LogLine("LN := ∅");
            LN = new OrderedSet<ConceptLattice>();

            //C := B
            var C = new SortedSetExtension<char>() { AB.Attributes };
            //g is the first object in A such that :({g}’  C);
            //if there is no such object, g is the last element of A
            LogLine("g = first object ∈ A such that ¬({g}’ ⊆ C)");
            var g = _objects[AB.Objects.Last()];
            var pos = 0;
            var changed = false;
            foreach (var objId in AB.Objects)
            {
                var obj = _objects[objId];
                var copyOfC = new OrderedSet<char> {C.Intersect(obj.Attributes)};
                if (obj.Attributes.Count == copyOfC.Count)
                {
                    pos++;
                    continue;      
                }

                g = obj;
                changed = true;
                break;
            }

            if (!changed)
            {
                LogLine("Such g is not found");
                LogLine("//End of LowerNeighbours");
                return;
            }

            //LogLine($"g = {g.Id} and g is {(changed ? "not " : "")}last");
            LogLine($"g = {g.Id}");
            LogLine("While (True) //g cycle");
            //while(g != _objects[AB.Objects.Last()])
            //while (/*g != _objects[AB.Objects.Last()]*/changed)
            while(true)
            {
                // E := {g}
                var E = new OrderedSet<int> {g.Id};
                LogLine($"E = {E}");
                // F := {g}’
                var F = g.Attributes;
                LogLine($"F = {F}");
                // h := g
                var h = g.Copy();
                LogLine($"h = {g.Id}");
                //While h is not the last element of A
                var hIsLast = h.Id != AB.Objects.Last();
                LogLine($"While h is not the last element of A");
                LogLine($"h is {(h.Id != AB.Objects.Last() ? "not " : "")}the last element in A");
                while (h.Id != AB.Objects.Last())
                {
                    //h is the next element of A
                    pos++;
                    h = _objects[AB.Objects.ElementAt(pos)];
                    LogLine($"h is the next element of A → h = {h.Id}");
                    //If ¬(F ∩ {h}’ <= C)
                    var fIntersectionH = Intersection(F, h.Attributes);
                    
                    var copyOfC = new OrderedSet<char> { C.Intersect(fIntersectionH) };
                    LogLine($"If ¬(F ∩ {{h}}’ ⊆ C)");
                    LogLine($"If ¬({F} ∩ {h.Attributes} ⊆ {C}) → {fIntersectionH.Count != copyOfC.Count}");
                    if (fIntersectionH.Count != copyOfC.Count)
                    {
                        //E := E v {h}
                        Log($"E = {E} ⋃ {{{h.Id}}}");
                        E.Add(h.Id);
                        LogLine($" = {E}");
                        //F := F ∩ {h}’
                        //F.Add(h.Attributes);
                        Log($"F = {F} ⋂ {{h.Attributes}}");
                        F = new OrderedSet<char> {F.Intersect(h.Attributes)};
                        LogLine($" = {F}");
                    }
                    LogLine($"Next iteration //h cycle");
                    LogLine($"h is {(h.Id != AB.Objects.Last() ? "not " : "")}the last element in A");
                } 

                //If F ∩ C = B
                var fIntersectionC = new OrderedSet<char>(){ F.Intersect(C) };
                var fIntersectionCEqualB = SetsAreEqual(fIntersectionC, AB.Attributes);
                LogLine($"If F ∩ C = B");
                LogLine($"If {F} ∩ {C} = {AB.Attributes} → {fIntersectionCEqualB}");
                if (fIntersectionCEqualB)
                {
                    //LN := LN v {(E, F)}
                    var EF = new ConceptLattice(E, F);
                    LogLine($"LN = {LN} ⋃ {{{EF}}}");
                    LN.Add(EF);
                }
                LogLine($"C = {C} ⋃ {{F}}");
                C.Add(F);
                //TODO: 4.7 + 5

                //g is the first object in A such that !({g}’ <= C)
                //if there is no such object, g is the last element of A
                LogLine("g = first object ∈ A such that ¬({g}’ ⊆ C)");
                g = _objects[AB.Objects.Last()];
                pos = 0;
                changed = false;
                foreach (var objId in AB.Objects)
                {
                    var obj = _objects[objId];
                    var copyOfC = new OrderedSet<char> { C.Intersect(obj.Attributes) };
                    if (obj.Attributes.Count == copyOfC.Count)
                    {
                        pos++;
                        continue;
                    }

                    changed = true;
                    g = obj;
                    break;
                }

                if (!changed)
                {
                    LogLine("Such g is not found");
                    LogLine("END OF LowerNeighbours");
                    return;
                }

                LogLine($"g = {g.Id}");
                LogLine($"Next iteration //g cycle");
            }
        }

        private void AddToCh(ConceptLattice key, ConceptLattice value)
        {
            if (Ch.ContainsKey(key))
                Ch[key].Add(value);
            else
                Ch[key] = new OrderedSet<ConceptLattice>() {value};
        }

        private void AddToUpperNeighbour(ConceptLattice key, ConceptLattice value)
        {
            if (_upperNeighbourDictionary.ContainsKey(key))
                _upperNeighbourDictionary[key].Add(value);
            else
                _upperNeighbourDictionary[key] = new OrderedSet<ConceptLattice>() { value };
        }

        private static SortedSetExtension<T> Intersection<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            var tmp = first.Intersect(second);
            var res = new SortedSetExtension<T>();
            foreach (var obj in tmp)
            {
                res.Add(obj);
            }
            return res;
        }

        private static bool SetsAreEqual<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            return first.Count<T>() == second.Count<T>() && SetContainsSet(first, second);
        }

        private static bool SetContainsSet<T>(IEnumerable<T> set, IEnumerable<T> SetContainer)
        {
            return set.All(SetContainer.Contains);
        }

        public string GetLog()
        {
            return _log.ToString();
        }
    }
}
