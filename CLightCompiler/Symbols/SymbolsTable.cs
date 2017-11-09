using LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symbols
{
    public class SymbolsTable
    {
        private Stack<Dictionary<string, Symbol>> _stack;

        public SymbolsTable()
        {
            _stack = new Stack<Dictionary<string, Symbol>>();
            StartBlock();
        }

        public void StartBlock()
        {
            _stack.Push(new Dictionary<string, Symbol>());
        }

        public void EndBlock()
        {
            _stack.Pop();
        }

        public Symbol AddSymbol(Token token)
        {
            if (token.Category != Tokens.Ident)
                throw new SymbolIsNotIdentException(token);

            if (_stack.Peek().ContainsKey(token.Ident))
                throw new SymbolAlreadyExistsException(token);

            Symbol symbol = new Symbol();
            _stack.Peek().Add(token.Ident, symbol);

            return symbol;
        }

        public Symbol GetSymbol(Token token)
        {
            if (token.Category != Tokens.Ident)
                throw new SymbolIsNotIdentException(token);

            Symbol symbol = null;

            Stack<Dictionary<string, Symbol>> tempStack = new Stack<Dictionary<string, Symbol>>();
            while (_stack.Count() > 0 && !_stack.Peek().ContainsKey(token.Ident))
                tempStack.Push(_stack.Pop());

            if (_stack.Count() > 0)
                symbol = _stack.Peek()[token.Ident];

            while (tempStack.Count() > 0)
                _stack.Push(tempStack.Pop());

            if (symbol == null)
                throw new SymbolNotFoundException(token);

            return symbol;
        }
    }
}
