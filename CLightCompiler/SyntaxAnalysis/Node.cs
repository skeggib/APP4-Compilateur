using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis
{
    public class Node
    {
        private static int _globalId = 0;

        private int _id;

        public NodeCategory Category { get; private set; }

        /// <summary>
        /// Contient la valeur si le noeud est une constante et l'identifiant
        /// si le noeud est une reference, aleur indeterminee pour les autres
        /// categories de noeud
        /// </summary>
        public string Value { get; private set; }

        public IList<Node> Childs { get; private set; }

        public Node(NodeCategory category, string value = null, params Node[] childs)
        {
            if ((category == NodeCategory.NodeConst ||
                category == NodeCategory.NodeRefFunc ||
                category == NodeCategory.NodeRefVar) &&
                value == null)
                throw new ArgumentException("The value cannot be null if the node is a const or a ref", nameof(value));

            _id = _globalId;
            _globalId++;

            Category = category;
            Value = value;

            if (childs != null)
                Childs = new List<Node>(childs);
        }

        public override string ToString()
        {
            string str = string.Empty;
            str += $"[{_id}] {Category.ToString()} {{";

            for (int i = 0; i < Childs.Count; i++)
            {
                if (i != 0)
                    str += ", ";
                str += Childs[i]._id;
            }

            str += "}\n";

            for (int i = 0; i < Childs.Count; i++)
            {
                str += Childs[i].ToString();
            }

            return str;
        }

        public override bool Equals(object obj)
        {
            Node other = obj as Node;
            if (other == null)
                return false;

            if (other.Category != Category)
                return false;

            if ((Category == NodeCategory.NodeConst ||
                Category == NodeCategory.NodeRefFunc ||
                Category == NodeCategory.NodeRefVar) &&
                !(other.Value?.Equals(Value) ?? ReferenceEquals(other.Value, Value)))
                return false;

            if (other.Childs != null && Childs != null)
            {
                if (other.Childs.Count != Childs.Count)
                    return false;
                for (int i = 0; i < Childs.Count; i++)
                    if (!other.Childs[i].Equals(Childs[i]))
                        return false;
            }
            else if (!ReferenceEquals(other.Childs, Childs))
                return false;

            return true;
        }
    }
}
