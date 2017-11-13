using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;

namespace SemanticAnalysis
{
    public class SymbolNotFoundException : SemanticException
    {
        public SymbolNotFoundException(Token token) : 
            base(token.Offset, "Symbol not found")
        {
        }
    }
}
