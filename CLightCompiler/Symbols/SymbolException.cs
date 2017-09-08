using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;

namespace Symbols
{
    public class SymbolException : Exception // TODO
    {
        public Token Token { get; private set; }

        public SymbolException(Token token)
        {
            Token = token;
        }
    }
}
