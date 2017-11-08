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

            return null;
        }

        private Node Primary()
        {
            Token curr = _tokens[_index];

            switch (curr.Category)
            {
                case TokenCategory.TokIdent:
                    return new Node(NodeCategory.NodeRefVar, curr.Ident);
                case TokenCategory.TokValue:
                    return new Node(NodeCategory.NodeConst, curr.Value);
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
                    _index++;
                    if (_tokens[_index].Category != TokenCategory.TokClosingParenthesis)
                        throw new SyntaxException(_tokens[_index].Offset, "unexpected ')'");
                    return n1;
            }

            return null;
        }

        private Node Factor()
        {
            Token curr = _tokens[_index];
            Node n1 = Primary();
            if (n1 == null)
                return null;
            _index++;
            curr = _tokens[_index];
            _index++;
            Node n2 = Factor();
            if (curr.Category == TokenCategory.TokMultiply)
                return new Node(NodeCategory.NodeMultiplication, null, n1, n2);
            else if (curr.Category == TokenCategory.TokDivide)
                return new Node(NodeCategory.NodeDivision, null, n1, n2);
            _index--;
            return n1;
        }

        private Node Terme()
        {
            Token curr = _tokens[_index];
            Node n1 = Primary();
            if (n1 == null)
                return null;
            _index++;
            curr = _tokens[_index];
            _index++;
            Node n2 = Terme();
            if (curr.Category == TokenCategory.TokPlus)
                return new Node(NodeCategory.NodeAddition, null, n1, n2);
            else if (curr.Category == TokenCategory.TokMinus)
                return new Node(NodeCategory.NodeSubstraction, null, n1, n2);
            _index--;
            return n1;
        }
    }
}
