using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    public class Token
    {
        public TokenCategory Category { get; set; }
        public string Ident { get; set; }
        public int Value { get; set; }
        public int Offset { get; set; }

        public Token(TokenCategory category, int offset)
        {
            Category = category;
            Ident = string.Empty;
            Value = 0;
            Offset = offset;
        }
    }
}
