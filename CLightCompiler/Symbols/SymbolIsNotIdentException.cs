﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LexicalAnalysis;

namespace Symbols
{
    public class SymbolIsNotIdentException : SymbolException // TODO
    {
        public SymbolIsNotIdentException(Token token) : 
            base(token)
        {
        }
    }
}
