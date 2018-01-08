﻿using LexicalAnalysis;
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

        public Nodes Category { get; private set; }

        public Token Token { get; private set; }

        public List<Token> Tokens { get; }

        public int Slot { get; set; }

        public IList<Node> Childs { get; private set; }

        public int VarCount;

        public Node(Nodes category, Token token = null, params Node[] childs)
        {
            if ((category == Nodes.Const ||
                category == Nodes.Assign ||
                category == Nodes.DeclVar ||
                category == Nodes.Call ||
                category == Nodes.RefVar) &&
                token == null)
                throw new ArgumentException("The value cannot be null if the node is a const or a ref", nameof(token));

            _id = _globalId;
            _globalId++;

            Category = category;
            Token = token;

            if (childs != null)
                Childs = new List<Node>(childs);

            Tokens = new List<Token>();
        }

        public override string ToString()
        {
            string str = string.Empty;
            str += $"[{_id}] {Category.ToString()}";

            if (Category == Nodes.Const)
                str += $" ({Token.Value})";
            else if (Category == Nodes.Assign ||
                Category == Nodes.DeclVar ||
                Category == Nodes.Call ||
                Category == Nodes.RefVar)
                str += $" ({Token.Ident})";
            else if (Category == Nodes.DeclFunc)
            {
                str += $" {Token.Ident}(";
                if (Tokens.Count == 0)
                    str += "void";
                else
                {
                    for (var i = 0; i < Tokens.Count; ++i)
                    {
                        if (i != 0)
                            str += ",";
                        str += $"{Tokens[i].Ident}";
                    }
                }
                str += ")";
            }

            str += $" {{";

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

            if (!ReferenceEquals(other.Token, Token))
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
