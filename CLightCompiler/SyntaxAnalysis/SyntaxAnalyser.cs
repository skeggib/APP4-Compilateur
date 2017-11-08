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

            return Terme();
        }

        private Node Primary()
        {
            if (_index >= _tokens.Count)
                return null;

            switch (_tokens[_index].Category)
            {
                case TokenCategory.TokIdent:
                    _index++;
                    return new Node(NodeCategory.NodeRefVar, _tokens[_index-1].Ident);

                case TokenCategory.TokValue:
                    _index++;
                    return new Node(NodeCategory.NodeConst, _tokens[_index-1].Value);

                case TokenCategory.TokMinus: // Revoir si deux '-' a la suite sont valides
                    _index++;
                    Node n = Primary();
                    if (n == null)
                        throw new SyntaxException(_tokens[_index].Offset, "unexpected token after '-'");
                    return new Node(NodeCategory.NodeNegative, null, n);

                case TokenCategory.TokOpeningParenthesis:
                    _index++;
                    Node n1 = Terme();
                    if (n1 == null)
                        throw new SyntaxException(_tokens[_index].Offset, "unexpected expression");
                    if (_tokens[_index].Category != TokenCategory.TokClosingParenthesis)
                        throw new SyntaxException(_tokens[_index].Offset, "unexpected ')'");
                    _index++;
                    return n1;
            }

            return null;
        }

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
            else
                return p;

            _index++;
            if (_index >= _tokens.Count)
                throw new SyntaxException(_tokens[_index-1].Offset, "Missing operand");

            Node f = Factor();
            if (f == null)
                throw new SyntaxException(_tokens[_index-1].Offset, "Invalid operand");

            op.Childs.Add(f);
            return op;
        }

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
                throw new SyntaxException(_tokens[_index-1].Offset, "Missing operand");

            Node t = Terme();
            if (t == null)
                throw new SyntaxException(_tokens[_index-1].Offset, "Invalid operand");

            op.Childs.Add(t);
            return op;
        }
    }
}
