using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;

namespace SemanticAnalysis
{
    public class SymbolAlreadyExistsException : SymbolException // TODO
    {
        public SymbolAlreadyExistsException(Token token) : 
            base(token)
        {
        }
    }
}
