using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;

namespace SemanticAnalysis
{
    public class SymbolIsNotIdentException : SemanticException
    {
        public SymbolIsNotIdentException(Token token) :
            base(token.Offset, "Symbol is not an identifier")
        {
        }
    }
}
