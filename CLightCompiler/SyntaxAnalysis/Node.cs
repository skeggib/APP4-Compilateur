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
        /// Contient la valeur si le noeud est une constante et l'identifiant si le noeud est une reference
        /// </summary>
        public string Value { get; private set; }

        public IList<Node> Childs { get; private set; }

        public Node(NodeCategory category, string value = null, params Node[] childs)
        {
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
    }
}
