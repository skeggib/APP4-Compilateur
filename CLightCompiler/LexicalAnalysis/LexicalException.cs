using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    public class LexicalException : Exception
    {
        public int Offset { get; private set; }

        public LexicalException(int offset)
        {
            Offset = offset;
        }
    }
}
