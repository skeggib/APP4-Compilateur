using LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalysis
{
    public class SyntaxAnalyser
    {
        private IList<Token> _tokens;
        private int _index;

        public Node Convert(IList<Token> tokens)
        {
            _tokens = tokens;
            _index = 0;

            return Expression();
        }

        // P -> ident | const | -P | !P | (E)
        private Node Primary()
        {
            if (_index >= _tokens.Count)
                return null;

            switch (_tokens[_index].Category)
            {
                case Tokens.Ident:
                    _index++;
                    return new Node(Nodes.RefVar, _tokens[_index - 1].Ident);

                case Tokens.Value:
                    _index++;
                    return new Node(Nodes.Const, _tokens[_index - 1].Value);

                case Tokens.Minus: // TODO Revoir si deux '-' a la suite sont valides
                    {
                        _index++;
                        Node p = Primary();
                        if (p == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected token after '-'");
                        return new Node(Nodes.Negative, null, p);
                    }

                case Tokens.Not:
                    {
                        _index++;
                        Node p = Primary();
                        if (p == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected token after '!'");
                        return new Node(Nodes.Not, null, p);
                    }

                case Tokens.OpeningParenthesis:
                    _index++;
                    Node e = Expression();
                    if (e == null)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected expression");
                    if (_tokens[_index].Category != Tokens.ClosingParenthesis)
                        throw new SyntaxException(_tokens[_index].Offset, "expected ')'");
                    _index++;
                    return e;
            }

            return null;
        }

        // F -> P [ ( * | / ) F ]
        private Node Factor()
        {
            if (_index >= _tokens.Count)
                return null;

            Node p = Primary();
            if (p == null)
                return null;

            if (_index >= _tokens.Count)
                return p;

            Node op = null;
            if (_tokens[_index].Category == Tokens.Multiply)
                op = new Node(Nodes.Multiplication, null, p);
            else if (_tokens[_index].Category == Tokens.Divide)
                op = new Node(Nodes.Division, null, p);
            else if (_tokens[_index].Category == Tokens.Modulo)
                op = new Node(Nodes.Modulo, null, p);
            else
                return p;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node f = Factor();
            if (f == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(f);
            return op;
        }
        
        // T -> F [ ( + | - ) T ]
        private Node Terme()
        {
            if (_index >= _tokens.Count)
                return null;

            Node f = Factor();
            if (f == null)
                return null;

            if (_index >= _tokens.Count)
                return f;

            Node op = null;
            if (_tokens[_index].Category == Tokens.Plus)
                op = new Node(Nodes.Addition, null, f);
            else if (_tokens[_index].Category == Tokens.Minus)
                op = new Node(Nodes.Substraction, null, f);
            else
                return f;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node t = Terme();
            if (t == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(t);
            return op;
        }

        // C -> T [ ( == | != | < | > | <= | >= ) C ]
        private Node Comparaison()
        {
            if (_index >= _tokens.Count)
                return null;

            Node t = Terme();
            if (t == null)
                return null;

            if (_index >= _tokens.Count)
                return t;

            Node op = null;
            switch (_tokens[_index].Category)
            {
                case Tokens.Equals:
                    op = new Node(Nodes.AreEqual, null, t);
                    break;

                case Tokens.NotEquals:
                    op = new Node(Nodes.AreNotEqual, null, t);
                    break;

                case Tokens.LowerThan:
                    op = new Node(Nodes.LowerThan, null, t);
                    break;

                case Tokens.GreaterThan:
                    op = new Node(Nodes.GreaterThan, null, t);
                    break;

                case Tokens.LowerOrEqual:
                    op = new Node(Nodes.LowerOrEqual, null, t);
                    break;

                case Tokens.GreaterOrEqual:
                    op = new Node(Nodes.GreaterOrEqual, null, t);
                    break;

                default:
                    return t;
            }

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node c = Comparaison();
            if (c == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(c);
            return op;
        }

        // L -> C [ && L ]
        private Node Logic()
        {
            if (_index >= _tokens.Count)
                return null;

            Node c = Comparaison();
            if (c == null)
                return null;

            if (_index >= _tokens.Count)
                return c;

            Node op = null;
            if (_tokens[_index].Category == Tokens.And)
                op = new Node(Nodes.And, null, c);
            else
                return c;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node l = Logic();
            if (l == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(l);
            return op;
        }

        // E -> L [ || E ]
        private Node Expression()
        {
            if (_index >= _tokens.Count)
                return null;

            Node l = Logic();
            if (l == null)
                return null;

            if (_index >= _tokens.Count)
                return l;

            Node op = null;
            if (_tokens[_index].Category == Tokens.Or)
                op = new Node(Nodes.Or, null, l);
            else
                return l;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Missing operand");

            Node e = Expression();
            if (e == null)
                throw new SyntaxException(_tokens[_index - 1].Offset, "Invalid operand");

            op.Childs.Add(e);
            return op;
        }
    }
}
