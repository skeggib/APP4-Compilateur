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
                case TokenCategory.TokOpeningParenthesis: // TODO
                    break;
            }

            return null;
        }

        //private Node Factor()
        //{
        //    Token curr = _tokens[_index];


        //}
    }
}
