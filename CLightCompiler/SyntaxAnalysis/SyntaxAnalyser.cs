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
                case TokenCategory.TokIdent:
                    _index++;
                    return new Node(NodeCategory.NodeRefVar, _tokens[_index - 1].Ident);

                case TokenCategory.TokValue:
                    _index++;
                    return new Node(NodeCategory.NodeConst, _tokens[_index - 1].Value);

                case TokenCategory.TokMinus: // TODO Revoir si deux '-' a la suite sont valides
                    {
                        _index++;
                        Node p = Primary();
                        if (p == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected token after '-'");
                        return new Node(NodeCategory.NodeNegative, null, p);
                    }

                case TokenCategory.TokNot:
                    {
                        _index++;
                        Node p = Primary();
                        if (p == null)
                            throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected token after '!'");
                        return new Node(NodeCategory.NodeNot, null, p);
                    }

                case TokenCategory.TokOpeningParenthesis:
                    _index++;
                    Node e = Expression();
                    if (e == null)
                        throw new SyntaxException(_tokens[_index - 1].Offset, "unexpected expression");
                    if (_tokens[_index].Category != TokenCategory.TokClosingParenthesis)
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
            if (_tokens[_index].Category == TokenCategory.TokMultiply)
                op = new Node(NodeCategory.NodeMultiplication, null, p);
            else if (_tokens[_index].Category == TokenCategory.TokDivide)
                op = new Node(NodeCategory.NodeDivision, null, p);
            else if (_tokens[_index].Category == TokenCategory.TokModulo)
                op = new Node(NodeCategory.NodeModulo, null, p);
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
            if (_tokens[_index].Category == TokenCategory.TokPlus)
                op = new Node(NodeCategory.NodeAddition, null, f);
            else if (_tokens[_index].Category == TokenCategory.TokMinus)
                op = new Node(NodeCategory.NodeSubstraction, null, f);
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
                case TokenCategory.TokEquals:
                    op = new Node(NodeCategory.NodeAreEqual, null, t);
                    break;

                case TokenCategory.TokNotEquals:
                    op = new Node(NodeCategory.NodeAreNotEqual, null, t);
                    break;

                case TokenCategory.TokLowerThan:
                    op = new Node(NodeCategory.NodeLowerThan, null, t);
                    break;

                case TokenCategory.TokGreaterThan:
                    op = new Node(NodeCategory.NodeGreaterThan, null, t);
                    break;

                case TokenCategory.TokLowerOrEquals:
                    op = new Node(NodeCategory.NodeLowerOrEqual, null, t);
                    break;

                case TokenCategory.TokGreaterOrEquals:
                    op = new Node(NodeCategory.NodeGreaterOrEqual, null, t);
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
            if (_tokens[_index].Category == TokenCategory.TokAnd)
                op = new Node(NodeCategory.NodeLogicAnd, null, c);
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
            if (_tokens[_index].Category == TokenCategory.TokOr)
                op = new Node(NodeCategory.NodeLogicOr, null, l);
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
