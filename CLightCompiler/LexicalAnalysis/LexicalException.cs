using System;
using System.Collections.Generic;
using System.Text;

namespace LexicalAnalysis
{
    public class LexicalException : Exception
    {
        public int Offset { get; private set; }
        public string Word { get; private set; }

        public LexicalException(int offset, string word)
            : base("Lexical exception: '" + word + "' at offset " + offset)
        {
            Offset = offset;
            Word = word;
        }
    }
}
