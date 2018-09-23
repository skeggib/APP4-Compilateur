using System.Collections.Generic;
using LexicalAnalysis;

namespace SyntaxAnalysis
{
    public interface ISyntaxAnalyser
    {
        Node Convert(IList<Token> tokens);
    }
}
